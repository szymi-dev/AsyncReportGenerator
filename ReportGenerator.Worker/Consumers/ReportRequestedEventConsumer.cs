using Amazon.S3;
using Amazon.S3.Model;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportGenerator.Application.Events;
using ReportGenerator.Infrastructure.Persistence;

namespace ReportGenerator.Worker.Consumers;

public class ReportRequestedEventConsumer : IConsumer<ReportRequestedEvent>
{
    private readonly ApplicationDbContext _dbContext;

    public ReportRequestedEventConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReportRequestedEvent> context)
    {
        var message = context.Message;

        var report = await _dbContext.Reports.FindAsync(new object[] { message.ReportId }, context.CancellationToken);
        if (report == null)
        {
            return;
        }

        report.MarkAsProcessing();
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        var fileName = $"raport_{message.ReportId}.pdf";

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Raport: {message.ReportName}")
                    .SemiBold().FontSize(30).FontColor(Colors.Blue.Darken2);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);
                        x.Item().Text("Raport xyz");
                        x.Item().Text($"ID Zlecenia: {message.ReportId}");
                    });
            });
        }).GeneratePdf();

        var s3Config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000",
            ForcePathStyle = true
        };

        using var s3Client = new AmazonS3Client("minioadmin", "minioadmin", s3Config);

        using var stream = new MemoryStream(pdfBytes);
        var putRequest = new PutObjectRequest
        {
            BucketName = "reports",
            Key = fileName,
            InputStream = stream,
            ContentType = "application/pdf"
        };

        await s3Client.PutObjectAsync(putRequest, context.CancellationToken);

        var urlRequest = new GetPreSignedUrlRequest
        {
            BucketName = "reports",
            Key = fileName,
            Expires = DateTime.UtcNow.AddHours(24),
            Protocol = Protocol.HTTP
        };

        var fileUrl = s3Client.GetPreSignedURL(urlRequest);

        report.MarkAsCompleted(fileUrl);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.Publish(new ReportCompletedEvent(message.ReportId, fileUrl));
    }
}
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
    private readonly ILogger<ReportRequestedEventConsumer> _logger;

    public ReportRequestedEventConsumer(ApplicationDbContext dbContext, ILogger<ReportRequestedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReportRequestedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Odebrano zlecenie na raport: {ReportName} (ID: {Id})", message.ReportName, message.ReportId);

        var report = await _dbContext.Reports.FindAsync(new object[] { message.ReportId }, context.CancellationToken);
        if (report == null)
        {
            _logger.LogWarning("Nie znaleziono raportu w bazie!");
            return;
        }

        report.MarkAsProcessing();
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Zmieniono status na Processing...");

        _logger.LogInformation("Rozpoczynam fizyczne generowanie pliku PDF...");

        var fileName = $"{message.ReportName}_{message.ReportId}.pdf";
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        Document.Create(container =>
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
                        x.Item().Text("To jest prawdziwy plik wygenerowany w tle przez architekturę Event-Driven!");
                        x.Item().Text($"ID Zlecenia: {message.ReportId}");
                        x.Item().Text($"Wygenerowano dokładnie o: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Strona ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf(filePath);

        _logger.LogInformation("Zapisano plik PDF na dysku w lokalizacji: {FilePath}", filePath);

        report.MarkAsCompleted();
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Sukces! Zakończono przetwarzanie.");
    }
}
using MassTransit;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using ReportGenerator.Infrastructure.Persistence;
using ReportGenerator.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReportRequestedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
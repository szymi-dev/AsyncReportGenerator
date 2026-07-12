using MassTransit;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using ReportGenerator.Infrastructure.Persistence;
using ReportGenerator.Worker.Consumers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "ReportGenerator.Worker")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var rabbitHost = builder.Configuration["RabbitMqHost"] ?? "localhost";

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

        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
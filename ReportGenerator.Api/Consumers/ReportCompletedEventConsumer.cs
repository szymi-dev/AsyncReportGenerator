using MassTransit;
using Microsoft.AspNetCore.SignalR;
using ReportGenerator.Application.Events;
using ReportGenerator.Api.Hubs;

namespace ReportGenerator.Api.Consumers;

public class ReportCompletedEventConsumer : IConsumer<ReportCompletedEvent>
{
    private readonly IHubContext<ReportHub> _hubContext;

    public ReportCompletedEventConsumer(IHubContext<ReportHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<ReportCompletedEvent> context)
    {
        var message = context.Message;

        await _hubContext.Clients.All.SendAsync("ReportCompleted", message.ReportId, message.FileUrl);
    }
}
using MassTransit;
using MediatR;
using ReportGenerator.Application.Events;
using ReportGenerator.Application.Repositories;
using ReportGenerator.Domain.Entities;

namespace ReportGenerator.Application.Commands;

public class RequestReportCommandHandler : IRequestHandler<RequestReportCommand, Guid>
{
    private readonly IReportRepository _reportRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    public RequestReportCommandHandler(IReportRepository reportRepository, IPublishEndpoint publishEndpoint)
    {
        _reportRepository = reportRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(RequestReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report(request.Name);

        await _reportRepository.AddAsync(report, cancellationToken); 
        
        await _publishEndpoint.Publish(new ReportRequestedEvent(report.Id, report.Name), cancellationToken);

        return report.Id;
    }
}

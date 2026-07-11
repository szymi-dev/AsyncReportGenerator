using MediatR;
using ReportGenerator.Application.Repositories;

namespace ReportGenerator.Application.Queries;

public class GetReportStatusQueryHandler : IRequestHandler<GetReportStatusQuery, ReportStatusDto>
{
    private readonly IReportRepository _reportRepository;

    public GetReportStatusQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportStatusDto> Handle(GetReportStatusQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetAsync(request.ReportId);
    }
}
using ReportGenerator.Application.Queries;
using ReportGenerator.Domain.Entities;

namespace ReportGenerator.Application.Repositories;

public interface IReportRepository
{
    Task AddAsync(Report report, CancellationToken cancellationToken);
    Task<ReportStatusDto> GetAsync(Guid Id);
}

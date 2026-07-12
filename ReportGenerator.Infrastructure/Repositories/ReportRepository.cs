using ReportGenerator.Application.Queries;
using ReportGenerator.Application.Repositories;
using ReportGenerator.Domain.Entities;
using ReportGenerator.Infrastructure.Persistence;

namespace ReportGenerator.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReportRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken)
    {
        await _dbContext.Reports.AddAsync(report, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReportStatusDto?> GetAsync(Guid Id)
    {
        var report = await _dbContext.FindAsync<Report>(Id);

        if (report == null) return null;

        return new ReportStatusDto(report.Id, report.Name, report.Status.ToString(), report.FileUrl);
    }
}
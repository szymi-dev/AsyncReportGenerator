using MediatR;

namespace ReportGenerator.Application.Queries;

public record GetReportStatusQuery(Guid ReportId) : IRequest<ReportStatusDto>;

public record ReportStatusDto(Guid Id, string Name, string Status);
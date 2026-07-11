using MediatR;

namespace ReportGenerator.Application.Commands;

public record RequestReportCommand(string Name) : IRequest<Guid>;

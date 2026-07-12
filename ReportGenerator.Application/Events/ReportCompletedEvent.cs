namespace ReportGenerator.Application.Events;

public record ReportCompletedEvent(Guid ReportId, string FileUrl);
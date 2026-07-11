namespace ReportGenerator.Application.Events;

public record ReportRequestedEvent(Guid ReportId, string ReportName);
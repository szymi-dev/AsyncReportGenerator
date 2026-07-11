using ReportGenerator.Domain.Enums;

namespace ReportGenerator.Domain.Entities;

public class Report
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ReportStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string? FileUrl { get; private set; }

    private Report() { }

    public Report(string name) 
    { 
        if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("Report name cannot be null or empty.", nameof(name));

        Id = Guid.NewGuid();
        Status = ReportStatus.Pending;
        Name = name;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsPending() 
    { 
        Status = ReportStatus.Pending;
    }

    public void MarkAsProcessing()
    {
        Status = ReportStatus.Processing;
    }

    public void MarkAsCompleted(string fileUrl)
    {
        Status = ReportStatus.Completed;
        FileUrl = fileUrl;
    }

    public void MarkAsFailed()
    {
        Status = ReportStatus.Failed;
    }
}

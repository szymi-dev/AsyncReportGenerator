using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportGenerator.Application.Commands;
using ReportGenerator.Application.Queries;

namespace ReportGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RequestReport([FromBody] RequestReportCommand command)
    {
        var reportId = await _mediator.Send(command);

        return Accepted(new { ReportId = reportId, Message = "Zlecenie przyjęte do przetwarzania." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var report = await _mediator.Send(new GetReportStatusQuery(id));
        if (report == null) return NotFound();

        return Ok(report);
    }
}
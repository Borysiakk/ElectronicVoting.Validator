
using ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;
using ElectronicVoting.Validator.Domain.Models.Blockchain;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicVoting.Validator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElectionConsensusController: BaseController
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    
    public ElectionConsensusController(IMediator mediator, IBackgroundJobClient backgroundJobClient) : base(mediator)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost("pre-prepare-block")]
    public IActionResult PrePrepareBlock([FromBody] PendingBlockDto pendingBlockDto, CancellationToken ct)
    {
        var request = new PrePrepareBlock()
        {
            PendingBlockDto = pendingBlockDto,
        };
        
        _backgroundJobClient.Enqueue<IMediator>(a => a.Send(request, ct));
        
        return Ok();
    }
    
    [HttpPost("prepare-block")]
    public IActionResult PrepareBlock([FromBody] PrepareBlock request, CancellationToken ct)
    {
        _backgroundJobClient.Enqueue<IMediator>(a => a.Send(request, ct));
        return Ok();
    }
}
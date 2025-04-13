using ElectronicVoting.Validator.Application.Handlers.Commands.Api.Election;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicVoting.Validator.API.Controllers;

public class ElectionController: BaseController
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public ElectionController(IMediator mediator, IBackgroundJobClient backgroundJobClient) : base(mediator)
    {
        _backgroundJobClient = backgroundJobClient;
    }
    
    [HttpPost("cast-vote")]
    public IActionResult CastVote([FromBody] CastVote command, CancellationToken ct)
    {
        _backgroundJobClient.Enqueue<IMediator>(a => a.Send(command, ct));
        return Ok();
    }
}
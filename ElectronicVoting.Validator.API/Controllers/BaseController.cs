using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicVoting.Validator.API.Controllers;

public class BaseController: ControllerBase
{
    protected readonly IMediator Mediator;
    public BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }
}
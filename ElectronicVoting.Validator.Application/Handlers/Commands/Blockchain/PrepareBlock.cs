using MediatR;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;

public record PrepareBlock(long PendingBlockId) : IRequest
{
    public long PendingBlockId { get; set; } = PendingBlockId;
}

public class PrepareBlockHandler: IRequestHandler<PrepareBlock>
{
    public Task Handle(PrepareBlock request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
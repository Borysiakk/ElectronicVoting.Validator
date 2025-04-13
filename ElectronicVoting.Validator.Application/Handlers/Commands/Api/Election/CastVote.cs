
using ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;
using ElectronicVoting.Validator.Application.Service;
using ElectronicVoting.Validator.Domain.Models;
using ElectronicVoting.Validator.Infrastructure.MediatR;
using ElectronicVoting.Validator.Infrastructure.Repository;
using Hangfire;
using MediatR;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Api.Election;

public record CastVote: IRequest
{
    public VoteEncryption VoteEncryption { get; set; }
}

public class CastVoteHandler :IRequestHandler<CastVote>
{

    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IVoteValidationService _voteValidationService;
    private readonly IPendingTransactionRepository _pendingTransactionRepository;

    public CastVoteHandler(IVoteValidationService voteValidationService, IPendingTransactionRepository pendingTransactionRepository, IBackgroundJobClient backgroundJobClient)
    {
        _voteValidationService = voteValidationService;
        _pendingTransactionRepository = pendingTransactionRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task Handle(CastVote request, CancellationToken cancellationToken)
    {
        var resultValidateVote = await _voteValidationService.ValidateVoteAsync(request.VoteEncryption);

         var cratePendingTransaction = new CratePendingTransaction()
         {
             //Data = "Dane testowe",
         };
        
        _backgroundJobClient.Enqueue<IMediator>(a => a.Send(cratePendingTransaction, cancellationToken));
    }

    private async Task<bool> ValidateVoteAsync(VoteEncryption voteEncryption, CancellationToken cancellationToken = default)
    {
        return await _voteValidationService.ValidateVoteAsync(voteEncryption);
    }
    
}
using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using FluentResults;

namespace ElectronicVoting.Validator.Application.Factories;

public static class ReceiveLocalBlockValidationCommandFactory
{
    public static ReceiveLocalBlockValidationCommand CreateSuccessfulCommand(Result<PendingBlockDetailsDto> reconstructPendingBlock, bool isValidHash, Guid currentValidatorId)
    {
        return new ReceiveLocalBlockValidationCommand
        {
            IsValid = isValidHash,
            Hash = reconstructPendingBlock.Value.Hash,
            PendingBlockId = reconstructPendingBlock.Value.Id,
            SignedByValidatorId = currentValidatorId,
        };
    }

    public static ReceiveLocalBlockValidationCommand CreateFailedCommand(Result<PendingBlockDetailsDto> originalPendingBlockDetailsResult, Result<PendingBlockDetailsDto> reconstructPendingBlock, Guid currentValidatorId)
    {
        var rejectionReason = reconstructPendingBlock.Errors.Select(error => error.Message).ToList();
        return new ReceiveLocalBlockValidationCommand
        {
            IsValid = false,
            Hash = string.Empty,
            RejectionReason = rejectionReason,
            PendingBlockId = originalPendingBlockDetailsResult.Value.Id,
            SignedByValidatorId = currentValidatorId
        };
    }

}
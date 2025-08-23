

using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using FluentResults;

namespace ElectronicVoting.Validator.Application.Factories;

public interface IBlockValidationResultFactory
{
    public BlockValidationResultEntity CreateLeaderValidationResultAsync(PendingBlockDetailsDto pendingBlockDetailsDto, Guid leaderValidatorId);
}

public static class BlockValidationResultFactory
{
    public static BlockValidationResultEntity CreateForLeader (PendingBlockDetailsDto pendingBlockDetailsDto, Guid leaderValidatorId)
    {
        return new BlockValidationResultEntity()
        {
            Id = Guid.NewGuid(),
            IsValid = true,
            IsLeaderValidation = true,
            ValidatorId = leaderValidatorId,
            Hash = pendingBlockDetailsDto.Hash,
            PendingBlockEntityId = pendingBlockDetailsDto.Id,
        };
    }

    public static BlockValidationResultEntity CreateForValidator(ReceiveLocalBlockValidationCommand command, Guid validatorId)
    {
        return new BlockValidationResultEntity()
        {
            Id = Guid.NewGuid(),
            IsValid = command.IsValid,
            ValidatorId = validatorId,
            Hash = command.Hash,
            PendingBlockEntityId = command.PendingBlockId,
            RejectionReason = command.RejectionReason,
        };
    }
}
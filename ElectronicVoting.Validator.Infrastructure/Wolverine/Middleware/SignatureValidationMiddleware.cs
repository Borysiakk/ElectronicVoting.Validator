using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace ElectronicVoting.Validator.Infrastructure.Wolverine.Middleware;

public class SignatureValidationMiddleware(ISignatureService signatureService, IValidatorNodeRepository validatorNodeRepository, ILogger<SignatureValidationMiddleware> logger)
{
    public async Task<(HandlerContinuation, bool isValid)> BeforeAsync(ISignedCommand command, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrEmpty(command.Signature))
            {
                logger.LogError("Walidacja podpisu nie mozliwa z podowodu braku podpisu");
                return (HandlerContinuation.Stop, false);           
            }
            
            var signedByValidator = await validatorNodeRepository.GetByIdAsync(command.SignedByValidatorId, ct);
            if (signedByValidator is null)
            {
                logger.LogError("Walidacja podpisu nie mozliwa z podowodu braku klucza publicznego w bazie danych");
                return (HandlerContinuation.Stop, false);
            }
            
            var publicKey = CertificateHelper.GetCertificateFromString(signedByValidator.PublicKey);
            var verify = signatureService.Verify(command, publicKey, command.Signature);
            return (HandlerContinuation.Continue, verify);
        }
        catch (Exception e)
        {
            logger.LogError("Walidacja podpisu nie mozliwa z podowodu bledu");
            logger.LogError(e.Message);
            return (HandlerContinuation.Stop, false);
        }

    }

}
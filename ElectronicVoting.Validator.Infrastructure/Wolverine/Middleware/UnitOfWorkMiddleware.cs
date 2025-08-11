using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Wolverine;

namespace ElectronicVoting.Validator.Infrastructure.Wolverine.Middleware;

public class UnitOfWorkMiddleware(IUnitOfWork unitOfWork)
{
    public async Task<HandlerContinuation> BeforeAsync(ITransaction transaction, CancellationToken ct)
    {
        await unitOfWork.BeginTransaction(ct);
        return HandlerContinuation.Continue;
    }
    
    public async Task<HandlerContinuation> AfterAsync(ITransaction transaction, CancellationToken ct)
    {
        await unitOfWork.Commit(ct);
        return HandlerContinuation.Continue;
    }
    
    public async Task<HandlerContinuation> OnExceptionAsync(ITransaction transaction, Exception ex)
    {
        await unitOfWork.Rollback(CancellationToken.None);
        return HandlerContinuation.Stop;       
    }
}
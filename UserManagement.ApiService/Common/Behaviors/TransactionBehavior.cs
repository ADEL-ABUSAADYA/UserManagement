using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data;

namespace UserManagement.ApiService.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICommand
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly Context _context;

    public TransactionBehavior(
        ILogger<TransactionBehavior<TRequest, TResponse>> logger,
        Context context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        
        IDbContextTransaction transaction = default;

        try
        {
            
            // await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:15';", cancellationToken);
            //
            // _logger.LogInformation("Transaction before execute");

            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            

            var response = await next();
            if (response is RequestResult<bool> result && !result.isSuccess)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _logger.LogInformation("Handler returned failure, transaction rolled back.");
                return response;
            }
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"save changes {DateOnly.FromDateTime(DateTime.Now)}");
            await transaction.CommitAsync(cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync(CancellationToken.None);
            }

            throw;
        }
    }

}
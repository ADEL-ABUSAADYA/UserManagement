using Microsoft.EntityFrameworkCore.Storage;
using UserManagement.ApiService.Data;

namespace UserManagement.ApiService.Middlewares;

public class TransactionMiddleware
{
    RequestDelegate _nextAction;
    Context _context;
    public TransactionMiddleware(RequestDelegate nextAction, Context context)
    {
        _nextAction = nextAction;
        _context = context;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        IDbContextTransaction transaction = default;

        try
        {
            transaction = _context.Database.BeginTransaction();

            await _nextAction(context);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            throw;
        }
    }
}
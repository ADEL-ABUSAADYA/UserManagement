using UserManagement.ApiService.Common;

public class TimeOutMiddleware : IMiddleware
{
    private readonly CancellationTokenProvider _cancellationTokenProvider;

    public TimeOutMiddleware(CancellationTokenProvider provider)
    {
       _cancellationTokenProvider = provider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, context.RequestAborted);

        context.Items["CancellationToken"] = linkedCts.Token;
        _cancellationTokenProvider.CancellationToken = linkedCts.Token;

        Task requestTask = next(context);
        Task delayTask = Task.Delay(TimeSpan.FromSeconds(10), linkedCts.Token);

        var finishedTask = await Task.WhenAny(requestTask, delayTask);

        if (finishedTask == delayTask)
        {
            timeoutCts.Cancel();
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsync("Request timed out.");
            }
        }
    }
}

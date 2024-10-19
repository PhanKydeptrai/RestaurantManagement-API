using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Shared;
using Serilog.Context;

namespace RestaurantManagement.API.Behavior;

public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        _logger.LogInformation("Processing request {Name}", requestName);
        TResponse result = await next();
        if (result.IsSuccess)
        {
            _logger.LogInformation("Request {Name} processed successfully", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Errors, true))
            {
                _logger.LogError("Request {Name} processed with error", requestName);

            }
        }

        return result;
    }
}
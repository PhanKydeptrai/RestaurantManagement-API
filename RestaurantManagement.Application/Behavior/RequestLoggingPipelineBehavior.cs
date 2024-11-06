using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Shared;
using Serilog.Context;

namespace RestaurantManagement.API.Behavior;

public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        logger.LogInformation("Processing request {Name}", requestName);
        TResponse result = await next();
        if (result.IsSuccess)
        {
            logger.LogInformation("Request {Name} processed successfully", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Errors, true))
            {
                logger.LogError("Request {Name} processed with error", requestName);

            }
        }

        return result;
    }
}
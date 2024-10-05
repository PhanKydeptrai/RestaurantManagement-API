using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Behaviors;


public class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultForLog
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);
        
        var result = await next();
        
        _logger.LogInformation("Completed Request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

        return result;
    }
}

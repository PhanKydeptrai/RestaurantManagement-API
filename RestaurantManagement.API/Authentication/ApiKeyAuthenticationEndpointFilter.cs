namespace RestaurantManagement.API.Authentication;

internal class ApiKeyAuthenticationEndpointFilter : IEndpointFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private readonly IConfiguration _configuration  ;

    public ApiKeyAuthenticationEndpointFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];
        if(!IsApiKeyValid(apiKey))
        {
            return Results.Unauthorized();
        }
        return await next(context);
    }

    private bool IsApiKeyValid(string? apiKey)
    {
        if(string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }
        string? actualApiKey = _configuration.GetValue<string>("ApiKey")!;
        // string? actualApiKey = _configuration.GetValue<string>("ApiKey")!;
        // string? hehe = _configuration["ApiKey"];
        return apiKey == actualApiKey;
    }
}

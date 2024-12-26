using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.API.Middleware;
using RestaurantManagement.Application;
using RestaurantManagement.Infrastructure;
using RestaurantManagement.Infrastructure.Extentions;
using Serilog;
using System.Reflection;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add infrastructure extentions to get connection string
builder.Services.AddInfrastructureExtentions(builder.Configuration)
                .AddInfrastructure()
                .AddApplication(builder.Configuration)
                .AddRateLimiter();
//Add endpoints
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddExceptionHandler<GlobalExceptionHandling>();
builder.Services.AddProblemDetails();

//Add serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

//Add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq();
});




//Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("boss", policy => policy.RequireClaim(ClaimTypes.Role, "Boss"));
    options.AddPolicy("customer", policy => policy.RequireClaim(ClaimTypes.Role, "Subscriber"));
    options.AddPolicy("management", policy => policy.RequireClaim(ClaimTypes.Role, "Manager", "Boss"));

});

//JWT
#region Cấu hình JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"])),
        ValidateLifetime = true, // Kiểm tra thời gian hết hạn của token
        ClockSkew = TimeSpan.Zero, // Loại bỏ thời gian trễ mặc định
                                   // Đảm bảo token chứa claim về vai trò
        RoleClaimType = ClaimTypes.Role
    };

})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["ClientId"]!;
    googleOptions.ClientSecret = builder.Configuration["ClientSecret"]!;
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["AppId"]!;
    facebookOptions.AppSecret = builder.Configuration["AppSecret"]!;
});


// Cấu hình cho swaggergen
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "RestaurantManagement-API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });

    //Cấu hình swagger sử dụng ap key
    option.AddSecurityDefinition("X-Api-Key", new OpenApiSecurityScheme()
    {
        Description = "API Key",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    var scheme = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference()
        {
            Type = ReferenceType.SecurityScheme,
            Id = "X-Api-Key"

        },
        In = ParameterLocation.Header
    };

    var requirement = new OpenApiSecurityRequirement()
    {
        { scheme, new string[] { } }
    };

    option.AddSecurityRequirement(requirement);


    option.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd"))
    });

    option.MapType<TimeOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "time",
        Example = new OpenApiString(DateTime.Now.ToString("HH:mm:ss"))
    });
});
#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseRateLimiter();
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

// Map endpoints
app.MapEndpoints();
app.Run();

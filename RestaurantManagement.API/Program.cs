using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.API.Middleware;
using RestaurantManagement.Application;
using RestaurantManagement.Infrastructure;
using RestaurantManagement.Infrastructure.Extentions;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add infrastructure extentions to get connection string
builder.Services.AddInfrastructureExtentions(builder.Configuration)
                .AddInfrastructure()
                .AddApplication(builder.Configuration);
//Add endpoints
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

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

//Add rate limiter  
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("ResetPass", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromHours(1)
        }));

    options.AddPolicy("AntiSpam", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));
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
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),
        ValidateLifetime = true, // Kiểm tra thời gian hết hạn của token
        ClockSkew = TimeSpan.Zero, // Loại bỏ thời gian trễ mặc định
                                   // Đảm bảo token chứa claim về vai trò
        RoleClaimType = ClaimTypes.Role
    };

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
    //Xác định tên dựa theo 
    var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //Tạo địa chỉ file
    var filePath = Path.Combine(AppContext.BaseDirectory, fileName); //AppContext.BaseDirectory Lấy địa chỉ thư mục gốc
    option.IncludeXmlComments(filePath);

});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<RequestLogContextMiddleware>();

app.UseRateLimiter();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// Map endpoints
app.MapEndpoints();
app.Run();

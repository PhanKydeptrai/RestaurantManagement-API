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

// builder.Services.AddExceptionHandler<GlobalExceptionHandling>();
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

#region Rate limiter
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

    options.AddPolicy("AntiSpamRegister", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamLogin", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamEmplyeeLogin", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    // options.AddPolicy("AntiSpamEmplyeeLogin", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    //     partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    //     factory: _ => new FixedWindowRateLimiterOptions
    //     {
    //         PermitLimit = 1,
    //         Window = TimeSpan.FromSeconds(2)
    //     }));

    options.AddPolicy("AntiSpamCustomerResetPass", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamEmployeeResetPass", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamChangePassword", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    //Khách hàng tự huỷ tài khoản 
    options.AddPolicy("AntiSpamDeActiveCustomerAccount", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamCustomerCreateBooking", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamSubscriberCreateBooking", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamTableArrange", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamCancelBooking", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamCreateCategoryCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamUpdateCategoryCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 1,
            Window = TimeSpan.FromSeconds(2)
        }));

    options.AddPolicy("AntiSpamRemoveManyCategoryCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreCategoryCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreManyCategoryCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamCreateCustomerCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamDeleteCustomerCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUpdateEmployeeInformationCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamCreateEmployeeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamDeleteEmployeeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreEmployeeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUpdateEmployeeRoleCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamCreateMealCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUpdateMealCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRemoveMealCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreMealCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamChangeSellStatusCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreSellStatusCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamAddMealToOrderCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUpdateMealInOrderCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamDeleteMealFromOrderCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamPayOrderCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamCreateTableCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamDeleteTableCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreTableCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamAssignTableToCustomerCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUnAssignTableToCustomerCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamAssignTableToBookedCustomerCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUnAssignTableToBookedCustomerCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamCreateTableTypeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUpdateTableTypeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamDeleteTableTypeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamRestoreTableTypeCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamCreateVoucherCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamUpdateVoucherCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));

    options.AddPolicy("AntiSpamDeleteVoucherCommand", httpContext => RateLimitPartition.GetFixedWindowLimiter(
    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
    factory: _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 1,
        Window = TimeSpan.FromSeconds(2)
    }));


});


#endregion 


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

})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
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

    // //Xác định tên dựa theo 
    // var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // //Tạo địa chỉ file
    // var filePath = Path.Combine(AppContext.BaseDirectory, fileName); //AppContext.BaseDirectory Lấy địa chỉ thư mục gốc
    // Console.WriteLine(filePath);
    // option.IncludeXmlComments(filePath);

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


app.UseExceptionHandler();

// Map endpoints
app.MapEndpoints();
app.Run();

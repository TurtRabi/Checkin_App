using System.Text.Json;
using API_UsePrevention.Extensions;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using StackExchange.Redis;
using Service.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//config db
builder.Services.AddDbContext<TravelCardsDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));



//Register repository;;
builder.Services.AddRepositoryServices();

//Register service;
builder.Services.AddAppServices();

// Register Redis
var redisConnectionString = builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis:ConnectionString is not configured.");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<IRedisService, RedisService>();

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//config JWT
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("Admin"));
});


var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    response.ContentType = "application/json";

    var result = response.StatusCode switch
    {
        401 => JsonSerializer.Serialize(new { errorCode = "HB40101", message = "Token missing or invalid" }),
        403 => JsonSerializer.Serialize(new { errorCode = "HB40301", message = "Permission denied" }),
        404 => JsonSerializer.Serialize(new { errorCode = "HB40401", message = "Resource not found" }),
        _ => null
    };

    if (result != null)
        await response.WriteAsync(result);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
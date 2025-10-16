using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Endpoints;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;
using MinimalAPI.Validators;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("InMemoryDb");
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    // Add response headers to help clients understand rate limiting
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        
        // Create a proper API response consistent with your application
        var response = ApiResponse<object>.Fail(
            "Too many requests",
            new List<string> { "API rate limit exceeded. Please try again later." }
        );
        
        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
});

// Register services and repositories
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IFundService, FundService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IFundRepository, FundRepository>();
builder.Services.AddScoped<ContactEndpointHandlers>();
builder.Services.AddScoped<FundEndpointHandlers>();
builder.Services.AddScoped<IValidator<CreateContactRequest>, CreateContactValidator>();
builder.Services.AddScoped<IValidator<UpdateContactRequest>, UpdateContactValidator>();
builder.Services.AddScoped<IValidator<AddContactToFundRequest>, AddContactToFundValidator>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    await ApplicationDbContext.SeedDataAsync(dbContext);
}

app.UseHttpsRedirection();

// Enable rate limiting globally
app.UseRateLimiter();

// Map enpoints from separate class
app.MapContactEndpoints();
app.MapFundEndpoints();


// Global exception handling
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;

        if (exception is BadHttpRequestException badRequestException &&
            badRequestException.InnerException is JsonException jsonException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = ApiResponse<object>.Fail(
                "Invalid JSON format",
                new List<string> { $"The request contains malformed JSON: {jsonException.Message}" }
            );
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response = ApiResponse<object>.Error(
                "An error occurred while processing your request",
                new List<string> { exception?.Message ?? "Unknown error" }
            );
        }

        await context.Response.WriteAsJsonAsync(response);
    });
});

app.Run();


using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notification.Extensions;
using Notification.Features.DeliveryReceipts;
using Notification.Features.EmailMessages;
using Notification.Features.EmailService;
using Notification.Features.Messages;
using Notification.Features.SmsService;
using Notification.Features.TextMessages;
using Notification.Infrastructure.Filters;
using Notification.Infrastructure.Persistence;
using Notification.Middleware;
using Notification.Shared.Responses;

var builder = WebApplication.CreateBuilder(args);

EnsureProductionSettings(builder);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSmsModule();
builder.Services.AddEmailModule();
builder.Services.AddMessageModule();
builder.Services.AddTextMessageModule();
builder.Services.AddEmailMessageModule();
builder.Services.AddDeliveryReceiptModule();
builder.Services.AddScoped<WebhookAccessFilter>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        return new BadRequestObjectResult(
            ApiResult<object>.Fail("Validation failed.", "ERROR", StatusCodes.Status400BadRequest, errors));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await database.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();

static void EnsureProductionSettings(WebApplicationBuilder builder)
{
    if (builder.Environment.IsDevelopment())
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(builder.Configuration["Webhooks:Secret"]))
    {
        throw new InvalidOperationException("Webhooks:Secret is required outside Development.");
    }
}

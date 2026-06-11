using Cart.API.Database;
using Cart.API.ExceptionHandlers;
using Cart.API.Repositories;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddExceptionHandler<CartNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<CartProductNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<CartStockInsuficienteExceptionHandler>();
builder.Services.AddExceptionHandler<CartCantidadInvalidaExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => e.Value!.Errors.First().ErrorMessage)
            .ToList();

        var result = new ObjectResult(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = "Los datos enviados son inválidos.",
            Instance = context.HttpContext.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                ["errorCode"] = "CRT-004",
                ["errorMessage"] = "Los datos enviados tienen un formato inválido."
            }
        })
        {
            StatusCode = 400
        };

        return result;
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().Initialize();

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();
app.MapControllers();
app.Run();
using Products.API.Models;
using Products.API.Services;
using Products.API.DTOs;
using Products.API.Exceptions;
using Products.API.ExceptionHandlers;
using ECommerce.Shared.Observability;



var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging("Products.API");

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ProductService>();

builder.Services.AddExceptionHandler<ProductExceptionHandler>();

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.UseAppRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/products", (string? categoria, string? nombre, ProductService productService) =>
{
    return Results.Ok(productService.GetFiltered(categoria, nombre));
});

app.MapGet("/api/products/{id}", (Guid id, ProductService productService) =>
{
    Product? product = productService.GetById(id);

    if (product is null)
    {
        throw new NotFoundException(
            "PRD-001",
            "Producto no encontrado.");
    }

    return Results.Ok(product);
});

app.MapPost("/api/products", (CreateProductRequest request, ProductService productService) =>
{
    Product product = new Product
    {
        Nombre = request.Nombre,
        Descripcion = request.Descripcion,
        Precio = request.Precio,
        Stock = request.Stock,
        Categoria = request.Categoria
    };

    Product createdProduct = productService.Create(product);

    return Results.Created($"/api/products/{createdProduct.Id}", createdProduct);
});

app.MapPut("/api/products/{id}", (Guid id, UpdateProductRequest request, ProductService productService) =>
{
    Product product = new Product
    {
        Nombre = request.Nombre,
        Descripcion = request.Descripcion,
        Precio = request.Precio,
        Stock = request.Stock,
        Categoria = request.Categoria
    };

    Product? updatedProduct = productService.Update(id, product);

    if (updatedProduct is null)
    {
        throw new NotFoundException(
            "PRD-001",
            "Producto no encontrado.");
    }

    return Results.Ok(updatedProduct);
});

app.MapDelete("/api/products/{id}", (Guid id, ProductService productService) =>
{
    bool deleted = productService.Delete(id);

    if (!deleted)
    {
        throw new NotFoundException(
            "PRD-001",
            "Producto no encontrado.");
    }

    return Results.NoContent();
});

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");


app.Run();
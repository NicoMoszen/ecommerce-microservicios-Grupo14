using Products.API.Models;
using Products.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/products", (ProductService productService) =>
{
    return Results.Ok(productService.GetAll());
});

app.MapGet("/api/products/{id}", (Guid id, ProductService productService) =>
{
    Product? product = productService.GetById(id);

    if (product is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(product);
});

app.MapPost("/api/products", (Product product, ProductService productService) =>
{
    Product createdProduct = productService.Create(product);

    return Results.Created($"/api/products/{createdProduct.Id}", createdProduct);
});

app.MapPut("/api/products/{id}", (Guid id, Product product, ProductService productService) =>
{
    Product? updatedProduct = productService.Update(id, product);

    if (updatedProduct is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updatedProduct);
});

app.MapDelete("/api/products/{id}", (Guid id, ProductService productService) =>
{
    bool deleted = productService.Delete(id);

    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run();
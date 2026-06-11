using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Exceptions;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Product>), 200)]
        [ProducesResponseType(500)]
        public IActionResult GetAll([FromQuery] string? categoria, [FromQuery] string? nombre)
        {
            List<Product> products = _productService.GetFiltered(categoria, nombre);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetById(Guid id)
        {
            Product? product = _productService.GetById(id);

            if (product is null)
            {
                throw new NotFoundException(
                    "PRD-001",
                    "Producto no encontrado.");
            }

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult Create([FromBody] CreateProductRequest request)
        {
            Product product = new Product
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                Categoria = request.Categoria
            };

            Product createdProduct = _productService.Create(product);

            return Created($"/api/products/{createdProduct.Id}", createdProduct);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            Product product = new Product
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                Categoria = request.Categoria
            };

            Product? updatedProduct = _productService.Update(id, product);

            if (updatedProduct is null)
            {
                throw new NotFoundException(
                    "PRD-001",
                    "Producto no encontrado.");
            }

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult Delete(Guid id)
        {
            bool deleted = _productService.Delete(id);

            if (!deleted)
            {
                throw new NotFoundException(
                    "PRD-001",
                    "Producto no encontrado.");
            }

            return NoContent();
        }
    }
}
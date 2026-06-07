using Products.API.Models;

namespace Products.API.Services
{
    public class ProductService
    {
        private readonly List<Product> _products = new();

        public List<Product> GetAll()
        {
            return _products;
        }

        public Product? GetById(Guid id)
        {
            return _products.FirstOrDefault(product => product.Id == id);
        }

        public Product Create(Product product)
        {
            if (IsInvalidProductData(product.Nombre, product.Descripcion, product.Precio, product.Stock, product.Categoria))
            {
                throw new ArgumentException("PRD-002");
            }

            product.Id = Guid.NewGuid();
            product.FechaCreacion = DateTime.UtcNow;

            _products.Add(product);

            return product;
        }

        public Product? Update(Guid id, Product updatedProduct)
        {
            if (IsInvalidProductData(
                updatedProduct.Nombre,
                updatedProduct.Descripcion,
                updatedProduct.Precio,
                updatedProduct.Stock,
                updatedProduct.Categoria))
            {
                throw new ArgumentException("PRD-002");
            }

            Product? existingProduct = GetById(id);

            if (existingProduct is null)
            {
                return null;
            }

            existingProduct.Nombre = updatedProduct.Nombre;
            existingProduct.Descripcion = updatedProduct.Descripcion;
            existingProduct.Precio = updatedProduct.Precio;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.Categoria = updatedProduct.Categoria;

            return existingProduct;
        }

        public bool Delete(Guid id)
        {
            Product? existingProduct = GetById(id);

            if (existingProduct is null)
            {
                return false;
            }

            _products.Remove(existingProduct);

            return true;
        }

        private static bool IsInvalidProductData(string nombre, string? descripcion, decimal precio, int stock, string categoria)
        {
            return string.IsNullOrWhiteSpace(nombre)
                || nombre.Length > 100
                || descripcion?.Length > 500
                || precio <= 0
                || stock < 0
                || string.IsNullOrWhiteSpace(categoria);
        }
    }
}
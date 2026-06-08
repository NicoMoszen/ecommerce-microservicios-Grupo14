using Products.API.Models;
using Products.API.Exceptions;


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
                throw new ProductException(
                 "PRD-002",
                 "Los datos del producto son inválidos.",
                 400);
            }

            bool duplicatedProduct = _products.Any(existingProduct =>
                existingProduct.Nombre.Equals(product.Nombre, StringComparison.OrdinalIgnoreCase)
                && existingProduct.Categoria.Equals(product.Categoria, StringComparison.OrdinalIgnoreCase));

            if (duplicatedProduct)
            {
                throw new ProductException(
                    "PRD-003",
                    "Ya existe un producto con ese nombre en la categoría.",
                    409);
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
                throw new ProductException(
                 "PRD-002",
                 "Los datos del producto son inválidos.",
                 400);
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

        public List<Product> GetFiltered(string? categoria, string? nombre)
        {
            IEnumerable<Product> query = _products;

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(product =>
                    product.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(product =>
                    product.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            }

            return query.ToList();
        }
    }
}
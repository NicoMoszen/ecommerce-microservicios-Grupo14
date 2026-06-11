using Products.API.Models;
using Products.API.Exceptions;
using Products.API.Data;


namespace Products.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product? GetById(Guid id)
        {
            return _productRepository.GetById(id);
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

            Product? existingProduct = _productRepository.GetByNameAndCategory(product.Nombre, product.Categoria);

            bool duplicatedProduct = existingProduct is not null;

            if (duplicatedProduct)
            {
                throw new ProductException(
                    "PRD-003",
                    "Ya existe un producto con ese nombre en la categoría.",
                    409);
            }

            product.Id = Guid.NewGuid();
            product.FechaCreacion = DateTime.UtcNow;

            return _productRepository.Create(product);
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

            return _productRepository.Update(id, updatedProduct);
        }

        public bool Delete(Guid id)
        {
            return _productRepository.Delete(id);
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
            IEnumerable<Product> query = _productRepository.GetAll();

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
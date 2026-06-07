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
            product.Id = Guid.NewGuid();
            product.FechaCreacion = DateTime.UtcNow;

            _products.Add(product);

            return product;
        }

        public Product? Update(Guid id, Product updatedProduct)
        {
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
    }
}
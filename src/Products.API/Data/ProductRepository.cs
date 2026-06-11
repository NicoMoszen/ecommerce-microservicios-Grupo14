using Dapper;
using Microsoft.Data.Sqlite;
using Products.API.Models;

namespace Products.API.Data
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=products.db";
        }

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public List<Product> GetAll()
        {
            using var connection = CreateConnection();

            var result = connection.Query<dynamic>(@"
                SELECT id, nombre, descripcion, precio, stock, categoria, fecha_creacion
                FROM productos
            ");

            return result.Select(MapToProduct).ToList();
        }

        public Product? GetById(Guid id)
        {
            using var connection = CreateConnection();

            var result = connection.QueryFirstOrDefault<dynamic>(@"
                SELECT id, nombre, descripcion, precio, stock, categoria, fecha_creacion
                FROM productos
                WHERE id = @Id
            ", new { Id = id.ToString() });

            return result is null ? null : MapToProduct(result);
        }

        public Product? GetByNameAndCategory(string nombre, string categoria)
        {
            using var connection = CreateConnection();

            var result = connection.QueryFirstOrDefault<dynamic>(@"
                SELECT id, nombre, descripcion, precio, stock, categoria, fecha_creacion
                FROM productos
                WHERE LOWER(nombre) = LOWER(@Nombre)
                  AND LOWER(categoria) = LOWER(@Categoria)
            ", new { Nombre = nombre, Categoria = categoria });

            return result is null ? null : MapToProduct(result);
        }

        public Product Create(Product product)
        {
            using var connection = CreateConnection();

            connection.Execute(@"
                INSERT INTO productos (id, nombre, descripcion, precio, stock, categoria, fecha_creacion)
                VALUES (@Id, @Nombre, @Descripcion, @Precio, @Stock, @Categoria, @FechaCreacion)
            ", new
            {
                Id = product.Id.ToString(),
                product.Nombre,
                product.Descripcion,
                product.Precio,
                product.Stock,
                product.Categoria,
                FechaCreacion = product.FechaCreacion.ToString("o")
            });

            return product;
        }

        public Product? Update(Guid id, Product product)
        {
            using var connection = CreateConnection();

            int affectedRows = connection.Execute(@"
                UPDATE productos
                SET nombre = @Nombre,
                    descripcion = @Descripcion,
                    precio = @Precio,
                    stock = @Stock,
                    categoria = @Categoria
                WHERE id = @Id
            ", new
            {
                Id = id.ToString(),
                product.Nombre,
                product.Descripcion,
                product.Precio,
                product.Stock,
                product.Categoria
            });

            return affectedRows == 0 ? null : GetById(id);
        }

        public bool Delete(Guid id)
        {
            using var connection = CreateConnection();

            int affectedRows = connection.Execute(@"
                DELETE FROM productos
                WHERE id = @Id
            ", new { Id = id.ToString() });

            return affectedRows > 0;
        }

        private static Product MapToProduct(dynamic result)
        {
            return new Product
            {
                Id = Guid.Parse((string)result.id),
                Nombre = (string)result.nombre,
                Descripcion = result.descripcion as string,
                Precio = Convert.ToDecimal(result.precio),
                Stock = (int)(long)result.stock,
                Categoria = (string)result.categoria,
                FechaCreacion = DateTime.Parse((string)result.fecha_creacion)
            };
        }
    }
}
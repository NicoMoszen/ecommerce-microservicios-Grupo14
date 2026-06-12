using Orders.API.Clients;
using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Models;
using Orders.API.Repositories;

namespace Orders.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IUsersApiClient _usersApi;
    private readonly IProductsApiClient _productsApi;


    private static readonly Dictionary<string, string[]> TransicionesValidas = new()
    {
        ["Pendiente"] = ["Confirmada", "Cancelada"],
        ["Confirmada"] = ["Enviada", "Cancelada"],
        ["Enviada"] = ["Entregada"],
        ["Entregada"] = [],
        ["Cancelada"] = []
    };

    public OrderService(
        IOrderRepository repository,
        IUsersApiClient usersApi,
        IProductsApiClient productsApi)
    {
        _repository = repository;
        _usersApi = usersApi;
        _productsApi = productsApi;
    }

    public Task<List<Order>> GetAllAsync(Guid? usuarioId)
    {
        return _repository.GetAllAsync(usuarioId);
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);

        if (order is null)
            throw new OrderNotFoundException(); 

        return order;
    }

    public async Task<Order> CreateAsync(CreateOrderRequest request)
    {
        var errores = new List<string>();

        if (request.UsuarioId == Guid.Empty)
            errores.Add("El campo usuarioId es requerido.");

        if (request.Items is null || request.Items.Count == 0)
            errores.Add("La orden debe tener al menos un item.");
        else
        {
            if (request.Items.Any(i => i.ProductoId == Guid.Empty))
                errores.Add("Todos los items deben tener productoId.");

            if (request.Items.Any(i => i.Cantidad <= 0))
                errores.Add("La cantidad de cada item debe ser mayor a cero.");
        }

        if (errores.Count > 0)
            throw new OrderDatosInvalidosException(string.Join(" ; ", errores));

         var itemsConsolidados = request.Items!
            .GroupBy(i => i.ProductoId)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
            .ToList();

        var usuarioExiste = await _usersApi.UserExistsAsync(request.UsuarioId);

        if (!usuarioExiste)
            throw new OrderUsuarioNotFoundException();

        var orderItems = new List<OrderItem>();
        var sinStock = new List<string>();

        foreach (var item in itemsConsolidados)
        {
            var producto = await _productsApi.GetProductAsync(item.ProductoId);

            if (producto is null)
                throw new OrderProductoNotFoundException(
                    $"Producto no encontrado al crear la orden: '{item.ProductoId}'.");

            if (producto.Stock < item.Cantidad) 
            {
                sinStock.Add(
                    $"Stock insuficiente para '{producto.Nombre}'. " +
                    $"Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");
                continue;
            }

            orderItems.Add(new OrderItem
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        if (sinStock.Count > 0)
            throw new OrderStockInsuficienteException(string.Join(" ; ", sinStock));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Items = orderItems,
            Total = orderItems.Sum(i => i.Cantidad * i.PrecioUnitario),
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = null
        };

        await _repository.CreateAsync(order);

        return order;
    }

    public async Task<Order> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request)
    {
        var estadoDestino = TransicionesValidas.Keys
            .FirstOrDefault(e => e.Equals(request.Estado, StringComparison.OrdinalIgnoreCase));

        if (estadoDestino is null)
            throw new OrderDatosInvalidosException(
                $"El estado '{request.Estado}' no es válido. " +
                "Estados posibles: Pendiente, Confirmada, Enviada, Entregada, Cancelada.");

        var order = await _repository.GetByIdAsync(id);

        if (order is null)
            throw new OrderNotFoundException();

        if (!TransicionesValidas[order.Estado].Contains(estadoDestino))
            throw new OrderEstadoInvalidoException(
                $"Una orden en estado '{order.Estado}' no puede pasar a '{estadoDestino}'.");

        var fechaActualizacion = DateTime.UtcNow;
        await _repository.UpdateStatusAsync(id, estadoDestino, fechaActualizacion);

        order.Estado = estadoDestino;
        order.FechaActualizacion = fechaActualizacion;

        return order;
    }
}
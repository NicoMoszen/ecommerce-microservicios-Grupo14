using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Models;
using Orders.API.Services;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
	private readonly IOrderService _orderService;

	public OrdersController(IOrderService orderService)
	{
		_orderService = orderService;
	}

	[HttpGet]
	[ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAll([FromQuery] Guid? usuarioId)
	{
		var orders = await _orderService.GetAllAsync(usuarioId);
		return Ok(orders.Select(MapToResponse).ToList());
	}

	[HttpGet("{id}")]
	[ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetById(Guid id)
	{
		var order = await _orderService.GetByIdAsync(id);
		return Ok(MapToResponse(order));
	}

	[HttpPost]
	[ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
	{
		var order = await _orderService.CreateAsync(request);

		return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToResponse(order));
	}

	[HttpPut("{id}/status")]
	[ProducesResponseType(typeof(UpdateOrderStatusResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
	{
		var order = await _orderService.UpdateStatusAsync(id, request);

		return Ok(new UpdateOrderStatusResponse
		{
			Id = order.Id,
			Estado = order.Estado,
			FechaActualizacion = order.FechaActualizacion!.Value
		});
	}

	private static OrderResponse MapToResponse(Order order) => new()
	{
		Id = order.Id,
		UsuarioId = order.UsuarioId,
		Items = order.Items.Select(i => new OrderItemResponse
		{
			ProductoId = i.ProductoId,
			Cantidad = i.Cantidad,
			PrecioUnitario = i.PrecioUnitario
		}).ToList(),
		Total = order.Total,
		Estado = order.Estado,
		FechaCreacion = order.FechaCreacion
	};
}
using System.Security.Claims;
using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Order;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request)
    {
        var userId = GetUserId();

        var result = await _orderService.CreateAsync(
            userId,
            request);

        if (!result.Success)
            return BadRequest(
                ApiResponse<object>.Fail(result.Message));

        return Ok(
            ApiResponse<OrderDto>.Ok(
                result.Data!,
                result.Message));
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();

        var orders = await _orderService
            .GetMyOrdersAsync(userId);

        return Ok(
            ApiResponse<List<OrderDto>>.Ok(orders));
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetById(int orderId)
    {
        var userId = GetUserId();

        var result = await _orderService
            .GetByIdAsync(userId, orderId);

        if (!result.Success)
            return NotFound(
                ApiResponse<object>.Fail(result.Message));

        return Ok(
            ApiResponse<OrderDto>.Ok(
                result.Data!,
                result.Message));
    }
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllForAdmin()
    {
        var orders = await _orderService.GetAllForAdminAsync();

        return Ok(
            ApiResponse<List<AdminOrderDto>>.Ok(orders));
    }

    [HttpPut("{orderId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
    int orderId,
    UpdateOrderStatusRequest request)
    {
        var result =
            await _orderService.UpdateStatusAsync(
                orderId,
                request.Status);

        if (!result.Success)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    result.Message));
        }

        return Ok(
            ApiResponse<object?>.Ok(
                null,
                result.Message));
    }

    private int GetUserId()
    {
        var claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}
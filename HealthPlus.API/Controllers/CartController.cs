using System.Security.Claims;
using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Cart;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();

        var cart = await _cartService.GetCartAsync(userId);

        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(
        [FromBody] AddCartItemRequest request)
    {
        var userId = GetUserId();

        var result = await _cartService.AddItemAsync(
            userId,
            request);

        if (!result.Success)
            return BadRequest(
                ApiResponse<object>.Fail(result.Message));

        return Ok(
            ApiResponse<CartDto>.Ok(
                result.Data!,
                result.Message));
    }

    [HttpPut("{productId:int}")]
    public async Task<IActionResult> UpdateItem(
        int productId,
        [FromBody] UpdateCartItemRequest request)
    {
        var userId = GetUserId();

        var result = await _cartService.UpdateItemAsync(
            userId,
            productId,
            request);

        if (!result.Success)
            return BadRequest(
                ApiResponse<object>.Fail(result.Message));

        return Ok(
            ApiResponse<CartDto>.Ok(
                result.Data!,
                result.Message));
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();

        var result = await _cartService.RemoveItemAsync(
            userId,
            productId);

        if (!result.Success)
            return NotFound(
                ApiResponse<object>.Fail(result.Message));

        return Ok(
            ApiResponse<object>.Ok(
                null!,
                result.Message));
    }

    private int GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}
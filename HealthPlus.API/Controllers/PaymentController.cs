using System.Security.Claims;
using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Payment;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("{orderId:int}")]
    public async Task<IActionResult> Pay(
        int orderId,
        PaymentRequest request)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result =
            await _paymentService.ProcessPaymentAsync(
                userId,
                orderId,
                request);

        if (!result.Success)
        {
            return BadRequest(
                ApiResponse<PaymentDto?>.Fail(
                    result.Message));
        }

        return Ok(
            ApiResponse<PaymentDto?>.Ok(
                result.Data!,
                result.Message));
    }
}
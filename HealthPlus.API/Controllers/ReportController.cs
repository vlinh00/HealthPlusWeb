using HealthPlus.API.Common;
using HealthPlus.API.DTOs.Report;
using HealthPlus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthPlus.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(
        IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result =
            await _reportService.GetSummaryAsync();

        return Ok(
            ApiResponse<ReportSummaryDto>.Ok(result));
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result =
            await _reportService.GetRevenueAsync(
                fromDate,
                toDate);

        return Ok(
            ApiResponse<List<RevenueReportDto>>.Ok(
                result));
    }

    [HttpGet("best-selling-products")]
    public async Task<IActionResult>
        GetBestSellingProducts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
    {
        var result =
            await _reportService
                .GetBestSellingProductsAsync(
                    fromDate,
                    toDate);

        return Ok(
            ApiResponse<List<BestSellingProductDto>>.Ok(
                result));
    }
}
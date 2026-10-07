using HealthPlus.API.DTOs.Report;
using HealthPlus.API.Services.Interfaces;
using HealthPlus.API.Repositories.Interfaces;

namespace HealthPlus.API.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(
        IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportSummaryDto>
        GetSummaryAsync()
    {
        var totalOrders =
            await _reportRepository.GetTotalOrdersAsync(
                null,
                null);

        var totalRevenue =
            await _reportRepository.GetTotalRevenueAsync(
                null,
                null);

        var totalCustomers =
            await _reportRepository.GetTotalCustomersAsync();

        var totalProducts =
            await _reportRepository.GetTotalProductsAsync();

        return new ReportSummaryDto
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts
        };
    }

    public async Task<List<RevenueReportDto>>
        GetRevenueAsync(
            DateTime? fromDate,
            DateTime? toDate)
    {
        var orders =
            await _reportRepository
                .GetOrdersForRevenueAsync(
                    fromDate,
                    toDate);

        return orders
            .GroupBy(x => x.OrderDate.Date)
            .OrderBy(x => x.Key)
            .Select(x => new RevenueReportDto
            {
                Date = x.Key,
                Revenue = x.Sum(o => o.TotalAmount)
            })
            .ToList();
    }

    public async Task<List<BestSellingProductDto>>
        GetBestSellingProductsAsync(
            DateTime? fromDate,
            DateTime? toDate)
    {
        var orderItems =
            await _reportRepository
                .GetOrderItemsForBestSellingAsync(
                    fromDate,
                    toDate);

        return orderItems
            .GroupBy(x => new
            {
                x.ProductId,
                x.ProductName
            })
            .Select(x => new BestSellingProductDto
            {
                ProductId = x.Key.ProductId,

                ProductName = x.Key.ProductName,

                QuantitySold =
                    x.Sum(i => i.Quantity),

                Revenue =
                    x.Sum(i =>
                        i.UnitPrice * i.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .ToList();
    }
}
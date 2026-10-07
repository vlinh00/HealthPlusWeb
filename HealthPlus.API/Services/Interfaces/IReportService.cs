using HealthPlus.API.DTOs.Report;

namespace HealthPlus.API.Services.Interfaces;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync();

    Task<List<RevenueReportDto>> GetRevenueAsync(
        DateTime? fromDate,
        DateTime? toDate);

    Task<List<BestSellingProductDto>>
        GetBestSellingProductsAsync(
            DateTime? fromDate,
            DateTime? toDate);
}
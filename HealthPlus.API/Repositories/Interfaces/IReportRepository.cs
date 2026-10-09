using HealthPlus.API.Entities;

namespace HealthPlus.API.Repositories.Interfaces;

public interface IReportRepository
{
    Task<int> GetTotalOrdersAsync(
        DateTime? fromDate,
        DateTime? toDate);

    Task<decimal> GetTotalRevenueAsync(
        DateTime? fromDate,
        DateTime? toDate);

    Task<int> GetTotalCustomersAsync();

    Task<int> GetTotalProductsAsync();

    Task<List<Order>> GetOrdersForRevenueAsync(
        DateTime? fromDate,
        DateTime? toDate);

    Task<List<OrderItem>> GetOrderItemsForBestSellingAsync(
        DateTime? fromDate,
        DateTime? toDate);
}
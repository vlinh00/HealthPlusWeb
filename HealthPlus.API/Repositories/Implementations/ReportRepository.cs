using HealthPlus.API.Data;
using HealthPlus.API.Entities;
using HealthPlus.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthPlus.API.Repositories.Implementations;

public class ReportRepository : IReportRepository
{
    private readonly HealthPlusDbContext _context;

    public ReportRepository(HealthPlusDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalOrdersAsync(
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(x => x.Status != "Cancelled");

        query = ApplyDateFilter(
            query,
            fromDate,
            toDate);

        return await query.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(x =>
                x.PaymentStatus == "Paid" &&
                x.Status != "Cancelled");

        query = ApplyDateFilter(
            query,
            fromDate,
            toDate);

        return await query
            .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;
    }

    public async Task<int> GetTotalCustomersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .CountAsync(x => x.Role == "Customer");
    }

    public async Task<int> GetTotalProductsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .CountAsync(x => x.IsActive);
    }

    public async Task<List<Order>> GetOrdersForRevenueAsync(
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(x =>
                x.PaymentStatus == "Paid" &&
                x.Status != "Cancelled");

        query = ApplyDateFilter(
            query,
            fromDate,
            toDate);

        return await query
            .OrderBy(x => x.OrderDate)
            .ToListAsync();
    }

    public async Task<List<OrderItem>>
        GetOrderItemsForBestSellingAsync(
            DateTime? fromDate,
            DateTime? toDate)
    {
        var query = _context.OrderItems
            .AsNoTracking()
            .Include(x => x.Order)
            .Where(x =>
                x.Order.PaymentStatus == "Paid" &&
                x.Order.Status != "Cancelled");

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.Order.OrderDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endDate =
                toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.Order.OrderDate < endDate);
        }

        return await query.ToListAsync();
    }

    private static IQueryable<Order> ApplyDateFilter(
        IQueryable<Order> query,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.OrderDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endDate =
                toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.OrderDate < endDate);
        }

        return query;
    }
}
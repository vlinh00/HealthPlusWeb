namespace HealthPlus.API.DTOs.Report;

public class ReportSummaryDto
{
    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public int TotalCustomers { get; set; }

    public int TotalProducts { get; set; }
}
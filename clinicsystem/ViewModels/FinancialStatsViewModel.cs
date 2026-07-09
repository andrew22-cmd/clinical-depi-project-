namespace clinicsystem.ViewModels
{
    public class FinancialStatsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueThisWeek { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int TotalPaidReservations { get; set; }
        public decimal AverageRevenuePerReservation { get; set; }
        public decimal HighestConsultationFee { get; set; }
        public decimal LowestConsultationFee { get; set; }
    }
}

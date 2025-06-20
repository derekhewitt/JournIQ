namespace JournIQ.UI.Models
{
    public class CalendarWeekViewModel
    {
        public List<CalendarDayViewModel> Days { get; set; } = new();
        public decimal WeeklyPnL => Days.Sum(d => d.TotalPnL);
        public int WeeklyTradeCount => Days.Sum(d => d.TradeCount);
        public decimal WeeklyAvgEfficiency()
        {
            if (Days.Count == 0) return 0;

            return Days.Average(d => d.AvgEfficiency());
        }
            
    }

}

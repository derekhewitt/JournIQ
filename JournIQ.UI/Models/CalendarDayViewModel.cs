using JournalIQ.Core;

namespace JournIQ.UI
{
    public class CalendarDayViewModel
    {
        public DateTime Date { get; set; }
        public List<Trade> Trades { get; set; } = new();
        public decimal TotalPnL => Trades.Sum(t => t.PnL);
        public int TradeCount => Trades.Count;
        public decimal AvgEfficiency()
        {
            if(Trades.Count == 0) return 0;

            return Trades
                .Where(t => t.EntryEfficiency.HasValue && t.ExitEfficiency.HasValue)
                .DefaultIfEmpty()
                .Average(t => ((t.EntryEfficiency ?? 0) + (t.ExitEfficiency ?? 0)) / 2);
        }
            
    }

}

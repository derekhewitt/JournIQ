namespace JournalIQ.Core
{
    public class Trade
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime EntryTime { get; set; }
        public decimal EntryPrice { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal? ExitPrice { get; set; }
        public int Quantity { get; set; }
        public string Direction { get; set; } // "Long" or "Short"
        public string Notes { get; set; }
        public decimal? HighDuringPosition { get; set; }
        public decimal? LowDuringPosition { get; set; }

        public List<TradeTag> TradeTags { get; set; } = new();
    }

}

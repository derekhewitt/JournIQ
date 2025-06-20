using JournalIQ.Core.Models;
using System.Drawing;

namespace JournalIQ.Core
{
    public static class TradingConfig
    {
        public const decimal PerTradeCommission = 1.04m;
    }

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
        public long InternalOrderId { get; set; } 
        public decimal? HighDuringPosition { get; set; }
        public decimal? LowDuringPosition { get; set; }
        public decimal PnL
        {
            get
            {
                if (!ExitPrice.HasValue)
                    return 0;

                var tickValue = FuturesContractSpecs.GetTickValue(Symbol);
                var tickSize = FuturesContractSpecs.GetTickSize(Symbol);

                if (tickValue == 0 || tickSize == 0)
                    return 0;

                var priceDifference = Direction == "Long"
                    ? ExitPrice.Value - EntryPrice
                    : EntryPrice - ExitPrice.Value;

                var tickCount = priceDifference / tickSize;
                var grossPnL = tickCount * tickValue * Quantity;

                return grossPnL - TradingConfig.PerTradeCommission;
            }
        }



        public List<TradeTag> TradeTags { get; set; } = new();
    }

}

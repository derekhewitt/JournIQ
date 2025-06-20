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
        public decimal MaxOpenProfit
        {
            get
            {
                var valueToCompare = Direction == "Long" ? HighDuringPosition : LowDuringPosition;
                if (!valueToCompare.HasValue)
                    return 0;

                return CalculateGrossPnL(valueToCompare!.Value);
            }
        }
        public decimal MaxOpenLoss
        {
            get
            {
                var valueToCompare = Direction == "Long" ? LowDuringPosition : HighDuringPosition;
                if (!valueToCompare.HasValue)
                    return 0;

                return CalculateGrossPnL(valueToCompare!.Value);
            }
        }
        public decimal? EntryEfficiency
        {
            get
            {
                if (!HighDuringPosition.HasValue || !LowDuringPosition.HasValue)
                    return null;

                var range = HighDuringPosition.Value - LowDuringPosition.Value;
                if (range == 0) return 100;

                return Direction == "Long"
                    ? (1 - (EntryPrice - LowDuringPosition.Value) / range) * 100
                    : (1 - (HighDuringPosition.Value - EntryPrice) / range) * 100;
            }
        }

        public decimal? ExitEfficiency
        {
            get
            {
                if (!ExitPrice.HasValue || !HighDuringPosition.HasValue || !LowDuringPosition.HasValue)
                    return null;
                if (PnL < 0)
                    return 0;

                var denominator = MaxOpenProfit - MaxOpenLoss;
                return (denominator == 0 ? 0m : (PnL - MaxOpenLoss) / denominator) * 100;

            }
        }
        public decimal PnL
        {
            get
            {
                if (!ExitPrice.HasValue)
                    return 0;

                return CalculateGrossPnL(ExitPrice.Value) - TradingConfig.PerTradeCommission;
            }
        }
        public List<TradeTag> TradeTags { get; set; } = new();

        #region Methods
        private decimal CalculateGrossPnL(decimal fromPrice)
        {
            var tickValue = FuturesContractSpecs.GetTickValue(Symbol);
            var tickSize = FuturesContractSpecs.GetTickSize(Symbol);

            if (tickValue == 0 || tickSize == 0)
                return 0;

            var priceDifference = Direction == "Long"
                    ? fromPrice - EntryPrice
                    : EntryPrice - fromPrice;

            var tickCount = priceDifference / tickSize;
            var grossPnL = tickCount * tickValue * Quantity;


            return grossPnL;
        }
        #endregion
    }

}

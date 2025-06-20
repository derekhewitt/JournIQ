using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIQ.Core
{
    public class SierraTradeRow
    {
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public string OrderType { get; set; }
        public int Quantity { get; set; }
        public string BuySell { get; set; }
        public decimal FillPrice { get; set; }
        public string OpenClose { get; set; }
        public long InternalOrderId { get; set; }
        public decimal? HighDuringPosition { get; set; }
        public decimal? LowDuringPosition { get; set; }
    }
}

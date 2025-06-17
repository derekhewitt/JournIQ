using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIQ.Core
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TradeTag> TradeTags { get; set; } = new();
    }

    public class TradeTag
    {
        public int TradeId { get; set; }
        public Trade Trade { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}

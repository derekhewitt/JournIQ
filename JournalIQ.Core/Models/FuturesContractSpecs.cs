using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIQ.Core.Models
{
    public static class FuturesContractSpecs
    {
        private static readonly Dictionary<string, (decimal TickSize, decimal TickValue)> Specs = new(StringComparer.OrdinalIgnoreCase)
    {
        { "MES", (0.25m, 1.25m) },
        { "MNQ", (0.25m, 0.50m) },
        { "ES",  (0.25m, 12.50m) },
        { "NQ",  (0.25m, 5.00m) }
    };

        public static decimal GetTickValue(string symbol)
        {
            return GetSpec(symbol).TickValue;
        }

        public static decimal GetTickSize(string symbol)
        {
            return GetSpec(symbol).TickSize;
        }

        private static (decimal TickSize, decimal TickValue) GetSpec(string symbol)
        {
            foreach (var key in Specs.Keys)
            {
                if (symbol.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    return Specs[key];
            }

            return (0m, 0m);
        }
    }


}

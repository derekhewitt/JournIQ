using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JournalIQ.Core
{
    public class TradeImportService : ITradeImportService
    {
        private readonly ITradeRepository _tradeRepo;
        private readonly ITagRepository _tagRepo;

        public TradeImportService(ITradeRepository tradeRepo, ITagRepository tagRepo)
        {
            _tradeRepo = tradeRepo;
            _tagRepo = tagRepo;
        }

        public async Task ImportSierraTradesAsync(List<SierraTradeRow> parsedTrades)
        {
            var sierraTag = await _tagRepo.GetOrCreateAsync("sierra");

            var trades = ConvertToTrades(parsedTrades);

            foreach (var trade in trades)
            {
                trade.TradeTags = new List<TradeTag> { new() { Tag = sierraTag } };
                await _tradeRepo.AddTradeAsync(trade);
            }
           
            await _tradeRepo.SaveAsync();
        }

        public List<Trade> ConvertToTrades(List<SierraTradeRow> fills)
        {
            var trades = new List<Trade>();
            var openFills = new Queue<SierraTradeRow>();

            foreach (var fill in fills.OrderBy(f => f.DateTime))
            {
                if (fill.OpenClose == "Open")
                {
                    openFills.Enqueue(fill);
                }
                else if (fill.OpenClose == "Close" && openFills.Count > 0)
                {
                    var entry = openFills.Dequeue();

                    var trade = new Trade
                    {
                        Symbol = CleanSymbol(entry.Symbol),
                        EntryTime = entry.DateTime,
                        EntryPrice = entry.FillPrice,
                        ExitTime = fill.DateTime,
                        ExitPrice = fill.FillPrice,
                        Direction = entry.BuySell.Equals("Buy", StringComparison.OrdinalIgnoreCase) ? "Long" : "Short",
                        Quantity = entry.Quantity, // assumes 1:1 match
                        Notes = $"Imported from Sierra on {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                        HighDuringPosition = fill.HighDuringPosition,
                        LowDuringPosition = fill.LowDuringPosition,
                    };

                    trades.Add(trade);
                }
            }

            return trades;
        }

        public static string CleanSymbol(string rawSymbol)
        {
            if (string.IsNullOrWhiteSpace(rawSymbol))
                return string.Empty;

            // Strip off [Sim] or brackets if present
            var cleaned = Regex.Replace(rawSymbol, @"^\[.*?\]", "").Trim();

            // Optionally remove suffixes like "_FUT_CME"
            var underscoreIndex = cleaned.IndexOf("_");
            if (underscoreIndex > 0)
                cleaned = cleaned.Substring(0, underscoreIndex);

            return cleaned;
        }


    }

}

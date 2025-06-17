using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            foreach (var row in parsedTrades)
            {
                var trade = new Trade
                {
                    Symbol = row.Symbol,
                    EntryTime = row.DateTime,
                    EntryPrice = row.FillPrice,
                    Quantity = row.Quantity,
                    Direction = row.BuySell.Equals("Buy", StringComparison.OrdinalIgnoreCase) ? "Long" : "Short",
                    Notes = $"Imported from Sierra on {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                    HighDuringPosition = row.HighDuringPosition,
                    LowDuringPosition = row.LowDuringPosition,
                    TradeTags = new List<TradeTag> { new() { Tag = sierraTag } }
                };

                await _tradeRepo.AddTradeAsync(trade);
            }

            await _tradeRepo.SaveAsync();
        }
    }

}

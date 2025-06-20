
namespace JournalIQ.Core
{
    public interface ITradeImportService
    {
        Task<(int imported, int duplicates)> ImportSierraTradesAsync(List<SierraTradeRow> parsedTrades);
    }

}


namespace JournalIQ.Core
{
    public interface ITradeImportService
    {
        Task ImportSierraTradesAsync(List<SierraTradeRow> parsedTrades);
    }

}

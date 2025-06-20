namespace JournalIQ.Core
{
    public interface ITradeRepository
    {
        Task AddTradeAsync(Trade trade);
        Task<List<long>> GetInternalOrderIdsAsync();
        Task<List<Trade>> GetAllAsync();
        Task SaveAsync();
    }

}

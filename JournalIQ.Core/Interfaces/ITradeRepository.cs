namespace JournalIQ.Core
{
    public interface ITradeRepository
    {
        Task AddTradeAsync(Trade trade);
        Task<List<Trade>> GetAllAsync();
        Task SaveAsync();
    }

}

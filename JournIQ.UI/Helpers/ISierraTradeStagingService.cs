using JournalIQ.Core;

namespace JournIQ.UI
{
    public interface ISierraTradeStagingService
    {
        void Store(string userId, List<SierraTradeRow> trades);
        List<SierraTradeRow> Retrieve(string userId);
        void Clear(string userId);
    }

    public class SierraTradeStagingService : ISierraTradeStagingService
    {
        private readonly Dictionary<string, List<SierraTradeRow>> _storage = new();

        public void Store(string userId, List<SierraTradeRow> trades)
        {
            _storage[userId] = trades;
        }

        public List<SierraTradeRow> Retrieve(string userId)
        {
            return _storage.ContainsKey(userId) ? _storage[userId] : new List<SierraTradeRow>();
        }

        public void Clear(string userId)
        {
            if (_storage.ContainsKey(userId))
                _storage.Remove(userId);
        }
    }
}

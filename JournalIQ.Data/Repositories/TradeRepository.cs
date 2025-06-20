using JournalIQ.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalIQ.Data
{
    public class TradeRepository : ITradeRepository
    {
        private readonly DataContext _context;

        public TradeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddTradeAsync(Trade trade)
        {
            await _context.Trades.AddAsync(trade);
        }

        public Task<List<Trade>> GetAllAsync() => _context.Trades.ToListAsync();

        public Task SaveAsync() => _context.SaveChangesAsync();

        public async Task<List<long>> GetInternalOrderIdsAsync()
        {
            return await _context.Trades
                .Select(t => t.InternalOrderId)
                .ToListAsync();
        }
    }

}

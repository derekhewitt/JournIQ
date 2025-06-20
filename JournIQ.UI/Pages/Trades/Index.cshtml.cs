using JournalIQ.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JournIQ.UI.Pages.Trades;

public class IndexModel : PageModel
{
    private readonly ITradeRepository _tradeRepository;

    public IndexModel(ITradeRepository tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    [BindProperty(SupportsGet = true)]
    public string? SymbolFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public List<TradeDayGroup> GroupedTrades { get; set; } = new();

    public async Task OnGetAsync()
    {
        var trades = await _tradeRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(SymbolFilter))
        {
            trades = trades.Where(t => t.Symbol.Contains(SymbolFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (StartDate.HasValue)
        {
            trades = trades.Where(t => t.EntryTime.Date >= StartDate.Value.Date).ToList();
        }

        if (EndDate.HasValue)
        {
            trades = trades.Where(t => t.EntryTime.Date <= EndDate.Value.Date).ToList();
        }

        GroupedTrades = trades
            .GroupBy(t => t.EntryTime.Date)
            .Select(g => new TradeDayGroup
            {
                Date = g.Key,
                DailyPnL = g.Sum(t => t.PnL),
                Trades = g.OrderBy(t => t.EntryTime).ToList()
            })
            .OrderByDescending(g => g.Date)
            .ToList();
    }

    public string GetEfficiencyColor(decimal? efficiency)
    {
        if (!efficiency.HasValue) return "text-muted";
        if (efficiency >= 80) return "text-success";
        if (efficiency >= 50) return "text-primary";
        if (efficiency >= 20) return "text-warning";
        return "text-danger";
    }
}

public class TradeDayGroup
{
    public DateTime Date { get; set; }
    public decimal DailyPnL { get; set; }
    public List<Trade> Trades { get; set; } = new();
}

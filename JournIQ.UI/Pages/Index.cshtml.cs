using JournalIQ.Core;
using JournIQ.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace JournIQ.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITradeRepository _tradeRepository;
        public List<CalendarWeekViewModel> CalendarWeeks { get; set; }

        public decimal TotalNetPnL { get; set; }
        public double ProfitFactor { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public decimal Expectancy { get; set; }
        public double WinRate => TotalWins + TotalLosses == 0 ? 0 : (double)TotalWins / (TotalWins + TotalLosses);


        public IndexModel(ILogger<IndexModel> logger, ITradeRepository tradeRepository)
        {
            _logger = logger;
            _tradeRepository = tradeRepository;
        }

        public async Task OnGetAsync()
        {
            var allTrades = await _tradeRepository.GetAllAsync();


            // == Dashboard Metrics ==
            TotalNetPnL = allTrades.Sum(t => t.PnL);

            var winningTrades = allTrades.Where(t => t.PnL > 0).ToList();
            var losingTrades = allTrades.Where(t => t.PnL < 0).ToList();

            TotalWins = winningTrades.Count;
            TotalLosses = losingTrades.Count;

            var grossProfit = winningTrades.Sum(t => t.PnL);
            var grossLoss = -losingTrades.Sum(t => t.PnL); // Convert to positive for calculation

            ProfitFactor = grossLoss > 0 ? (double)(grossProfit / grossLoss) : 0;

            var avgWin = winningTrades.Any() ? winningTrades.Average(t => t.PnL) : 0;
            var avgLoss = losingTrades.Any() ? -losingTrades.Average(t => t.PnL) : 0;

            Expectancy = (decimal)(WinRate * (double)avgWin - (1 - WinRate) * (double)avgLoss);

            // == Calendar Logic ==
            var groupedByDay = allTrades
                .GroupBy(t => t.EntryTime.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // fill in days (even with no trades)
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).StartOfWeek(DayOfWeek.Sunday);
            var end = start.AddDays(41); // 6-week calendar

            var days = new List<CalendarDayViewModel>();

            for (var day = start; day <= end; day = day.AddDays(1))
            {
                groupedByDay.TryGetValue(day.Date, out var trades);
                days.Add(new CalendarDayViewModel
                {
                    Date = day,
                    Trades = trades ?? new List<Trade>()
                });
            }

            // group into weeks
            CalendarWeeks = days
                .Select((d, i) => new { d, i })
                .GroupBy(x => x.i / 7)
                .Select(g => new CalendarWeekViewModel
                {
                    Days = g.Select(x => x.d).ToList()
                }).ToList();

        }

    }
}

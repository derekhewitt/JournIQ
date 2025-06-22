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
        public decimal LargestWin { get; set; }
        public decimal LargestLoss { get; set; }
        public double AvgDurationMinutes { get; set; }
        public decimal AvgWin { get; set; }
        public decimal AvgLoss { get; set; }
        public List<(DateTime Date, decimal PnL)> RecentDaysPnL { get; set; }


        public IndexModel(ILogger<IndexModel> logger, ITradeRepository tradeRepository)
        {
            _logger = logger;
            _tradeRepository = tradeRepository;
        }

        public async Task OnGetAsync()
        {
            var allTrades = await _tradeRepository.GetAllAsync();


            // == Dashboard Metrics ==
            if (allTrades.Any())
            {
                TotalNetPnL = allTrades.Sum(t => t.PnL);

                var wins = allTrades.Where(t => t.PnL > 0).ToList();
                var losses = allTrades.Where(t => t.PnL < 0).ToList();

                TotalWins = wins.Count;
                TotalLosses = losses.Count;

                var grossProfit = wins.Sum(t => t.PnL);
                var grossLoss = Math.Abs(losses.Sum(t => t.PnL));

                ProfitFactor = grossLoss == 0 ? 0 : (double)(grossProfit / grossLoss);
                Expectancy = allTrades.Average(t => t.PnL);

                LargestWin = wins.Any() ? wins.Max(t => t.PnL) : 0;
                LargestLoss = losses.Any() ? losses.Min(t => t.PnL) : 0;

                AvgWin = wins.Count > 0 ? wins.Average(t => t.PnL) : 0;
                AvgLoss = losses.Count > 0 ? losses.Average(t => t.PnL) : 0;

                AvgDurationMinutes = allTrades
                    .Where(t => t.EntryTime != null && t.ExitTime != null)
                    .Select(t => (t.ExitTime.Value - t.EntryTime).TotalMinutes)
                    .DefaultIfEmpty(0)
                    .Average();

                var pnlByDate = allTrades
                .GroupBy(t => t.EntryTime.Date)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.PnL));

                // Get last 14 *weekdays* (excluding Sat/Sun), even if no trades
                RecentDaysPnL = Enumerable.Range(0, 30) // scan 30 calendar days back
                    .Select(offset => DateTime.Today.AddDays(-offset))
                    .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    .Take(21)
                    .Select(date => (date, pnlByDate.TryGetValue(date, out var pnl) ? pnl : 0m))
                    .Reverse()
                    .ToList();

            }


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

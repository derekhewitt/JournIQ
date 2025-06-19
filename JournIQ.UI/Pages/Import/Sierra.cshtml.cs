using JournalIQ.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace JournIQ.UI.Pages.Import;

public class SierraModel : PageModel
{
    private readonly ITradeImportService _importService;
    private readonly ISierraTradeStagingService _stagingService;

    public SierraModel(ITradeImportService importService, ISierraTradeStagingService stagingService)
    {
        _importService = importService;
        _stagingService = stagingService;
    }

    [BindProperty]
    public IFormFile UploadFile { get; set; }

    public List<SierraTradeRow> ParsedTrades { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (UploadFile == null || UploadFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid .txt file.");
            return Page();
        }

        var trades = await ParseSierraLogAsync(UploadFile);
        _stagingService.Store("guest", trades);
        ParsedTrades = trades;
        return Page();
    }

    public async Task<IActionResult> OnPostImportAsync()
    {
        var trades = _stagingService.Retrieve("guest");
        if (trades == null || !trades.Any())
        {
            TempData["Error"] = "No staged trades to import.";
            return RedirectToPage();
        }

        await _importService.ImportSierraTradesAsync(trades);
        _stagingService.Clear("guest");

        TempData["Success"] = $"{trades.Count} trades imported successfully.";
        return RedirectToPage("/Index");
    }

    private async Task<List<SierraTradeRow>> ParseSierraLogAsync(IFormFile file)
    {
        var trades = new List<SierraTradeRow>();
        using var reader = new StreamReader(file.OpenReadStream());
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("ActivityType"))
                continue;

            var parts = line.Split('\t');
            System.Diagnostics.Debug.WriteLine("---- Parsed Row ----");
            for (int i = 0; i < parts.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine($"[{i}] = {parts[i]}");
            }

            trades.Add(new SierraTradeRow
            {
                DateTime = DateTime.Parse(parts[1]),
                Symbol = parts[3],
                OrderType = parts[7],
                Quantity = int.TryParse(parts[8], out var qty) ? qty : 0,
                BuySell = parts[9],
                FillPrice = decimal.TryParse(parts[13], out var fp) ? fp : 0,
                OpenClose = parts[16],
                HighDuringPosition = parts.Length > 20 && decimal.TryParse(parts[20], out var high) ? high : null,
                LowDuringPosition = parts.Length > 21 && decimal.TryParse(parts[21], out var low) ? low : null
            });
        }

        return trades;
    }
}

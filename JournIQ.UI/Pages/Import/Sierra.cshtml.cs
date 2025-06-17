using JournalIQ.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace JournIQ.UI.Pages.Import;

public class SierraModel : PageModel
{
    private readonly ITradeImportService _importService;

    public SierraModel(ITradeImportService importService)
    {
        _importService = importService;
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

        ParsedTrades = await ParseSierraLogAsync(UploadFile);
        return Page();
    }

    public async Task<IActionResult> OnPostImportAsync()
    {
        if (UploadFile == null || UploadFile.Length == 0)
        {
            ModelState.AddModelError("", "Missing file for import.");
            return Page();
        }

        var trades = await ParseSierraLogAsync(UploadFile);
        await _importService.ImportSierraTradesAsync(trades);

        TempData["Success"] = $"{trades.Count} trades imported successfully.";
        return RedirectToPage("/Trades/Index");
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

            var parts = Regex.Split(line.Trim(), @"\s+");
            System.Diagnostics.Debug.WriteLine("---- Parsed Row ----");
            for (int i = 0; i < parts.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine($"[{i}] = {parts[i]}");
            }

            if (parts.Length < 22) continue;

            trades.Add(new SierraTradeRow
            {
                DateTime = DateTime.Parse($"{parts[1]} {parts[2]}"),
                Symbol = parts[5],
                OrderType = parts[17],
                Quantity = int.TryParse(parts[18], out var qty) ? qty : 0,
                BuySell = parts[19],
                FillPrice = decimal.TryParse(parts[21], out var fp) ? fp : 0,
                OpenClose = parts[24],
                HighDuringPosition = decimal.TryParse(parts[26], out var high) ? high : null,
                LowDuringPosition = decimal.TryParse(parts[27], out var low) ? low : null
            });

        }

        return trades;
    }
}

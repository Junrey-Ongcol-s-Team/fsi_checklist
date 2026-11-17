// =====================================================================
// Basahon ug i-save ang mga setting sa app (pananglitan ngalan sa istasyon) sa table nga app_setting.
// =====================================================================
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public sealed class AppSettingsService
{
    public const string StationName = "station_name";
    public const string StationAddress = "station_address";
    public const string StationRegion = "station_region";     // letterhead linya 1: "Region 10"
    public const string StationOffice = "station_office";     // letterhead linya 2: "Misamis Occidental Provincial Office"
    public const string StationEmail = "station_email";       // letterhead: email sa station
    public const string StationHotline = "station_hotline";   // letterhead: hotline number sa station

    private readonly DatabaseService _database;

    public AppSettingsService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<string> GetAsync(string key)
    {
        await using var db = await _database.OpenContextAsync();
        var row = await db.AppSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Key == key);
        return row?.Value ?? string.Empty;
    }

    public async Task SetAsync(string key, string? value)
    {
        await using var db = await _database.OpenContextAsync();
        var row = await db.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (row == null)
            db.AppSettings.Add(new AppSetting { Key = key, Value = value });
        else
            row.Value = value;
        await db.SaveChangesAsync();
    }
}

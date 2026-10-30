// =====================================================================
// PRE-SIGNED NGA PIRMA (bahin 4 sa InspectionService)
//
// Ang mga taga-BFP (Inspector, Team Leader, Chief, Fire Marshal...) mo-pirma KAUSA ra sa Settings.
// Ang ilang pirma ma-save, dayon tawgon na lang kada inspection:
//   - Pirma sa inspection (ON_SITE), pananglitan Inspector / Team Leader:
//       awtomatik ibutang kung humana na ang form (DRAFT pa).
//   - Pirma sa opisina (APPROVAL), pananglitan Chief FSES:
//       awtomatik ibutang human ma-Complete ang inspection.
//   - Fire Marshal: mo-pili lang og Approved / Disapproved; ang saved nga pirma ang gamiton.
// Ang Owner / Representative ra ang mo-pirma og bag-o kada inspection.
//
// Asa gi-save:
//   - Ngalan  : table nga app_setting, key "presign.INSPECTOR.name" (ug uban pa)
//   - Pirma   : file nga signatures/presign-INSPECTOR.json (sa app data folder)
// Kada inspection naay KAUGALINGON nga kopya sa pirma (signatures/{id}.json) uban ang SHA-256 hash.
// =====================================================================
using System.Security.Cryptography;
using System.Text;
using fsi_checklist.Data;
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

/// <summary>One saved (pre-signed) BFP signature, shown in Settings.</summary>
public sealed record PresignInfo(string RoleCode, string RoleLabel, string Name, string? StrokesJson)
{
    public bool HasSignature => !string.IsNullOrEmpty(StrokesJson) && !string.IsNullOrWhiteSpace(Name);
}

public sealed partial class InspectionService
{
    // Ang mga role nga pwede mag pre-sign (tanan gawas sa Owner / Representative).
    public static readonly string[] PresignRoles =
        { "INSPECTOR", "TEAM_LEADER", "BUILDING_PLAN_EVALUATOR", "CHIEF_FSES", "FIRE_MARSHAL" };

    private static string PresignNameKey(string role) => $"presign.{role}.name";
    private static string PresignFile(string role) => Path.Combine("signatures", $"presign-{role}.json");

    // SETTINGS: kuhaon tanang saved nga pirma (bisan wala pa).
    public async Task<List<PresignInfo>> GetPresignsAsync()
    {
        await using var db = await _database.OpenContextAsync();
        var labels = await db.SignatoryRoles.AsNoTracking().ToDictionaryAsync(r => r.Code, r => r.Label);

        var list = new List<PresignInfo>();
        foreach (var role in PresignRoles)
            list.Add(await ReadPresignAsync(db, role, labels.GetValueOrDefault(role, role)));
        return list;
    }

    public async Task<PresignInfo> GetPresignAsync(string role)
    {
        await using var db = await _database.OpenContextAsync();
        string label = await db.SignatoryRoles.AsNoTracking()
            .Where(r => r.Code == role).Select(r => r.Label).FirstOrDefaultAsync() ?? role;
        return await ReadPresignAsync(db, role, label);
    }

    // SETTINGS: i-save (o ilisan) ang pirma sa usa ka role.
    // Ang daan nga inspection dili maapektohan kay naa silay kaugalingon nga kopya.
    public async Task SavePresignAsync(string role, string name, string strokesJson)
    {
        string fullPath = Path.Combine(FileSystem.AppDataDirectory, PresignFile(role));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllTextAsync(fullPath, strokesJson);

        await using var db = await _database.OpenContextAsync();
        string key = PresignNameKey(role);
        var row = await db.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (row == null)
            db.AppSettings.Add(new AppSetting { Key = key, Value = name.Trim() });
        else
            row.Value = name.Trim();
        await db.SaveChangesAsync();
    }

    private static async Task<PresignInfo> ReadPresignAsync(AppDbContext db, string role, string label)
    {
        string key = PresignNameKey(role);
        string name = await db.AppSettings.AsNoTracking()
            .Where(s => s.Key == key).Select(s => s.Value).FirstOrDefaultAsync() ?? string.Empty;
        return new PresignInfo(role, label, name, ReadSignatureFile(PresignFile(role)));
    }

    // AWTOMATIK: ibutang ang saved nga pirma sa mga linya sa BFP nga pending pa (tan-awa ang lagda sa ibabaw).
    private static async Task ApplyPresignsAsync(AppDbContext db, Inspection inspection)
    {
        var pending = await db.InspectionSignatories
            .Where(s => s.InspectionId == inspection.Id && s.Status == "PENDING"
                        && s.RoleCode != "OWNER_REP" && s.RoleCode != "FIRE_MARSHAL")
            .ToListAsync();
        if (pending.Count == 0)
            return;

        var stages = await db.FormTemplateSignatories.AsNoTracking()
            .Where(r => r.TemplateId == inspection.TemplateId)
            .ToDictionaryAsync(r => r.RoleCode, r => r.Stage);

        bool formDone = inspection.Status == "DRAFT" && (await ComputeProgressAsync(db, inspection)).IsComplete;
        bool completed = inspection.Status == "COMPLETED";
        bool changed = false;

        foreach (var sig in pending)
        {
            bool onSite = stages.GetValueOrDefault(sig.RoleCode, "ON_SITE") == "ON_SITE";
            if (onSite ? !formDone : !completed)
                continue;   // dili pa panahon sa kini nga pirma

            var saved = await ReadPresignAsync(db, sig.RoleCode, sig.RoleCode);
            if (!saved.HasSignature)
                continue;   // wala pa mo-pirma sa Settings: pwede gihapon mo-pirma og mano-mano

            await StampSignatureAsync(sig, saved.Name, null, saved.StrokesJson!);
            changed = true;
        }

        if (changed)
            await db.SaveChangesAsync();
    }

    // Isulat ang pirma sa usa ka linya: kaugalingon nga file + hash + oras. (Ang caller ang mo-SaveChanges.)
    private static async Task StampSignatureAsync(InspectionSignatory sig, string printedName, string? decision, string strokesJson)
    {
        string relativePath = Path.Combine("signatures", $"{sig.Id}.json");
        string fullPath = Path.Combine(FileSystem.AppDataDirectory, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllTextAsync(fullPath, strokesJson);

        sig.PrintedName = printedName.Trim();
        sig.Status = "SIGNED";
        sig.SignaturePath = relativePath;
        // SHA-256 hash: aron mahibal-an kung naay nag-usab sa file sa pirma.
        sig.SignatureSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(strokesJson))).ToLowerInvariant();
        sig.SignedAt = DateTime.UtcNow;
        sig.Decision = decision;
    }
}

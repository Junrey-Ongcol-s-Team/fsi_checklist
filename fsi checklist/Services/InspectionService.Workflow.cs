// =====================================================================
// WORKFLOW sa inspection (bahin 2 sa InspectionService):
//   - Dashboard: ihap sa 4 ka indicator ug listahan sa inspections nga naay progress
//   - Sign-off: mga linya sa pirma, pag-save sa pirma, ug pag-Complete (lock)
// Ang status sa inspection:
//   Draft             = wala pa nahuman ang form
//   Missing signature = nahuman na ang form pero kulang pa ang required nga pirma
//   Awaiting approval = na-complete ug na-lock na, naghulat sa pirma sa Fire Marshal
//   Completed         = naka-pirma na ang Fire Marshal
// =====================================================================
using System.Security.Cryptography;
using System.Text;
using fsi_checklist.Data;
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public enum InspectionState { Draft, MissingSignature, AwaitingApproval, Completed }

public sealed record DashboardItem(
    string Id,
    string OwnerName,
    string SubjectName,
    string TemplateName,
    string? ReferenceNo,
    DateTime? DateInspected,
    InspectionState State,
    ProgressResult Progress,
    DateTime UpdatedAt);

public sealed record DashboardData(
    int Drafts, int MissingSignatures, int Completed, int AwaitingApproval, List<DashboardItem> Items);

public sealed record SignatoryInfo(
    string Id,
    string InspectionId,
    string RoleCode,
    string RoleLabel,
    string Stage,
    bool IsRequired,
    string Status,
    string PrintedName,
    string? Decision,
    string? SignaturePath,
    DateTime? SignedAt);

public sealed record SignoffData(
    string InspectionId,
    string OwnerName,
    string SubjectName,
    string TemplateName,
    string Status,
    ProgressResult Progress,
    int MissingOnSite,
    bool IsApproved,
    List<SignatoryInfo> Signatories);

/// <summary>
/// The life of an inspection:
///   Draft            - the form is not finished yet (answers autosave, you can come back any time)
///   Missing signature- the form is finished, but required signatures at the inspection are not complete
///   Awaiting approval- completed and locked; the office (chief / fire marshal) still has to sign
///   Completed        - the fire marshal has signed
/// </summary>
public sealed partial class InspectionService
{
    private static readonly IReadOnlyDictionary<(string ItemId, string SlotKey), string> NoAnswers =
        new Dictionary<(string, string), string>();

    private readonly SemaphoreSlim _signoffLock = new(1, 1);

    // ----- dashboard ----------------------------------------------------------

    // DASHBOARD: kuhaon tanang inspection, kuwentahon ang progress ug status sa matag usa, ug ihapon ang 4 ka indicator.
    public async Task<DashboardData> GetDashboardAsync()
    {
        await using var db = await _database.OpenContextAsync();

        var inspections = await db.Inspections.AsNoTracking()
            .Select(i => new
            {
                i.Id,
                i.TemplateId,
                i.Status,
                i.ReferenceNo,
                i.DateInspected,
                i.UpdatedAt,
                SubjectName = i.Subject.Name,
                OwnerName = i.Subject.OwnerName,
                TemplateName = i.Template.DisplayName,
            })
            .ToListAsync();

        if (inspections.Count == 0)
            return new DashboardData(0, 0, 0, 0, new List<DashboardItem>());

        var templateIds = inspections.Select(i => i.TemplateId).Distinct().ToList();

        var itemsByTemplate = (await db.FormItems.AsNoTracking()
                .Where(i => templateIds.Contains(i.TemplateId)).ToListAsync())
            .GroupBy(i => i.TemplateId)
            .ToDictionary(g => g.Key, g => (IReadOnlyCollection<FormItem>)g.ToList());

        var columnsBySet = (await db.FormColumns.AsNoTracking().ToListAsync())
            .GroupBy(c => c.ColumnSetId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Ang mga tubag nga naay sulod ra (dili blangko).
        var answerRows = await db.InspectionAnswers.AsNoTracking()
            .Where(a => a.ValueText != null || a.ValueNum != null)
            .Select(a => new { a.InspectionId, a.ItemId, a.SlotKey, a.ValueText })
            .ToListAsync();

        var answersByInspection = new Dictionary<string, Dictionary<(string, string), string>>();
        foreach (var a in answerRows)
        {
            if (!answersByInspection.TryGetValue(a.InspectionId, out var map))
                answersByInspection[a.InspectionId] = map = new Dictionary<(string, string), string>();
            map.TryAdd((a.ItemId, a.SlotKey), a.ValueText ?? string.Empty);
        }

        var signatories = await db.InspectionSignatories.AsNoTracking()
            .Select(s => new { s.InspectionId, s.RoleCode, s.Status })
            .ToListAsync();

        var rules = await db.FormTemplateSignatories.AsNoTracking().ToListAsync();

        var result = new List<DashboardItem>();
        int drafts = 0, missing = 0, completed = 0, awaiting = 0;

        foreach (var i in inspections)
        {
            itemsByTemplate.TryGetValue(i.TemplateId, out var templateItems);
            answersByInspection.TryGetValue(i.Id, out var answers);

            var progress = ProgressCalculator.Compute(
                templateItems ?? Array.Empty<FormItem>(), columnsBySet, answers ?? NoAnswers);

            var mine = signatories.Where(s => s.InspectionId == i.Id).ToList();

            // Kulang ba ang required nga pirma sa inspection? (Owner, Inspector, Team Leader...)
            bool missingOnSite = rules
                .Where(r => r.TemplateId == i.TemplateId && r.Stage == "ON_SITE" && r.IsRequired)
                .Any(r => mine.Count(s => s.RoleCode == r.RoleCode &&
                                          (s.Status == "SIGNED" || s.Status == "REFUSED")) < r.MinCount);

            // Approved = naka-pirma na ang Fire Marshal.
            bool approved = mine.Any(s => s.RoleCode == "FIRE_MARSHAL" && s.Status == "SIGNED");

            // Tinoon ang status (tan-awa ang pasabot sa ibabaw sa file).
            InspectionState state;
            if (i.Status == "COMPLETED")
                state = approved ? InspectionState.Completed : InspectionState.AwaitingApproval;
            else if (progress.IsComplete && missingOnSite)
                state = InspectionState.MissingSignature;
            else
                state = InspectionState.Draft;

            switch (state)
            {
                case InspectionState.Draft: drafts++; break;
                case InspectionState.MissingSignature: missing++; break;
                case InspectionState.AwaitingApproval: awaiting++; break;
                default: completed++; break;
            }

            result.Add(new DashboardItem(
                i.Id,
                string.IsNullOrWhiteSpace(i.OwnerName) ? i.SubjectName : i.OwnerName!,
                i.SubjectName,
                i.TemplateName,
                i.ReferenceNo,
                i.DateInspected,
                state,
                progress,
                i.UpdatedAt));
        }

        result = result.OrderByDescending(r => r.UpdatedAt).ToList();
        return new DashboardData(drafts, missing, completed, awaiting, result);
    }

    // ----- sign-off -------------------------------------------------------------

    // SIGN-OFF: kuhaon ang mga linya sa pirma. Kung wala pa, himoon kini base sa form (form_template_signatory).
    public async Task<SignoffData?> LoadSignoffAsync(string inspectionId)
    {
        await _signoffLock.WaitAsync();
        try
        {
            await using var db = await _database.OpenContextAsync();

            var inspection = await db.Inspections.AsNoTracking()
                .Include(i => i.Subject)
                .Include(i => i.Template)
                .FirstOrDefaultAsync(i => i.Id == inspectionId);

            if (inspection == null)
                return null;

            var rules = await db.FormTemplateSignatories.AsNoTracking()
                .Where(r => r.TemplateId == inspection.TemplateId).ToListAsync();

            await EnsureSignatoryRowsAsync(db, inspection, rules);
            await ApplyPresignsAsync(db, inspection);   // ibutang ang pre-signed nga pirma sa BFP

            var roleLabels = await db.SignatoryRoles.AsNoTracking().ToDictionaryAsync(r => r.Code, r => r.Label);
            var rows = await db.InspectionSignatories.AsNoTracking()
                .Where(s => s.InspectionId == inspectionId).ToListAsync();

            var progress = await ComputeProgressAsync(db, inspection);

            var infos = rows
                .Select(s =>
                {
                    var rule = rules.FirstOrDefault(r => r.RoleCode == s.RoleCode);
                    return new
                    {
                        Order = (rule?.Stage == "APPROVAL" ? 1000 : 0) + (rule?.SortOrder ?? 0) * 10 + s.SortOrder,
                        Info = ToInfo(s, rule?.Stage ?? "ON_SITE", rule?.IsRequired ?? false,
                            roleLabels.GetValueOrDefault(s.RoleCode, s.RoleCode)),
                    };
                })
                .OrderBy(x => x.Order)
                .Select(x => x.Info)
                .ToList();

            return new SignoffData(
                inspection.Id,
                string.IsNullOrWhiteSpace(inspection.Subject.OwnerName) ? inspection.Subject.Name : inspection.Subject.OwnerName!,
                inspection.Subject.Name,
                inspection.Template.DisplayName,
                inspection.Status,
                progress,
                CountMissingOnSite(rules, rows),
                rows.Any(s => s.RoleCode == "FIRE_MARSHAL" && s.Status == "SIGNED"),
                infos);
        }
        finally
        {
            _signoffLock.Release();
        }
    }

    public async Task<SignatoryInfo?> GetSignatoryAsync(string signatoryId)
    {
        await using var db = await _database.OpenContextAsync();

        var sig = await db.InspectionSignatories.AsNoTracking().FirstOrDefaultAsync(s => s.Id == signatoryId);
        if (sig == null)
            return null;

        var templateId = await db.Inspections.AsNoTracking()
            .Where(i => i.Id == sig.InspectionId).Select(i => i.TemplateId).FirstAsync();

        var rule = await db.FormTemplateSignatories.AsNoTracking()
            .FirstOrDefaultAsync(r => r.TemplateId == templateId && r.RoleCode == sig.RoleCode);

        string label = await db.SignatoryRoles.AsNoTracking()
            .Where(r => r.Code == sig.RoleCode).Select(r => r.Label).FirstOrDefaultAsync() ?? sig.RoleCode;

        return ToInfo(sig, rule?.Stage ?? "ON_SITE", rule?.IsRequired ?? false, label);
    }

    // I-SAVE ANG PIRMA.
    // Mga lagda: ang pirma sa inspection kinahanglan humana ang form; ang pirma sa opisina human ma-complete.
    // Kung na-pirmahan na, dili na mausab.
    /// <summary>Stores the drawn signature on a signature line. A signed line can never be changed.</summary>
    public async Task<(bool Ok, string Message)> SaveSignatureAsync(
        string signatoryId, string printedName, string? decision, string strokesJson)
    {
        await using var db = await _database.OpenContextAsync();

        var sig = await db.InspectionSignatories.FirstOrDefaultAsync(s => s.Id == signatoryId);
        if (sig == null)
            return (false, "This signature line no longer exists.");

        if (sig.Status == "SIGNED")
            return (false, "This line is already signed and cannot be changed.");

        var inspection = await db.Inspections.AsNoTracking().FirstAsync(i => i.Id == sig.InspectionId);

        var rule = await db.FormTemplateSignatories.AsNoTracking()
            .FirstOrDefaultAsync(r => r.TemplateId == inspection.TemplateId && r.RoleCode == sig.RoleCode);

        if ((rule?.Stage ?? "ON_SITE") == "ON_SITE")
        {
            if (inspection.Status != "DRAFT")
                return (false, "This inspection is already completed.");

            var progress = await ComputeProgressAsync(db, inspection);
            if (!progress.IsComplete)
                return (false, $"Finish the form first ({progress.Missing} items are still missing).");
        }
        else if (inspection.Status != "COMPLETED")
        {
            return (false, "The approval signatures come after the inspection is completed.");
        }

        await StampSignatureAsync(sig, printedName, decision, strokesJson);
        await db.SaveChangesAsync();

        await db.Inspections
            .Where(i => i.Id == inspection.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.UpdatedAt, DateTime.UtcNow));

        return (true, "Signed.");
    }

    // COMPLETE: i-lock ang inspection. Kinahanglan humana ang form ug kompleto ang required nga pirma.
    // Human niini, dili na mausab ang mga tubag (gibantayan sa trigger sa schema.sql).
    /// <summary>Locks the inspection (answers can no longer change) once everything is filled in and signed.</summary>
    public async Task<(bool Ok, string Message)> CompleteInspectionAsync(string inspectionId)
    {
        await using var db = await _database.OpenContextAsync();

        var inspection = await db.Inspections.AsNoTracking().FirstOrDefaultAsync(i => i.Id == inspectionId);
        if (inspection == null)
            return (false, "This inspection no longer exists.");

        if (inspection.Status == "COMPLETED")
            return (true, "Already completed.");

        var progress = await ComputeProgressAsync(db, inspection);
        if (!progress.IsComplete)
            return (false, $"{progress.Missing} items are still missing in the form.");

        var rules = await db.FormTemplateSignatories.AsNoTracking()
            .Where(r => r.TemplateId == inspection.TemplateId).ToListAsync();
        await ApplyPresignsAsync(db, inspection);   // pre-signed nga pirma sa inspection
        var rows = await db.InspectionSignatories.AsNoTracking()
            .Where(s => s.InspectionId == inspectionId).ToListAsync();

        int missing = CountMissingOnSite(rules, rows);
        if (missing > 0)
            return (false, $"{missing} required signature(s) are still missing.");

        await db.Inspections
            .Where(i => i.Id == inspectionId && i.Status == "DRAFT")
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Status, "COMPLETED")
                .SetProperty(i => i.CompletedAt, (DateTime?)DateTime.UtcNow)
                .SetProperty(i => i.UpdatedAt, DateTime.UtcNow));

        // Human ma-complete: ibutang ang pre-signed nga pirma sa opisina (pananglitan Chief FSES).
        inspection.Status = "COMPLETED";
        await ApplyPresignsAsync(db, inspection);

        return (true, "Inspection completed.");
    }

    public static string? ReadSignatureFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;

        string fullPath = Path.Combine(FileSystem.AppDataDirectory, relativePath);
        return File.Exists(fullPath) ? File.ReadAllText(fullPath) : null;
    }

    // ----- helpers ---------------------------------------------------------------

    // Himoon ang mga linya sa pirma nga kulang pa (usa matag role). Ang Owner/Representative kay gi-prefill na ang ngalan.
    private static async Task EnsureSignatoryRowsAsync(
        AppDbContext db, Inspection inspection, List<FormTemplateSignatory> rules)
    {
        var existing = await db.InspectionSignatories
            .Where(s => s.InspectionId == inspection.Id).ToListAsync();

        bool added = false;

        foreach (var rule in rules)
        {
            int have = existing.Count(s => s.RoleCode == rule.RoleCode);

            for (int n = have; n < Math.Max(1, rule.MinCount); n++)
            {
                db.InspectionSignatories.Add(new InspectionSignatory
                {
                    Id = NewId(),
                    InspectionId = inspection.Id,
                    RoleCode = rule.RoleCode,
                    PrintedName = rule.RoleCode == "OWNER_REP" ? inspection.Subject?.OwnerName ?? string.Empty : string.Empty,
                    Status = "PENDING",
                    SortOrder = n,
                });
                added = true;
            }
        }

        if (added)
            await db.SaveChangesAsync();
    }

    // Kuwentahon ang progress sa usa ka inspection gikan sa database.
    private static async Task<ProgressResult> ComputeProgressAsync(AppDbContext db, Inspection inspection)
    {
        var items = await db.FormItems.AsNoTracking()
            .Where(i => i.TemplateId == inspection.TemplateId).ToListAsync();

        var setIds = items.Where(i => i.ColumnSetId != null).Select(i => i.ColumnSetId!).Distinct().ToList();

        var columnsBySet = (await db.FormColumns.AsNoTracking()
                .Where(c => setIds.Contains(c.ColumnSetId)).ToListAsync())
            .GroupBy(c => c.ColumnSetId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rows = await db.InspectionAnswers.AsNoTracking()
            .Where(a => a.InspectionId == inspection.Id && (a.ValueText != null || a.ValueNum != null))
            .Select(a => new { a.ItemId, a.SlotKey, a.ValueText })
            .ToListAsync();

        var answers = new Dictionary<(string, string), string>();
        foreach (var a in rows)
            answers.TryAdd((a.ItemId, a.SlotKey), a.ValueText ?? string.Empty);

        return ProgressCalculator.Compute(items, columnsBySet, answers);
    }

    // Ihapon pila pa ka required nga pirma sa inspection ang kulang.
    private static int CountMissingOnSite(
        IEnumerable<FormTemplateSignatory> rules, IReadOnlyCollection<InspectionSignatory> rows) =>
        rules.Where(r => r.Stage == "ON_SITE" && r.IsRequired)
            .Sum(r => Math.Max(0, r.MinCount - rows.Count(s =>
                s.RoleCode == r.RoleCode && (s.Status == "SIGNED" || s.Status == "REFUSED"))));

    private static SignatoryInfo ToInfo(InspectionSignatory s, string stage, bool isRequired, string roleLabel) =>
        new(s.Id, s.InspectionId, s.RoleCode, roleLabel, stage, isRequired, s.Status, s.PrintedName,
            s.Decision, s.SignaturePath, s.SignedAt);
}

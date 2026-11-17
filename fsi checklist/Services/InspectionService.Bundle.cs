// =====================================================================
// EXPORT / IMPORT sa mga inspection (bahin 5 sa InspectionService)
//
// EXPORT: ang mga gi-tick nga inspection sa Dashboard -> usa ka .zip nga file (FSI bundle).
//   Sulod sa zip: usa ka FOLDER matag inspection (ngalan = owner_establishment_petsa):
//     manifest.json                         = unsa nga file kini (format, bersyon, listahan sa folder)
//     Juan-Cruz_ABC-Store_11-09-2026/
//        inspection.json                    = inspection, owner/establishment, mga tubag, pirma, litrato (listahan)
//        signatures/...json                 = ang mga pirma
//        photos/...jpg                      = ang mga litrato
//        Juan-Cruz_ABC-Store_...pdf         = ang PRINTOUT (BFP form) sa inspection (dili gamiton sa Import)
//   Ang zip gi-save sa FOLDER sa device (Android: Downloads/FSI Exports; Windows: Documents/FSI Exports)
//   ug gi-share (Messenger, Gmail, Drive, Bluetooth ...) gikan sa Dashboard.
//
// IMPORT: pilion ang .zip (gikan sa Downloads, Messenger/Gmail download, o Drive) -> i-add ang mga inspection.
//   - Kung naa na ang inspection (parehas nga id): laktawan (dili usbon ang naa na).
//   - Kung wala ang form (template) sa kini nga app: laktawan.
//   - Ang LOCKED nga inspection: i-lock gihapon human ma-import (ang trigger mobantay sa tubag).
// =====================================================================
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

/// <summary>Usa ka inspection sulod sa bundle.</summary>
public sealed class BundleInspection
{
    public Subject Subject { get; set; } = new();
    public Inspection Inspection { get; set; } = new();
    public List<InspectionAnswer> Answers { get; set; } = new();
    public List<InspectionSignatory> Signatories { get; set; } = new();
    public List<Attachment> Photos { get; set; } = new();
}

/// <summary>Ang "label" sa bundle (manifest.json).</summary>
public sealed class BundleManifest
{
    public string Format { get; set; } = "";
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Count { get; set; }
    public List<string> Folders { get; set; } = new();   // usa ka folder matag inspection
}

public sealed record ImportResult(int Imported, int Skipped, List<string> Notes);

/// <summary>Resulta sa Export: ang file (para sa Share) ug asa kini na-save sa device.</summary>
public sealed record ExportResult(string FilePath, string FileName, string? SavedTo);

public sealed partial class InspectionService
{
    private const string BundleFormat = "fsi-checklist-bundle";
    private const int BundleVersion = 2;   // 2 = usa ka folder matag inspection

    private static readonly JsonSerializerOptions BundleJson = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    // ----- EXPORT ------------------------------------------------------------

    /// <summary>Himoon ang .zip sa mga gipili nga inspection, i-save sa FSI Exports folder sa device.</summary>
    // makePdf: ang paghimo sa PDF (PrintService.BuildPdfAsync), gihatag sa Dashboard.
    public async Task<ExportResult> ExportBundleAsync(
        IReadOnlyCollection<string> inspectionIds,
        Func<string, Task<(string? Path, string? Error)>>? makePdf = null)
    {
        if (inspectionIds.Count == 0)
            throw new InvalidOperationException("Select at least one inspection to export.");

        await using var db = await _database.OpenContextAsync();

        var list = new List<BundleInspection>();
        foreach (var id in inspectionIds)
        {
            var inspection = await db.Inspections.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (inspection == null)
                continue;

            list.Add(new BundleInspection
            {
                Inspection = inspection,
                Subject = await db.Subjects.AsNoTracking().FirstAsync(s => s.Id == inspection.SubjectId),
                Answers = await db.InspectionAnswers.AsNoTracking().Where(a => a.InspectionId == id).ToListAsync(),
                Signatories = await db.InspectionSignatories.AsNoTracking().Where(s => s.InspectionId == id).ToListAsync(),
                Photos = await db.Attachments.AsNoTracking().Where(a => a.InspectionId == id).ToListAsync(),
            });
        }

        // Ngalan sa zip: FSI_Bundle_3-inspections_2026-11-09_1405.zip
        string plural = list.Count == 1 ? "" : "s";
        string name = $"FSI_Bundle_{list.Count}-inspection{plural}_{DateTime.Now:yyyy-MM-dd_HHmm}.zip";
        string path = Path.Combine(FileSystem.CacheDirectory, name);
        if (File.Exists(path))
            File.Delete(path);

        using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            var folders = new List<string>();
            foreach (var b in list)
            {
                // FOLDER sa inspection: owner_establishment_petsa (lahi kung parehas)
                string folder = FolderName(b);
                string unique = folder;
                for (int n = 2; folders.Contains(unique, StringComparer.OrdinalIgnoreCase); n++)
                    unique = $"{folder}_{n}";
                folders.Add(unique);

                await WriteEntryAsync(zip, $"{unique}/inspection.json", JsonSerializer.Serialize(b, BundleJson));

                // Ang mga file sa pirma ug litrato sulod sa folder sa inspection
                var files = b.Signatories.Select(x => x.SignaturePath)
                    .Concat(b.Photos.Select(p => (string?)p.FilePath))
                    .Where(p => !string.IsNullOrEmpty(p))
                    .Distinct();
                foreach (var relative in files)
                {
                    string full = Path.Combine(FileSystem.AppDataDirectory, relative!);
                    if (File.Exists(full))
                        zip.CreateEntryFromFile(full, $"{unique}/{ZipName(relative!)}");
                }

                // PDF (printout) sa inspection, sulod sa iyang folder
                if (makePdf != null)
                {
                    try
                    {
                        var (pdf, error) = await makePdf(b.Inspection.Id);
                        if (pdf != null && File.Exists(pdf))
                            zip.CreateEntryFromFile(pdf, $"{unique}/{Path.GetFileName(pdf)}");
                        else
                            AppLog.Info($"Export: no PDF for {unique}: {error}");
                    }
                    catch (Exception ex)
                    {
                        AppLog.Error(ex, $"Export: PDF failed for {unique}");
                    }
                }
            }

            var manifest = new BundleManifest
            {
                Format = BundleFormat,
                Version = BundleVersion,
                CreatedAt = DateTime.UtcNow,
                Count = list.Count,
                Folders = folders,
            };
            await WriteEntryAsync(zip, "manifest.json", JsonSerializer.Serialize(manifest, BundleJson));
        }

        // I-save usab sa FSI Exports folder sa device (aron makit-an sa ulahi)
        string? savedTo = null;
        try
        {
            savedTo = await SaveToExportsFolderAsync(path, name);
        }
        catch (Exception ex)
        {
            AppLog.Error(ex, "Couldn't save the export to the FSI Exports folder");
        }

        AppLog.Info($"Exported {list.Count} inspection(s): {path} (saved to {savedTo ?? "-"})");
        return new ExportResult(path, name, savedTo);
    }

    // Ngalan sa folder sa inspection: Juan-Cruz_ABC-Store_11-09-2026 (sama sa ngalan sa print)
    private static string FolderName(BundleInspection b)
    {
        static string Safe(string? t) =>
            System.Text.RegularExpressions.Regex.Replace(
                System.Text.RegularExpressions.Regex.Replace((t ?? "").Trim(), @"[^\w\-\. ]", "_"), @"\s+", " ")
            .Trim().Replace(' ', '-');
        static string Cut(string t) => t.Length > 40 ? t[..40].TrimEnd('-', '_', '.') : t;

        string owner = Cut(Safe(b.Subject.OwnerName));
        string place = Cut(Safe(b.Subject.Name));
        var date = b.Inspection.DateInspected ?? b.Inspection.CreatedAt;
        var parts = new[] { owner, place, date.ToString("MM-dd-yyyy") }.Where(x => x.Length > 0);
        string folder = string.Join("_", parts);
        return folder.Length > 0 ? folder : "Inspection";
    }

    // I-copy ang zip ngadto sa FSI Exports folder: Android = Downloads/FSI Exports, Windows = Documents/FSI Exports.
    // Ibalik ang lugar (para ipakita sa user), o null kung dili mahimo (pananglitan daan nga Android).
    private static async Task<string?> SaveToExportsFolderAsync(string path, string name)
    {
#if ANDROID
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
            return null;   // Android 9 pababa: Share ra

        var values = new Android.Content.ContentValues();
        values.Put("_display_name", name);          // MediaStore DISPLAY_NAME
        values.Put("mime_type", "application/zip");  // MediaStore MIME_TYPE
        values.Put("relative_path", "Download/FSI Exports");   // MediaStore RELATIVE_PATH (Downloads folder)

        var resolver = Android.App.Application.Context.ContentResolver!;
        var uri = resolver.Insert(Android.Provider.MediaStore.Downloads.ExternalContentUri, values);
        if (uri == null)
            return null;

        await using (var output = resolver.OpenOutputStream(uri)!)
        await using (var input = File.OpenRead(path))
            await input.CopyToAsync(output);

        return "Downloads / FSI Exports";
#elif WINDOWS
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FSI Exports");
        Directory.CreateDirectory(folder);
        string target = Path.Combine(folder, name);
        await using (var input = File.OpenRead(path))
        await using (var output = File.Create(target))
            await input.CopyToAsync(output);
        return folder;
#else
        await Task.CompletedTask;
        return null;
#endif
    }

    // ----- IMPORT ------------------------------------------------------------

    /// <summary>Basahon ang .zip ug i-add ang mga inspection nga wala pa sa kini nga tablet.</summary>
    public async Task<ImportResult> ImportBundleAsync(Stream source)
    {
        // I-copy una sa memory (ang stream gikan sa FilePicker dili kanunay seekable)
        using var buffer = new MemoryStream();
        await source.CopyToAsync(buffer);
        buffer.Position = 0;

        using var zip = new ZipArchive(buffer, ZipArchiveMode.Read);

        var manifest = await ReadEntryAsync<BundleManifest>(zip, "manifest.json");
        if (manifest == null || manifest.Format != BundleFormat)
            throw new InvalidOperationException("This file is not an FSI Checklist export.");
        if (manifest.Version > BundleVersion)
            throw new InvalidOperationException("This export was made by a newer version of the app. Update the app first.");
        if (manifest.Folders.Count == 0)
            throw new InvalidOperationException("This export has no inspections.");

        int imported = 0, skipped = 0;
        var notes = new List<string>();

        // Matag folder = usa ka inspection
        foreach (var folder in manifest.Folders)
        {
            var b = await ReadEntryAsync<BundleInspection>(zip, $"{folder}/inspection.json");
            if (b == null)
            {
                skipped++;
                notes.Add($"{folder}: inspection.json is missing");
                continue;
            }

            string who = string.IsNullOrWhiteSpace(b.Subject.OwnerName) ? b.Subject.Name : b.Subject.OwnerName!;
            try
            {
                string? reason = await ImportOneAsync(zip, folder, b);
                if (reason == null)
                    imported++;
                else
                {
                    skipped++;
                    notes.Add($"{who}: {reason}");
                }
            }
            catch (Exception ex)
            {
                skipped++;
                notes.Add($"{who}: {ex.Message}");
                AppLog.Error(ex, $"Import failed for inspection {b.Inspection.Id}");
            }
        }

        AppLog.Info($"Import: {imported} added, {skipped} skipped");
        return new ImportResult(imported, skipped, notes);
    }

    // Usa ka inspection: ibalik ang rason kung gi-laktawan, o null kung na-import.
    private async Task<string?> ImportOneAsync(ZipArchive zip, string folder, BundleInspection b)
    {
        await using var db = await _database.OpenContextAsync();
        var insp = b.Inspection;

        if (await db.Inspections.AnyAsync(i => i.Id == insp.Id))
            return "already on this tablet";
        if (!await db.FormTemplates.AnyAsync(t => t.Id == insp.TemplateId))
            return "this form is not in this app";

        // Ang mga pangutana ug role nga naa sa kini nga app (aron dili mo-error kung lahi ang bersyon)
        var itemIds = (await db.FormItems.Where(i => i.TemplateId == insp.TemplateId).Select(i => i.Id).ToListAsync()).ToHashSet();
        var roles = (await db.SignatoryRoles.Select(r => r.Code).ToListAsync()).ToHashSet();

        await using var tx = await db.Database.BeginTransactionAsync();

        // Subject (owner / establishment): kung naa na, gamiton ang naa
        if (!await db.Subjects.AnyAsync(s => s.Id == b.Subject.Id))
            db.Subjects.Add(b.Subject);

        // Inspection: ibutang una nga DILI locked (aron masulod ang tubag), i-lock sa ulahi
        DateTime? lockedAt = insp.LockedAt;
        insp.LockedAt = null;
        insp.Subject = null!;
        insp.Template = null!;
        db.Inspections.Add(insp);
        await db.SaveChangesAsync();   // una ang subject ug inspection, dayon ang mga tubag (para sa foreign key)

        int dropped = 0;
        foreach (var a in b.Answers)
        {
            if (!itemIds.Contains(a.ItemId)) { dropped++; continue; }
            a.Id = 0;   // bag-ong numero sa kini nga database
            db.InspectionAnswers.Add(a);
        }

        foreach (var s in b.Signatories.Where(s => roles.Contains(s.RoleCode)))
        {
            s.SignaturePath = await CopyBundleFileAsync(zip, folder, s.SignaturePath);
            db.InspectionSignatories.Add(s);
        }

        foreach (var p in b.Photos)
        {
            p.FilePath = await CopyBundleFileAsync(zip, folder, p.FilePath) ?? p.FilePath;
            if (p.ItemId != null && !itemIds.Contains(p.ItemId))
                p.ItemId = null;
            db.Attachments.Add(p);
        }

        await db.SaveChangesAsync();

        if (lockedAt != null)
        {
            await db.Inspections.Where(i => i.Id == insp.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(i => i.LockedAt, lockedAt));
        }

        await tx.CommitAsync();

        if (dropped > 0)
            AppLog.Info($"Import {insp.Id}: {dropped} answer(s) skipped (question not in this app version)");
        return null;
    }

    // I-copy ang file sa pirma/litrato gikan sa zip ngadto sa app data folder. Ibalik ang bag-ong relative path.
    private static async Task<string?> CopyBundleFileAsync(ZipArchive zip, string folder, string? relative)
    {
        if (string.IsNullOrEmpty(relative))
            return relative;

        var entry = zip.GetEntry($"{folder}/{ZipName(relative)}");
        // Ang path sumala sa kini nga device (Windows "\" o Android "/")
        string local = Path.Combine(relative.Split('/', '\\', StringSplitOptions.RemoveEmptyEntries));
        if (entry == null)
            return local;

        string full = Path.Combine(FileSystem.AppDataDirectory, local);
        if (!File.Exists(full))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            await using var input = entry.Open();
            await using var output = File.Create(full);
            await input.CopyToAsync(output);
        }
        return local;
    }

    // ----- helpers ----------------------------------------------------------

    // Ang ngalan sa sulod sa zip kanunay "/" (pananglitan signatures/abc.json)
    private static string ZipName(string relative) => relative.Replace('\\', '/').TrimStart('/');

    private static async Task WriteEntryAsync(ZipArchive zip, string name, string text)
    {
        var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
        await using var writer = new StreamWriter(entry.Open());
        await writer.WriteAsync(text);
    }

    private static async Task<T?> ReadEntryAsync<T>(ZipArchive zip, string name)
    {
        var entry = zip.GetEntry(name);
        if (entry == null)
            return default;
        await using var stream = entry.Open();
        return await JsonSerializer.DeserializeAsync<T>(stream, BundleJson);
    }
}

// =====================================================================
// LITRATO (bahin 3 sa InspectionService): pag-add, pag-kuha ug pag-remove sa mga litrato.
//   - ang file gi-copy sa "photos" folder tupad sa database
//   - ang path gi-save sa table nga "attachment"
//   - dili na ma-add o ma-remove kung COMPLETED na ang inspection
// =====================================================================
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public sealed partial class InspectionService
{
    // Tanang litrato sa usa ka inspection.
    public async Task<List<Attachment>> GetPhotosAsync(string inspectionId)
    {
        await using var db = await _database.OpenContextAsync();
        return await db.Attachments.AsNoTracking()
            .Where(a => a.InspectionId == inspectionId)
            .OrderBy(a => a.TakenAt)
            .ToListAsync();
    }

    // I-copy ang litrato ngadto sa photos folder, dayon i-save sa database.
    public async Task<Attachment> AddPhotoAsync(string inspectionId, string itemId, string? tag, FileResult photo)
    {
        await using var db = await _database.OpenContextAsync();

        string status = await db.Inspections.Where(i => i.Id == inspectionId).Select(i => i.Status).FirstAsync();
        if (status != "DRAFT")
            throw new InvalidOperationException("This inspection is completed. Photos can no longer be changed.");

        string id = NewId();
        string ext = Path.GetExtension(photo.FileName);
        if (string.IsNullOrEmpty(ext))
            ext = ".jpg";

        string relativePath = Path.Combine("photos", id + ext);
        string fullPath = PhotoFullPath(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using (var source = await photo.OpenReadAsync())
        await using (var target = File.Create(fullPath))
            await source.CopyToAsync(target);

        var row = new Attachment
        {
            Id = id,
            InspectionId = inspectionId,
            ItemId = itemId,
            Kind = "PHOTO",
            FilePath = relativePath,
            Tag = tag,
            TakenAt = DateTime.UtcNow,
        };
        db.Attachments.Add(row);
        await db.SaveChangesAsync();

        AppLog.Info($"Photo added: {relativePath} (item {itemId}, tag {tag})");
        return row;
    }

    // Tangtangon ang litrato (sa database ug ang file).
    public async Task RemovePhotoAsync(string attachmentId)
    {
        await using var db = await _database.OpenContextAsync();

        var row = await db.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId);
        if (row == null)
            return;

        string status = await db.Inspections.Where(i => i.Id == row.InspectionId).Select(i => i.Status).FirstAsync();
        if (status != "DRAFT")
            throw new InvalidOperationException("This inspection is completed. Photos can no longer be changed.");

        db.Attachments.Remove(row);
        await db.SaveChangesAsync();

        string fullPath = PhotoFullPath(row.FilePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        AppLog.Info($"Photo removed: {row.FilePath}");
    }

    // Ang tibuok path sa litrato sa telepono/computer.
    public static string PhotoFullPath(string relativePath) =>
        Path.Combine(FileSystem.AppDataDirectory, relativePath);
}

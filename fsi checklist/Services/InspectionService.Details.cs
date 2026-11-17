// =====================================================================
// EDIT DETAILS (bahin 6 sa InspectionService): ang UNANG gi-fill up sa bag-ong inspection
// (ngalan sa establishment / plate no., address, owner, contact number, IO no., petsa)
// pwede usbon sa ulahi, gawas kung LOCKED na ang inspection.
// =====================================================================
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public sealed record InspectionDetailsData(
    string InspectionId,
    string TemplateName,
    string SubjectType,
    string SubjectName,
    string Address,
    string OwnerName,
    string ContactNo,
    string ReferenceNo,
    DateTime DateInspected,
    bool IsLocked);

public sealed partial class InspectionService
{
    public async Task<InspectionDetailsData?> GetDetailsAsync(string inspectionId)
    {
        await using var db = await _database.OpenContextAsync();
        var i = await db.Inspections.AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.Template)
            .FirstOrDefaultAsync(x => x.Id == inspectionId);
        if (i == null)
            return null;

        return new InspectionDetailsData(
            i.Id,
            i.Template.DisplayName,
            i.Subject.SubjectType,
            i.Subject.Name,
            i.Subject.Address ?? string.Empty,
            i.Subject.OwnerName ?? string.Empty,
            i.Subject.ContactNo ?? string.Empty,
            i.ReferenceNo ?? string.Empty,
            i.DateInspected ?? DateTime.Today,
            i.LockedAt != null);
    }

    // I-save ang giusab nga detalye. Dili pwede kung LOCKED.
    public async Task<(bool Ok, string Message)> UpdateDetailsAsync(InspectionDetailsData d)
    {
        if (string.IsNullOrWhiteSpace(d.SubjectName))
            return (false, "The name can't be empty.");

        await using var db = await _database.OpenContextAsync();
        var inspection = await db.Inspections.FirstOrDefaultAsync(x => x.Id == d.InspectionId);
        if (inspection == null)
            return (false, "This inspection no longer exists.");
        if (inspection.LockedAt != null)
            return (false, "This inspection is locked, so its details can no longer be edited.");

        var subject = await db.Subjects.FirstAsync(s => s.Id == inspection.SubjectId);
        var now = DateTime.UtcNow;

        // Kung ang subject gigamit sa LAIN nga inspection: himoan og kaugalingong kopya,
        // aron dili mausab ang detalye sa laing inspection (pananglitan ang locked).
        bool shared = await db.Inspections.AnyAsync(x => x.SubjectId == subject.Id && x.Id != inspection.Id);
        if (shared)
        {
            subject = new Subject
            {
                Id = NewId(),
                SubjectType = subject.SubjectType,
                CreatedAt = now,
            };
            db.Subjects.Add(subject);
            inspection.SubjectId = subject.Id;
        }

        subject.Name = d.SubjectName.Trim();
        subject.Address = NullIfBlank(d.Address);
        subject.OwnerName = NullIfBlank(d.OwnerName);
        subject.ContactNo = NullIfBlank(d.ContactNo);
        subject.UpdatedAt = now;

        inspection.ReferenceNo = NullIfBlank(d.ReferenceNo);
        inspection.DateInspected = d.DateInspected.Date;
        inspection.UpdatedAt = now;

        await db.SaveChangesAsync();
        AppLog.Info($"Details updated: inspection {inspection.Id}");
        return (true, "Details saved.");
    }
}

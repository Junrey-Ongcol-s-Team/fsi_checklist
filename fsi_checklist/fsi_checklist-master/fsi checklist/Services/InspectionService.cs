using System.Globalization;
using fsi_checklist.Data.Models;
using fsi_checklist.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public sealed record AnswerWrite(string ItemId, string SlotKey, string? Text, double? Num);

public sealed record NewInspectionRequest(
    string TemplateId,
    string SubjectName,
    string? Address,
    string? OwnerName,
    string? ContactNo,
    string? ReferenceNo,
    DateTime DateInspected);

public sealed class FormData
{
    public required Inspection Inspection { get; init; }
    public required List<FormSection> Sections { get; init; }
    public required List<FormItem> Items { get; init; }
    public required List<FormColumn> Columns { get; init; }
    public required List<FormChoice> Choices { get; init; }
    public required List<InspectionAnswer> Answers { get; init; }
}

public sealed class InspectionService
{
    private readonly DatabaseService _database;

    public InspectionService(DatabaseService database)
    {
        _database = database;
    }

    // ----- list -----------------------------------------------------------

    public async Task<List<InspectionListItem>> GetInspectionsAsync(string? search, string statusFilter)
    {
        await using var db = await _database.OpenContextAsync();

        IQueryable<Inspection> query = db.Inspections.AsNoTracking();

        if (statusFilter is "DRAFT" or "COMPLETED")
            query = query.Where(i => i.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search.Trim()}%";
            query = query.Where(i =>
                EF.Functions.Like(i.Subject.Name, pattern) ||
                (i.ReferenceNo != null && EF.Functions.Like(i.ReferenceNo, pattern)));
        }

        var rows = await query
            .OrderByDescending(i => i.UpdatedAt)
            .Select(i => new
            {
                i.Id,
                SubjectName = i.Subject.Name,
                TemplateName = i.Template.DisplayName,
                i.Status,
                i.DateInspected,
                i.ReferenceNo,
            })
            .ToListAsync();

        return rows.Select(r =>
        {
            var parts = new List<string>();
            if (r.DateInspected is { } date)
                parts.Add(date.ToString("d MMM yyyy", CultureInfo.CurrentCulture));
            if (!string.IsNullOrWhiteSpace(r.ReferenceNo))
                parts.Add($"Ref. {r.ReferenceNo}");

            return new InspectionListItem
            {
                Id = r.Id,
                SubjectName = r.SubjectName,
                TemplateDisplayName = r.TemplateName,
                IsCompleted = r.Status == "COMPLETED",
                DetailLine = parts.Count > 0 ? string.Join(" · ", parts) : "No date or reference",
            };
        }).ToList();
    }

    /// <summary>Deletes a draft inspection. Completed ones are locked and are left alone.</summary>
    public async Task<bool> DeleteDraftAsync(string inspectionId)
    {
        await using var db = await _database.OpenContextAsync();

        string? subjectId = await db.Inspections
            .Where(i => i.Id == inspectionId && i.Status == "DRAFT")
            .Select(i => i.SubjectId)
            .FirstOrDefaultAsync();

        if (subjectId == null)
            return false;

        // inspection_answer / finding / signatory / attachment rows go with it (ON DELETE CASCADE).
        int deleted = await db.Inspections
            .Where(i => i.Id == inspectionId && i.Status == "DRAFT")
            .ExecuteDeleteAsync();

        // Drop the subject too if nothing else points at it.
        await db.Subjects
            .Where(s => s.Id == subjectId && !db.Inspections.Any(i => i.SubjectId == s.Id))
            .ExecuteDeleteAsync();

        return deleted > 0;
    }

    // ----- new inspection ---------------------------------------------------

    /// <summary>Active checklists that actually have items loaded.</summary>
    public async Task<List<FormTemplate>> GetAvailableTemplatesAsync()
    {
        await using var db = await _database.OpenContextAsync();
        return await db.FormTemplates
            .AsNoTracking()
            .Where(t => t.IsActive && db.FormItems.Any(i => i.TemplateId == t.Id))
            .OrderBy(t => t.SortOrder)
            .ToListAsync();
    }

    public async Task<string> CreateInspectionAsync(NewInspectionRequest request)
    {
        await using var db = await _database.OpenContextAsync();

        var template = await db.FormTemplates.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId)
            ?? throw new InvalidOperationException("Checklist not found.");

        var now = DateTime.UtcNow;

        var subject = new Subject
        {
            Id = NewId(),
            SubjectType = template.SubjectType,
            Name = request.SubjectName.Trim(),
            Address = NullIfBlank(request.Address),
            OwnerName = NullIfBlank(request.OwnerName),
            ContactNo = NullIfBlank(request.ContactNo),
            CreatedAt = now,
            UpdatedAt = now,
        };

        var inspection = new Inspection
        {
            Id = NewId(),
            SubjectId = subject.Id,
            TemplateId = template.Id,
            ReferenceNo = NullIfBlank(request.ReferenceNo),
            DateInspected = request.DateInspected.Date,
            Status = "DRAFT",
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Subjects.Add(subject);
        db.Inspections.Add(inspection);
        await db.SaveChangesAsync();

        return inspection.Id;
    }

    // ----- filling a form ----------------------------------------------------

    public async Task<FormData?> LoadFormAsync(string inspectionId)
    {
        await using var db = await _database.OpenContextAsync();

        var inspection = await db.Inspections
            .AsNoTracking()
            .Include(i => i.Subject)
            .Include(i => i.Template)
            .FirstOrDefaultAsync(i => i.Id == inspectionId);

        if (inspection == null)
            return null;

        string templateId = inspection.TemplateId;

        var sections = await db.FormSections.AsNoTracking()
            .Where(s => s.TemplateId == templateId).ToListAsync();

        var items = await db.FormItems.AsNoTracking()
            .Where(i => i.TemplateId == templateId).ToListAsync();

        var itemIds = items.Select(i => i.Id).ToList();
        var columnSetIds = items
            .Where(i => i.ColumnSetId != null)
            .Select(i => i.ColumnSetId!)
            .Distinct()
            .ToList();

        var columns = await db.FormColumns.AsNoTracking()
            .Where(c => columnSetIds.Contains(c.ColumnSetId)).ToListAsync();

        var columnIds = columns.Select(c => c.Id).ToList();

        var choices = await db.FormChoices.AsNoTracking()
            .Where(c => (c.ItemId != null && itemIds.Contains(c.ItemId!)) ||
                        (c.ColumnId != null && columnIds.Contains(c.ColumnId!)))
            .ToListAsync();

        var answers = await db.InspectionAnswers.AsNoTracking()
            .Where(a => a.InspectionId == inspectionId).ToListAsync();

        return new FormData
        {
            Inspection = inspection,
            Sections = sections,
            Items = items,
            Columns = columns,
            Choices = choices,
            Answers = answers,
        };
    }

    /// <summary>Upserts a batch of answers; a null text and number removes the answer.</summary>
    public async Task SaveAnswersAsync(string inspectionId, IReadOnlyList<AnswerWrite> writes)
    {
        if (writes.Count == 0)
            return;

        await using var db = await _database.OpenContextAsync();

        var itemIds = writes.Select(w => w.ItemId).Distinct().ToList();
        var existing = await db.InspectionAnswers
            .Where(a => a.InspectionId == inspectionId && itemIds.Contains(a.ItemId))
            .ToListAsync();

        foreach (var write in writes)
        {
            var row = existing.FirstOrDefault(a =>
                a.ItemId == write.ItemId && a.SlotKey == write.SlotKey && a.RepeatNo == 0);

            bool isEmpty = write.Text == null && write.Num == null;

            if (isEmpty)
            {
                if (row != null)
                    db.InspectionAnswers.Remove(row);
                continue;
            }

            if (row == null)
            {
                db.InspectionAnswers.Add(new InspectionAnswer
                {
                    InspectionId = inspectionId,
                    ItemId = write.ItemId,
                    SlotKey = write.SlotKey,
                    RepeatNo = 0,
                    ValueText = write.Text,
                    ValueNum = write.Num,
                });
            }
            else
            {
                row.ValueText = write.Text;
                row.ValueNum = write.Num;
            }
        }

        await db.SaveChangesAsync();

        await db.Inspections
            .Where(i => i.Id == inspectionId)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.UpdatedAt, DateTime.UtcNow));
    }

    private static string NewId() => Guid.NewGuid().ToString();

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

// =====================================================================
// Ang mga C# class nga katumbas sa mga table sa schema.sql.
// Usa ka class = usa ka table. Usa ka property = usa ka column.
// =====================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fsi_checklist.Data.Models;

// Entities map 1:1 onto Resources/Raw/schema.sql. Column names are converted from
// PascalCase to snake_case in AppDbContext, table names are given explicitly here.

[Table("form_template")]
public sealed class FormTemplate
{
    [Key] public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Revision { get; set; } = string.Empty;
    public string? RevisionDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string SubjectType { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

[Table("form_section")]
public sealed class FormSection
{
    [Key] public string Id { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? ShowIfItemId { get; set; }
    public string? ShowIfValues { get; set; }
}

[Table("form_column")]
public sealed class FormColumn
{
    [Key] public string Id { get; set; } = string.Empty;
    public string ColumnSetId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

[Table("form_item")]
public sealed class FormItem
{
    [Key] public string Id { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string SectionId { get; set; } = string.Empty;
    public string? ParentItemId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string DataType { get; set; } = "NONE";
    public string? Unit { get; set; }
    public string? ColumnSetId { get; set; }
    public bool IsRequired { get; set; }
    public bool IsRepeating { get; set; }
    public string? HelpText { get; set; }
    public string? ShowIfItemId { get; set; }
    public string? ShowIfValues { get; set; }
}

[Table("form_choice")]
public sealed class FormChoice
{
    [Key] public string Id { get; set; } = string.Empty;
    public string? ItemId { get; set; }
    public string? ColumnId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

[Table("subject")]
public sealed class Subject
{
    [Key] public string Id { get; set; } = string.Empty;
    public string SubjectType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? OwnerName { get; set; }
    public string? ContactNo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Usa ka inspection. Ang Status kay DRAFT o COMPLETED.
[Table("inspection")]
public sealed class Inspection
{
    [Key] public string Id { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string? ReferenceNo { get; set; }
    public DateTime? ReferenceDate { get; set; }
    public DateTime? DateInspected { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Status { get; set; } = "DRAFT";
    public DateTime? CompletedAt { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Subject Subject { get; set; } = null!;
    public FormTemplate Template { get; set; } = null!;
}

// Usa ka tubag sa usa ka slot sa usa ka pangutana.
[Table("inspection_answer")]
public sealed class InspectionAnswer
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string InspectionId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string SlotKey { get; set; } = "value";
    public int RepeatNo { get; set; }
    public string? RepeatLabel { get; set; }
    public string? ValueText { get; set; }
    public double? ValueNum { get; set; }
}

[Table("app_setting")]
public sealed class AppSetting
{
    [Key] public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}

[Table("signatory_role")]
public sealed class SignatoryRole
{
    [Key] public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

// Composite key (TemplateId, RoleCode) is set in AppDbContext.
// Kinsa ang kinahanglan mo-pirma sa matag form, ug kanus-a (ON_SITE = sa inspection, APPROVAL = sa opisina).
[Table("form_template_signatory")]
public sealed class FormTemplateSignatory
{
    public string TemplateId { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string Stage { get; set; } = "ON_SITE";   // ON_SITE = at the inspection, APPROVAL = later at the office
    public bool IsRequired { get; set; }
    public int MinCount { get; set; } = 1;
    public int SortOrder { get; set; }
}

// Usa ka linya sa pirma: kinsa (role), ngalan, status (PENDING/SIGNED), file sa pirma, ug oras.
[Table("inspection_signatory")]
public sealed class InspectionSignatory
{
    [Key] public string Id { get; set; } = string.Empty;
    public string InspectionId { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string? PersonnelId { get; set; }
    public string PrintedName { get; set; } = string.Empty;
    public string? PositionTitle { get; set; }
    public string Status { get; set; } = "PENDING";   // PENDING, SIGNED, REFUSED
    public string? SignaturePath { get; set; }        // relative to the app data folder
    public string? SignatureSha256 { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? RefusedReason { get; set; }
    public string? Decision { get; set; }             // APPROVED / DISAPPROVED
    public int SortOrder { get; set; }
}

// Usa ka litrato nga gi-attach sa inspection. Ang file mismo naa sa "photos" folder; diri ang path ra.
[Table("attachment")]
public sealed class Attachment
{
    [Key] public string Id { get; set; } = string.Empty;
    public string InspectionId { get; set; } = string.Empty;
    public string? ItemId { get; set; }      // asa nga pangutana
    public string? FindingId { get; set; }
    public string Kind { get; set; } = "PHOTO";
    public string FilePath { get; set; } = string.Empty;   // relative sa app data folder, pananglitan photos/abc.jpg
    public string? Caption { get; set; }
    public string? Tag { get; set; }         // pananglitan FRONT, REAR, SIDE (064)
    public DateTime TakenAt { get; set; }
}

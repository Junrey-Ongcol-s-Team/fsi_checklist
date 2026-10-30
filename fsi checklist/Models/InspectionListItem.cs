// =====================================================================
// Usa ka row sa DAAN nga listahan (InspectionPage). Ang bag-ong dashboard naggamit na sa DashboardRow.
// =====================================================================
namespace fsi_checklist.Models;

public sealed class InspectionListItem
{
    public string Id { get; init; } = string.Empty;
    public string SubjectName { get; init; } = string.Empty;
    public string TemplateDisplayName { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public string StatusText => IsCompleted ? "Completed" : "Draft";
    public string DetailLine { get; init; } = string.Empty;
    public string AccessibilityText => $"{SubjectName}, {TemplateDisplayName}, {StatusText}";
}

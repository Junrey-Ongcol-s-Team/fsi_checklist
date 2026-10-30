// =====================================================================
// PAGE 2 (Dashboard) - ViewModel.
//   - 4 ka indicator (Drafts, Missing signature, Completed, Awaiting approval) - i-tap aron mo-filter
//   - search bar - mangita sa owner, business, plate no., form, reference o status
//   - listahan: titulo = owner, ubos = unsang inspection, progress bar (green = humana, yellow = naay kulang)
// =====================================================================
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

/// <summary>One inspection card on the dashboard.</summary>
public sealed class DashboardRow
{
    private static readonly Color Green = Color.FromArgb("#2E7D32");
    private static readonly Color Yellow = Color.FromArgb("#F9A825");

    // Himoon ang card: titulo = owner, ubos = form ug business, detalye = petsa ug reference.
    public DashboardRow(DashboardItem item)
    {
        Id = item.Id;
        State = item.State;
        Title = item.OwnerName;
        SubTitle = string.Equals(item.OwnerName, item.SubjectName, StringComparison.OrdinalIgnoreCase)
            ? item.TemplateName
            : $"{item.TemplateName} · {item.SubjectName}";

        var parts = new List<string>();
        if (item.DateInspected is { } date)
            parts.Add(date.ToString("d MMM yyyy"));
        if (!string.IsNullOrWhiteSpace(item.ReferenceNo))
            parts.Add($"Ref. {item.ReferenceNo}");
        DetailLine = parts.Count > 0 ? string.Join(" · ", parts) : "No date or reference";

        Fraction = item.Progress.Fraction;
        bool complete = item.Progress.IsComplete;
        // Green kung humana, yellow kung naay kulang pa.
        ProgressColor = complete ? Green : Yellow;
        ProgressText = item.Progress.Total == 0
            ? "No checklist items"
            : complete
                ? $"All {item.Progress.Total} items done"
                : $"{item.Progress.Done} of {item.Progress.Total} done · {item.Progress.Missing} missing";

        (StateText, StateColor) = State switch
        {
            InspectionState.Draft => ("Draft", Color.FromArgb("#FFE08A")),
            InspectionState.MissingSignature => ("Missing signature", Color.FromArgb("#FFCC80")),
            InspectionState.AwaitingApproval => ("Awaiting approval", Color.FromArgb("#90CAF9")),
            _ => ("Completed", Color.FromArgb("#A8E6B0")),
        };

        FormIsFinished = complete;
        // Tanang text nga pwede pangitaon sa search bar, gamay nga letra.
        SearchBlob = $"{item.OwnerName} {item.SubjectName} {item.TemplateName} {item.ReferenceNo} {StateText}".ToLowerInvariant();
    }

    public string Id { get; }
    public InspectionState State { get; }
    public string Title { get; }
    public string SubTitle { get; }
    public string DetailLine { get; }
    public string StateText { get; }
    public Color StateColor { get; }
    public double Fraction { get; }
    public Color ProgressColor { get; }
    public string ProgressText { get; }
    public bool FormIsFinished { get; }
    public string SearchBlob { get; }
    public bool CanDelete => State is InspectionState.Draft or InspectionState.MissingSignature;
    public string AccessibilityText => $"{Title}, {SubTitle}, {StateText}, {ProgressText}";
}

public sealed partial class DashboardPageViewModel : ObservableObject
{
    private readonly InspectionService _inspections;
    private List<DashboardRow> _all = new();

    public DashboardPageViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    public ObservableCollection<DashboardRow> Items { get; } = new();

    [ObservableProperty] private int drafts;
    [ObservableProperty] private int missingSignatures;
    [ObservableProperty] private int completed;
    [ObservableProperty] private int awaitingApproval;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EmptyText))]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EmptyText))]
    private string selectedFilter = "ALL";

    [ObservableProperty]
    private bool isRefreshing;

    // True kung walay makita sa listahan (para sa "walay sulod" nga mensahe).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private bool isEmpty = true;

    // Ang listahan makita ra kung naay sulod (sa Windows, motabon ang listahan sa mensahe).
    public bool HasItems => !IsEmpty;

    public string EmptyText =>
        !string.IsNullOrWhiteSpace(SearchText) ? "No inspections match your search."
        : SelectedFilter switch
        {
            "DRAFT" => "No drafts.",
            "MISSING" => "Nothing is waiting for a signature.",
            "COMPLETED" => "No completed inspections yet.",
            "AWAITING" => "Nothing is waiting for approval.",
            _ => "No inspections yet. Tap “New inspection” to start.",
        };

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedFilterChanged(string value) => ApplyFilter();

    // Kuhaon ang data gikan sa database, i-update ang 4 ka numero, ug i-filter ang listahan.
    // GUARD: para dili mag-doble ang pag-load (pananglitan OnAppearing ug OnInspectionIdChanged
    // parehas mo-tawag). Ang ikaduha nga tawag laktawan ra.
    private bool _loading;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (_loading)
            return;

        _loading = true;
        try { await LoadCoreAsync(); }
        finally { _loading = false; }
    }

    private async Task LoadCoreAsync()
    {
        try
        {
            var data = await _inspections.GetDashboardAsync();
            Drafts = data.Drafts;
            MissingSignatures = data.MissingSignatures;
            Completed = data.Completed;
            AwaitingApproval = data.AwaitingApproval;
            _all = data.Items.Select(i => new DashboardRow(i)).ToList();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't load the dashboard", AppLog.Describe(ex));
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try { await LoadAsync(); }
        finally { IsRefreshing = false; }
    }

    // I-filter ang listahan base sa gipiling indicator ug sa gi-type sa search bar.
    private void ApplyFilter()
    {
        IEnumerable<DashboardRow> query = _all;

        query = SelectedFilter switch
        {
            "DRAFT" => query.Where(r => r.State == InspectionState.Draft),
            "MISSING" => query.Where(r => r.State == InspectionState.MissingSignature),
            "COMPLETED" => query.Where(r => r.State == InspectionState.Completed),
            "AWAITING" => query.Where(r => r.State == InspectionState.AwaitingApproval),
            _ => query,
        };

        string search = (SearchText ?? string.Empty).Trim().ToLowerInvariant();
        if (search.Length > 0)
            query = query.Where(r => r.SearchBlob.Contains(search));

        Items.Clear();
        foreach (var row in query)
            Items.Add(row);

        IsEmpty = Items.Count == 0;
    }

    /// <summary>Tapping an indicator filters the list; tapping it again shows everything.</summary>
    [RelayCommand]
    private void SetFilter(string? filter) =>
        SelectedFilter = string.IsNullOrEmpty(filter) || SelectedFilter == filter ? "ALL" : filter;

    // Pag-tap sa card: kung draft nga wala pa humana -> ablihan ang form (mabalikan); kung dili -> sign-off page.
    [RelayCommand]
    private async Task OpenAsync(DashboardRow? row)
    {
        if (row == null)
            return;

        // An unfinished form opens for filling in; everything else opens the sign-off page.
        string route = row.State == InspectionState.Draft && !row.FormIsFinished
            ? $"{nameof(FormPage)}?inspectionId={Uri.EscapeDataString(row.Id)}"
            : $"{nameof(SignoffPage)}?inspectionId={Uri.EscapeDataString(row.Id)}";

        await Nav.GoAsync(route);
    }

    // Swipe -> Delete: pangutan-on una, dayon i-delete ang draft.
    [RelayCommand]
    private async Task DeleteAsync(DashboardRow? row)
    {
        if (row == null || !row.CanDelete)
            return;

        bool confirmed = await Dialogs.ConfirmAsync(
            "Delete this draft?", $"“{row.Title}” and everything filled in so far will be deleted.");
        if (!confirmed)
            return;

        try
        {
            await _inspections.DeleteDraftAsync(row.Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't delete", AppLog.Describe(ex));
        }
    }

    [RelayCommand]
    private Task NewInspectionAsync() => Nav.GoAsync(nameof(ChecklistTypesPage));
}

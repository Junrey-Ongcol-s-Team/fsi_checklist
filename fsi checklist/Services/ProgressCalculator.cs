// =====================================================================
// PROGRESS BAR: kuwentahon kung pila na ang natubag ug pila pa ang kulang.
// Gamiton sa duha ka lugar: sa form (live samtang nag-type) ug sa dashboard (gikan sa database).
// Ang giihap nga 'kinahanglan': Yes/No, Passed/Failed, mga pilian (CHOICE), ug mga item nga naay * (required).
// Dili giihap: remarks, checkbox, ug optional nga text.
// =====================================================================
using fsi_checklist.Data.Models;

namespace fsi_checklist.Services;

public readonly record struct ProgressResult(int Done, int Total)
{
    public int Missing => Total - Done;
    public bool IsComplete => Done >= Total;
    public double Fraction => Total == 0 ? 1.0 : (double)Done / Total;
}

/// <summary>
/// Decides which answers a form needs before it can be completed, and how many are filled in.
/// The same rules are used by the form screen (live) and by the dashboard (from the database).
/// </summary>
public static class ProgressCalculator
{
    private static readonly HashSet<string> ChoiceTypes = new() { "YES_NO", "YES_NO_NA", "PASS_FAIL", "CHOICE" };

    // Ang main nga tubag sa item: giihap kung Yes/No, Passed/Failed, CHOICE, o kung naay * (required).
    /// <summary>The main answer of an item: choices always count, other types only when the item is marked required.</summary>
    public static bool ValueSlotCounts(FormItem item) =>
        item.DataType != "NONE" && item.DataType != "BOOL" && item.DataType != "PHOTO"
        && (ChoiceTypes.Contains(item.DataType) || item.IsRequired);

    // Mga column (pananglitan Installed? / Compliant): giihap ang mga pilian, dili ang remarks.
    /// <summary>Extra columns (Yes/No, Passed/Failed...) count; free-text remarks do not.</summary>
    public static bool ColumnSlotCounts(FormColumn column) => ChoiceTypes.Contains(column.DataType);

    /// <param name="answers">(item id, slot key) of every answered slot; the value is its stored text ("" for numbers).</param>
    public static ProgressResult Compute(
        IReadOnlyCollection<FormItem> items,
        IReadOnlyDictionary<string, List<FormColumn>> columnsBySet,
        IReadOnlyDictionary<(string ItemId, string SlotKey), string> answers)
    {
        var byId = items.ToDictionary(i => i.Id);
        var visibleCache = new Dictionary<string, bool>();

        // Ang nakatago nga pangutana (show_if) dili iapil sa ihap.
        // Pananglitan sa LPG: ang Two-wheeled nga pangutana makita ra kung Two-wheeled ang gipili.
        bool IsVisible(FormItem item, int depth = 0)
        {
            if (visibleCache.TryGetValue(item.Id, out bool cached))
                return cached;

            bool visible = true;

            if (depth < 20)
            {
                if (!string.IsNullOrEmpty(item.ShowIfItemId))
                {
                    visible = answers.TryGetValue((item.ShowIfItemId, "value"), out var current)
                        && (item.ShowIfValues ?? string.Empty)
                            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                            .Contains(current, StringComparer.OrdinalIgnoreCase);
                }

                if (visible && item.ParentItemId != null && byId.TryGetValue(item.ParentItemId, out var parent))
                    visible = IsVisible(parent, depth + 1);
            }

            visibleCache[item.Id] = visible;
            return visible;
        }

        // total = pila ang kinahanglan, done = pila na ang natubag.
        int total = 0, done = 0;

        foreach (var item in items)
        {
            if (!IsVisible(item))
                continue;

            if (ValueSlotCounts(item))
            {
                total++;
                if (answers.ContainsKey((item.Id, "value")))
                    done++;
            }

            if (item.ColumnSetId != null && columnsBySet.TryGetValue(item.ColumnSetId, out var columns))
            {
                foreach (var column in columns)
                {
                    if (!ColumnSlotCounts(column))
                        continue;

                    total++;
                    if (answers.ContainsKey((item.Id, column.Key)))
                        done++;
                }
            }
        }

        return new ProgressResult(done, total);
    }
}

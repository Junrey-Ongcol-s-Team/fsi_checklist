// =====================================================================
// SHOW_IF: lagda kung kanus-a mogawas ang pangutana.
//   show_if_values = "A,B"                     -> tan-awon ang tubag (value) sa trigger item
//   show_if_values = "change_status:WITH_CHANGES" -> tan-awon ang COLUMN nga change_status sa trigger item
// Gamiton sa Form, Progress, Results ug Print aron pareho ang resulta.
// =====================================================================
namespace fsi_checklist.Services;

public static class ShowIfRule
{
    // Ibulag ang column (slot) ug ang listahan sa tubag nga mopagawas sa pangutana.
    public static (string Slot, string[] Values) Parse(string? showIfValues)
    {
        string text = showIfValues ?? string.Empty;
        string slot = "value";

        int colon = text.IndexOf(':');
        if (colon > 0)
        {
            slot = text[..colon].Trim();
            text = text[(colon + 1)..];
        }

        var values = text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return (slot, values);
    }

    // True kung ang tubag (current) naa sa listahan.
    // "!NA" sa sugod = mogawas kung DILI kini ang tubag (apil kung wala pa'y tubag).
    //   Pananglitan "result:!NA" = itago ang Capacity kung N/A ang gipili sa Passed / Failed / N/A.
    public static bool Matches(string? current, string[] values)
    {
        if (values.Length > 0 && values[0].StartsWith('!'))
        {
            var not = values.Select((v, i) => i == 0 ? v[1..] : v);
            return current == null || !not.Contains(current, StringComparer.OrdinalIgnoreCase);
        }
        return current != null && values.Contains(current, StringComparer.OrdinalIgnoreCase);
    }
}

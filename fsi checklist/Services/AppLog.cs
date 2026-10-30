// =====================================================================
// APPLOG: usa ka lugar para sa tanang error. SAYON I-DEBUG:
//   - Matag error gi-print sa Visual Studio Output window (Debug)
//   - ug gi-save usab sa file nga "app.log" sa folder sa app (makita ang path sa Settings page)
//   - Describe(ex) = himoon nga klaro ang mensahe para sa user (pananglitan "database is locked")
// =====================================================================
using Microsoft.Data.Sqlite;

namespace fsi_checklist.Services;

public static class AppLog
{
    private static readonly object Gate = new();

    // Asa ang log file. Ablihi kini sa Notepad kung naay problema.
    public static string LogPath => Path.Combine(FileSystem.AppDataDirectory, "app.log");

    public static void Info(string message) => Write("INFO", message);

    // Isulat ang tibuok error (uban ang stack trace) aron makita kung asa nga linya.
    public static void Error(Exception ex, string? where = null) =>
        Write("ERROR", (where == null ? "" : where + ": ") + ex);

    private static void Write(string level, string message)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
        System.Diagnostics.Debug.WriteLine(line);

        try
        {
            lock (Gate)
            {
                // Kung modako na kaayo ang log (labaw 1 MB), sugdi og bag-o.
                var info = new FileInfo(LogPath);
                if (info.Exists && info.Length > 1_000_000)
                    File.Delete(LogPath);

                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
        }
        catch
        {
            // Ang pag-log dili gyud angay mo-crash sa app.
        }
    }

    /// <summary>Himoon nga klaro ang error para sa user.</summary>
    public static string Describe(Exception ex)
    {
        // Ang EF Core nagputos sa tinuod nga error; pangitaon ang pinakasulod.
        Exception root = ex;
        while (root.InnerException != null)
            root = root.InnerException;

        if (root is SqliteException sqlite)
        {
            // 5 = BUSY / 6 = LOCKED: laing programa ang naggamit sa database.
            if (sqlite.SqliteErrorCode is 5 or 6)
                return "The database is being used by another program (for example DB Browser for SQLite). " +
                       "Close that program (or click Write Changes / Revert Changes in it), then try again.";

            // 19 = CONSTRAINT: sayop nga data (pananglitan wala nga form id).
            if (sqlite.SqliteErrorCode == 19)
                return "Database rule error: " + sqlite.Message + "\nDetails are in app.log (see Settings).";
        }

        return root.Message + "\nDetails are in app.log (see Settings).";
    }
}

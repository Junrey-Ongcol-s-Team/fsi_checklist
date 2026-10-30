// =====================================================================
// DATABASE (SQLite) - offline, naa sa telepono mismo.
// Sa unang higayon: himoon ang database gikan sa schema.sql, dayon i-load ang mga form gikan sa seeds/*.sql.
// Matag seed modagan KAUSA RA (girekord sa app_setting). Kung usbon nimo ang seed, i-delete ang bfp_inspection.db3.
// =====================================================================
using fsi_checklist.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public sealed class DatabaseService
{
    // VERSION sa database. Kung mag-usab ka sa schema.sql o sa mga id sa seeds, i-plus 1 kini.
    // Ang app mo-backup sa daan nga database ug mohimo og bag-o (tan-awa BackupIfOutdatedAsync).
    private const int CurrentSchemaVersion = 2;
    private const string SchemaFileName = "schema.sql";
    private const string DatabaseFileName = "bfp_inspection.db3";

    // Form seed scripts, each applied once and recorded in app_setting.
    // A seed is plain INSERTs, so it must not be re-run: when a form changes,
    // add a new script (written as upserts) and/or bump the version.
    private sealed record SeedScript(string Asset, string SettingKey, int Version);

    // Listahan sa mga form nga i-load. Matag bag-ong form: i-add diri ang iyang seeds/XXX.sql.
    private static readonly SeedScript[] Seeds =
    [
        new("seeds/064.sql", "seed.064", 1),
        new("seeds/051.sql", "seed.051", 1),
        new("seeds/061.sql", "seed.061", 1),
        new("seeds/068.sql", "seed.068", 1),
        new("seeds/069.sql", "seed.069", 1),
        new("seeds/075.sql", "seed.075", 1),
        new("seeds/024.sql", "seed.024", 1),
        new("seeds/021.sql", "seed.021", 1),
        new("seeds/026.sql", "seed.026", 1),
        new("seeds/006.sql", "seed.006", 1),
    ];

    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public DatabaseService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Asa nakabutang ang database file sa telepono/computer. Makita sa Output window (Debug).
    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

    /// <summary>Connection string; foreign keys are enforced on every pooled connection.</summary>
    // Default Timeout=10: kung busy ang database, maghulat og hangtod 10 segundos una mo-error.
    public static string ConnectionString =>
        $"Data Source={DatabasePath};Foreign Keys=True;Default Timeout=10";

    /// <summary>Makes sure the database exists and is seeded, then returns a fresh context.</summary>
    public async Task<AppDbContext> OpenContextAsync()
    {
        await InitializeAsync();
        return await _contextFactory.CreateDbContextAsync();
    }

    // Himoon ang database kung wala pa, ug i-load ang mga seed. Kausa ra kini modagan matag pag-abli sa app.
    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized)
                return;

            AppLog.Info($"DATABASE FILE PATH: {DatabasePath}");

            // 1) Kung daan nga version ang database, i-backup ug sugdi og bag-o.
            await BackupIfOutdatedAsync();

            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Database.OpenConnectionAsync();
            var connection = (SqliteConnection)db.Database.GetDbConnection();

            // 2) WAL mode: ang pagbasa ug pagsulat dili na mag-babagay (dili mo-lock sa usag usa).
            //    Pananglitan: ang dashboard mobasa samtang ang form nag-autosave.
            await ExecuteAsync(connection, null, "PRAGMA journal_mode=WAL;");

            int version = Convert.ToInt32(
                await ScalarAsync(connection, null, "PRAGMA user_version;"));

            // user_version 0 = bag-o pa ang database, busa i-run ang schema.sql.
            if (version == 0)
            {
                await CreateSchemaAsync(connection);
            }
            else if (version != CurrentSchemaVersion)
            {
                // Mahitabo ra kini kung mas BAG-O ang database kaysa sa app (pananglitan nag-install og daan nga build).
                throw new InvalidOperationException(
                    $"The database is version {version} but this app expects {CurrentSchemaVersion}. " +
                    "Use Settings > Reset database.");
            }

            await ApplySeedsAsync(connection);

            _initialized = true;
            AppLog.Info("Database ready.");
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Basahon ang version sa database file. Kung daan (dili 0 ug dili CurrentSchemaVersion),
    // ibalhin kini ngadto sa backup file aron mahimo og bag-o. WALAY MAWALA: naa gihapon ang backup.
    private static async Task BackupIfOutdatedAsync()
    {
        if (!File.Exists(DatabasePath))
            return;

        int version;
        await using (var conn = new SqliteConnection(ConnectionString))
        {
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA user_version;";
            version = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        if (version == 0 || version >= CurrentSchemaVersion)
            return;

        string backup = MoveToBackup($"v{version}");
        AppLog.Info($"Old database (version {version}) moved to {backup}. A new one will be created.");
    }

    // RESET (gikan sa Settings): i-backup ang database ug himoon og bag-o sa sunod nga paggamit.
    public async Task<string?> ResetDatabaseAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            _initialized = false;
            if (!File.Exists(DatabasePath))
                return null;

            string backup = MoveToBackup("reset");
            AppLog.Info($"Database reset. Old file moved to {backup}");
            return backup;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // I-rename ang database (ug ang -wal, -shm, -journal nga kauban niini) ngadto sa backup nga ngalan.
    private static string MoveToBackup(string tag)
    {
        // Isira una ang tanang bukas nga koneksyon sa app, kay dili ma-rename ang file nga bukas.
        SqliteConnection.ClearAllPools();

        string backup = Path.Combine(FileSystem.AppDataDirectory,
            $"bfp_inspection.backup-{tag}-{DateTime.Now:yyyyMMdd-HHmmss}.db3");

        File.Move(DatabasePath, backup);
        foreach (string suffix in new[] { "-wal", "-shm", "-journal" })
        {
            if (File.Exists(DatabasePath + suffix))
                File.Move(DatabasePath + suffix, backup + suffix);
        }

        return backup;
    }

    private static async Task CreateSchemaAsync(SqliteConnection connection)
    {
        string schemaSql = await ReadAssetAsync(SchemaFileName);

        await using var tx = (SqliteTransaction)await connection.BeginTransactionAsync();
        try
        {
            await ExecuteAsync(connection, tx, schemaSql);
            await ExecuteAsync(connection, tx, $"PRAGMA user_version = {CurrentSchemaVersion};");
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // I-load ang matag form (seed). Kung na-load na kaniadto (naa sa app_setting), laktawan.
    private static async Task ApplySeedsAsync(SqliteConnection connection)
    {
        foreach (var seed in Seeds)
        {
            var applied = await ScalarAsync(
                connection, null,
                "SELECT value FROM app_setting WHERE key = $key;",
                ("$key", seed.SettingKey)) as string;

            if (int.TryParse(applied, out int appliedVersion) && appliedVersion >= seed.Version)
                continue;

            string sql = await ReadAssetAsync(seed.Asset);

            await using var tx = (SqliteTransaction)await connection.BeginTransactionAsync();
            try
            {
                await ExecuteAsync(connection, tx, sql);
                await ExecuteAsync(
                    connection, tx,
                    "INSERT OR REPLACE INTO app_setting (key, value) VALUES ($key, $value);",
                    ("$key", seed.SettingKey),
                    ("$value", seed.Version.ToString()));
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                // Isulti kung UNSANG seed file ang sayop, aron dali makit-an.
                AppLog.Error(ex, $"Seed failed: {seed.Asset}");
                throw new InvalidOperationException($"Loading the form {seed.Asset} failed: {ex.Message}", ex);
            }
        }
    }

    private static async Task<string> ReadAssetAsync(string name)
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(name);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    // Plain ADO.NET is used for scripts: the multi-statement scripts contain braces
    // and triggers that ExecuteSqlRaw's format-string handling must not see.
    private static async Task ExecuteAsync(
        SqliteConnection connection, SqliteTransaction? tx, string sql,
        params (string Name, object Value)[] args)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = sql;
        foreach (var (name, value) in args)
            cmd.Parameters.AddWithValue(name, value);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<object?> ScalarAsync(
        SqliteConnection connection, SqliteTransaction? tx, string sql,
        params (string Name, object Value)[] args)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = sql;
        foreach (var (name, value) in args)
            cmd.Parameters.AddWithValue(name, value);
        return await cmd.ExecuteScalarAsync();
    }
}

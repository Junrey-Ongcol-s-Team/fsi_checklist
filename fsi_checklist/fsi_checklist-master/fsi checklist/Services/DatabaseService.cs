using fsi_checklist.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Services;

public sealed class DatabaseService
{
    private const int CurrentSchemaVersion = 1;
    private const string SchemaFileName = "schema.sql";
    private const string DatabaseFileName = "bfp_inspection.db3";

    // Form seed scripts, each applied once and recorded in app_setting.
    // A seed is plain INSERTs, so it must not be re-run: when a form changes,
    // add a new script (written as upserts) and/or bump the version.
    private sealed record SeedScript(string Asset, string SettingKey, int Version);

    private static readonly SeedScript[] Seeds =
    [
        new("seeds/064.sql", "seed.064", 1),
    ];

    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public DatabaseService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

    /// <summary>Connection string; foreign keys are enforced on every pooled connection.</summary>
    public static string ConnectionString =>
        $"Data Source={DatabasePath};Foreign Keys=True";

    /// <summary>Makes sure the database exists and is seeded, then returns a fresh context.</summary>
    public async Task<AppDbContext> OpenContextAsync()
    {
        await InitializeAsync();
        return await _contextFactory.CreateDbContextAsync();
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized)
                return;

            System.Diagnostics.Debug.WriteLine($"DATABASE FILE PATH: {DatabasePath}");

            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Database.OpenConnectionAsync();
            var connection = (SqliteConnection)db.Database.GetDbConnection();

            int version = Convert.ToInt32(
                await ScalarAsync(connection, null, "PRAGMA user_version;"));

            if (version == 0)
            {
                await CreateSchemaAsync(connection);
            }
            else if (version != CurrentSchemaVersion)
            {
                throw new InvalidOperationException(
                    $"Unsupported database version: {version}. " +
                    "A database migration is required.");
            }

            await ApplySeedsAsync(connection);

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
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
            catch
            {
                await tx.RollbackAsync();
                throw;
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

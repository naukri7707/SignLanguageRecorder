using LiteDB;

namespace SignLanguageRecorder.Services;

public class DatabaseService
{
    private const int LAST_VERSION = 1;

    private string connectionString;

    public string ConnectionString
    {
        get { return connectionString; }
        set { connectionString = value; }
    }

    public DatabaseService(SecureStorageService secureStorageService)
    {
        connectionString = GetConnectionString(secureStorageService).Result;
        UpdateDatabase();
    }

    public LiteDatabase GetLiteDatabase()
    {
        return new LiteDatabase(connectionString);
    }

    private void UpdateDatabase()
    {
        using var db = new LiteDatabase(connectionString);
        if (db.UserVersion != LAST_VERSION)
        {
            if (db.UserVersion == 0)
            {
                // 這裡用來將新使用者的 database 更新到最新版

                // 將 RecorderLayout.Name 設為索引
                var recorderLayout = db.GetCollection<RecorderLayout>();
                recorderLayout.EnsureIndex(it => it.Name, true);
                // 更新版本號至最新版
                db.UserVersion = LAST_VERSION;
            }

            if (db.UserVersion == 1)
            {
                // 這裡用來將舊使用者的 database 更新到下一版
                // db.UserVersion = 2;
            }

        }
    }

    public LiteFileInfo<string> Upload(byte[] bytes, string fileName)
    {
        using var db = new LiteDatabase(connectionString);
        var id = Guid.NewGuid().ToString();
        var stream = new MemoryStream(bytes);
        return db.FileStorage.Upload(id, fileName, stream);
    }

    public byte[] Download(string id)
    {
        using var db = new LiteDatabase(connectionString);
        var stream = new MemoryStream();
        db.FileStorage.Download(id, stream);
        stream.Position = 0;

        return stream.ToArray();
    }

    private static async Task<string> GetConnectionString(SecureStorageService secureStorageService)
    {
        const string databaseName = "data.db";
        const string password = "encryptionKey";
        var dataPath = FileSystem.Current.AppDataDirectory;

        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        var filePath = Path.Combine(dataPath, databaseName);
        var encryptionKey = await secureStorageService.Get(password);

        if (encryptionKey is null)
        {
            encryptionKey = Guid.NewGuid().ToString();
            await secureStorageService.Set(password, encryptionKey);
        }

        return $"FileName={filePath};Password={password};";
    }
}

using LiteDB;

namespace SignLanguageRecorder.Extensions;

public static class LiteDB
{
    public static IBsonDataReader Dump<T>(this ILiteDatabase database, string filePath)
    {
        return Dump(database, typeof(T).Name, filePath);
    }

    public static IBsonDataReader Dump(this ILiteDatabase database, string collectionName, string filePath)
    {
        // 自動加上.json附檔名（如果沒有）
        if (!Path.HasExtension(filePath) || Path.GetExtension(filePath).ToLower() != ".json")
        {
            filePath = $"{filePath}.json";
        }
        filePath = filePath.Replace('\\', '/');

        return database.Execute($"SELECT $ INTO $file('{filePath}') FROM {collectionName};");
    }
}

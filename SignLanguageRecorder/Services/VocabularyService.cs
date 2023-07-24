using LiteDB;
using SignLanguageRecorder.Extensions;
using System.IO;
using System.Text;
using ZXing;

namespace SignLanguageRecorder.Services;

public class VocabularyService
{
    private readonly DatabaseService databaseService;

    public VocabularyService(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public void AddVocabularyFromJsonFile(string filePath)
    {
        using var db = databaseService.GetLiteDatabase();
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var jsonReader = new JsonReader(reader);
        // 必須要 ToArray 否則不能正常遍歷
        var docs = jsonReader.DeserializeArray().ToArray().Select(it => it.AsDocument);
        var collection = db.GetCollection(nameof(VocabularyInfo));
        collection.Upsert(docs);
    }

    public void Drop()
    {
        using var db = databaseService.GetLiteDatabase();

        db.DropCollection(nameof(VocabularyInfo));
    }

    public void Dump(string filePath)
    {
        using var db = databaseService.GetLiteDatabase();

        db.Dump(nameof(VocabularyInfo), filePath);
    }
}
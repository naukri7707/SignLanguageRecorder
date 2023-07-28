using LiteDB;
using SignLanguageRecorder.Extensions;
using System.Text;

namespace SignLanguageRecorder.Services;

public class VocabularyService
{
    private VocabularyInfo[] vocabularyInfos;

    public VocabularyInfo[] VocabularyInfos => vocabularyInfos;

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

    public async void Test()
    {
        await UpdateVocabularyInfos();
        Console.WriteLine("1");
    }

    public async Task<VocabularyInfo[]> UpdateVocabularyInfos()
    {
        await Task.Yield();
        using var db = databaseService.GetLiteDatabase();
        var collection = db.GetCollection<VocabularyInfo>();

        vocabularyInfos = collection.FindAll().ToArray();

        return vocabularyInfos;
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
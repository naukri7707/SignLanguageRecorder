using LiteDB;
using SignLanguageRecorder.Extensions;
using System.Text;

namespace SignLanguageRecorder.Services;

public class VocabularyService
{
    private readonly DatabaseService databaseService;

    private VocabularyInfo[] vocabularyInfos;

    public VocabularyService(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public async Task<int> AddVocabularyFromJsonFileAsync(string filePath)
    {
        await Task.Yield();
        using var db = databaseService.GetLiteDatabase();
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var jsonReader = new JsonReader(reader);
        // 必須要 ToArray 否則不能正常遍歷
        var docs = jsonReader.DeserializeArray().ToArray().Select(it => it.AsDocument);
        var collection = db.GetCollection(nameof(VocabularyInfo));
        return collection.Upsert(docs);
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

    public async Task<VocabularyInfo[]> GetVocabularyInfos()
    {
        if (vocabularyInfos == null)
        {
            await Task.Yield();
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<VocabularyInfo>();

            vocabularyInfos = collection.FindAll().ToArray();
        }

        return vocabularyInfos;
    }
}
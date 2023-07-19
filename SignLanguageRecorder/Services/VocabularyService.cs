using LiteDB;
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

    public void DoThis()
    {
        //using var db = databaseService.GetLiteDatabase();
        //var collection = db.GetCollection<VocabularyInfo>();

        //var elep = collection.FindOne(it => it.Name == "大象1");
        //elep.Name = "大象";
        //elep.WordbookIds = 
        //collection.Update(elep);
        //collection.
    }

    public void AddVocabularyFromJsonFile(string filePath)
    {
        using var reader = new StreamReader(filePath, Encoding.UTF8);
        using var db = databaseService.GetLiteDatabase();

        var json = new JsonReader(reader);
        var vocabularies = json.DeserializeArray();
        var collection = db.GetCollection<VocabularyInfo>();
        collection.EnsureIndex(it => it.Name);
        collection.DeleteAll();
        foreach (var vocabulary in vocabularies)
        {
            var name = vocabulary["Name"].AsString;
            var ph = vocabulary["Phonetic"].AsString;
            var isRecognizable = vocabulary["IsRecognizable"].AsBoolean;
            var isVocabulary = vocabulary["IsVocabulary"].AsBoolean;
            var wordbookIds = vocabulary["WordbookIds"].AsArray;

            var dbObj = collection.FindOne(it => it.Name == name);
            if (dbObj == null)
            {
                var voc = new VocabularyInfo
                {
                    Name = name,
                    Phonetic = ph,
                    IsRecognizable = isRecognizable,
                    IsVocabulary = isVocabulary,
                    Signs = new SignInfo[] { new SignInfo() { Tag = "", WordbookIds = wordbookIds.Select(it => it.AsInt32).ToArray(), FileName = "" } }
                };
                collection.Insert(voc);
            }
            else
            {
                dbObj.Signs = dbObj.Signs
                    .Append(new SignInfo() { Tag = "", WordbookIds = wordbookIds.Select(it => it.AsInt32).ToArray(), FileName = "" })
                    .ToArray();
                for (int i = 0; i < dbObj.Signs.Length; i++)
                {
                    dbObj.Signs[i].Tag = $"({NumberToLetters(i)})";
                }
                collection.Update(dbObj);
            }

            
        }
    }

    static string NumberToLetters(int number)
    {
        number++;
        string[] lettersArray = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        string result = "";

        while (number > 0)
        {
            int remainder = (number-1) % 26;
            result = lettersArray[remainder] + result;
            number = (number - 1) / 26;
        }

        return result;
    }

    public void Dump<T>(string filePath)
    {
        using var db = databaseService.GetLiteDatabase();

        // 自動加上.json附檔名（如果沒有）
        if (!Path.HasExtension(filePath) || Path.GetExtension(filePath).ToLower() != ".json")
        {
            filePath = $"{filePath}.json";
        }
        filePath = filePath.Replace('\\', '/');

        db.Execute($"SELECT $ INTO $file('{filePath}') FROM {typeof(T).Name};");
    }
}
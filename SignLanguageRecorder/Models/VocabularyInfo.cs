using LiteDB;

namespace SignLanguageRecorder.Models;

public class VocabularyInfo
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Phonetic { get; set; }

    public int BookId { get; set; }

    public bool IsRecognizable { get; set; }

    public bool IsVocabulary { get; set; }

    public string[] DemoVideoRelativePaths { get; set; }
}
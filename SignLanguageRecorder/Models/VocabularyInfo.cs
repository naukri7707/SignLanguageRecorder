using LiteDB;
using Microsoft.Maui.Controls;

namespace SignLanguageRecorder.Models;

public class VocabularyInfo
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Phonetic { get; set; }

    public bool IsRecognizable { get; set; }

    public bool IsVocabulary { get; set; }

    public SignInfo[] Signs { get; set; }

    public bool MultiSign => Signs?.Length > 1;

    public bool IsCompleted => Signs.All(it => it.IsCompleted);

    public string ProgressText
    {
        get
        {
            var completedCount = Signs.Count(it => it.IsCompleted);
            return $"({completedCount}/{Signs.Length})";
        }
    }

    public string GetVideoName(int sign = 0)
    {
        return Signs.Length > 1
            ? $"{Name}_{Signs[sign].Tag}"
            : Name;
    }
}

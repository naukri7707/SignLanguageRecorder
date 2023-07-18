using LiteDB;

namespace SignLanguageRecorder.Models;

public class RecorderLayout
{
    public ObjectId Id { get; init; }

    public string Name { get; init; }

    public RecorderInfo[] Infos { get; init; }
}

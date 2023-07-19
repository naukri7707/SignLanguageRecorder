using LiteDB;

namespace SignLanguageRecorder.Models;

public abstract class UserInfo
{
    public string Name { get; set; }

    public abstract UserGroup Group { get; }
}

public enum UserGroup
{
    Guest,
    Signee,
    Recorder,
    Developer,
}

public class SigneeInfo : UserInfo
{
    public override UserGroup Group => UserGroup.Signee;

    public Dictionary<ObjectId, string> RecordedVideoPath { get; }
}

using LiteDB;

namespace SignLanguageRecorder.Models;

public class SignLanguageInfo : IValueEquals<SignLanguageInfo>, ICopyable<SignLanguageInfo>
{
    public SignLanguageInfo
        (string name = "", string demoVideoSource = "")
    {
        Name = name;
        DemoVideoSource = demoVideoSource;
    }

    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string DemoVideoSource { get; set; }

    public bool IsValueEquals(object other) => IsValueEquals((SignLanguageInfo)other);

    public bool IsValueEquals(SignLanguageInfo other)
    {
        return Name == other.Name
            && DemoVideoSource == other.DemoVideoSource;
    }

    public SignLanguageInfo ShallowCopy()
    {
        return (SignLanguageInfo)MemberwiseClone();
    }

    public SignLanguageInfo DeepCopy()
    {
        return new SignLanguageInfo()
        {
            Id = new ObjectId(Id),
            Name = Name,
            DemoVideoSource = DemoVideoSource,
        };
    }
}

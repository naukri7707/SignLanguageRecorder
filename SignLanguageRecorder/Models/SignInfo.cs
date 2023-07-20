namespace SignLanguageRecorder.Models;

public class SignInfo
{
    public string Tag { get; set; }

    public int[] WordbookIds { get; set; }

    // Todo 移除
    public bool IsCompleted => false;

    public override string ToString()
    {
        return Tag;
    }
}
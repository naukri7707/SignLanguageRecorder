namespace SignLanguageRecorder.Models;

public class SignInfo
{
    public string Tag { get; set; }

    public bool IsCompleted { get; set; }

    public string FileName { get; set; }

    public int[] WordbookIds { get; set; }

    public override string ToString()
    {
        return Tag;
    }
}
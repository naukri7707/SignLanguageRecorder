using LiteDB;

namespace SignLanguageRecorder.Services;

public class RecordService
{
    private readonly Recorder[] recorders;
    private readonly DatabaseService databaseService;

    public Recorder[] Recorders => recorders;

    private int activedRecorderCount = 1;
    public int ActivedRecorderCount
    {
        get => activedRecorderCount;
        set
        {
            if(value < 1 || value > 16)
            {
                throw new ArgumentException($"{nameof(ActivedRecorderCount)} 必須介於 1 ~ 16 之間");
            }
            activedRecorderCount = value;
        }
    }

    private bool isRecording;

    public bool IsRecording => isRecording;

    public RecordService() : this(
        Dependency.Inject<DatabaseService>()
        )
    { }

    public RecordService(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        //
        recorders = new Recorder[16];
        for (int i = 0; i < 16; i++)
        {
            var recorder = new Recorder
            {
                IsEnabled = false,
                IsVisible = false,
            };

            if (string.IsNullOrEmpty(recorder.ViewModel.RecorderName))
            {
                recorder.ViewModel.RecorderName = $"Cam {i}";
            }

            recorders[i] = recorder;
        }
    }

    public async void StartRecording(string videoName)
    {
        if (isRecording)
        {
            throw new Exception("Recorder is started already");
        }

        await Task.Yield();
        var recorders = Recorders;
        for (int i = 0; i < ActivedRecorderCount; i++)
        {
            var recorder = recorders[i];
            _ = recorder.ViewModel.StartRecordAsync(videoName);
        }
        isRecording = !isRecording;
    }

    public async void StopRecording()
    {
        if (!isRecording)
        {
            throw new Exception("Recorder is stopped already");
        }

        await Task.Yield();
        var recorders = Recorders;
        for (int i = 0; i < ActivedRecorderCount; i++)
        {
            var recorder = recorders[i];
            _ = recorder.ViewModel.StopRecordAsync();
        }
        isRecording = !isRecording;
    }

    public void SaveLayout(string layoutName)
    {
        var infos = recorders
            .Select(it => it.ViewModel.GetInfo())
            .Take(ActivedRecorderCount)
            .ToArray();

        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();

            var layoutId = collection.FindOne(x => x.Name == layoutName)?.Id ?? ObjectId.NewObjectId();
            var layout = new RecorderLayout
            {
                Name = layoutName,
                Infos = infos
            };
            collection.Upsert(layoutId, layout);
        }
    }

    public bool LoadLayout(string layoutName)
    {
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();
            var layout = collection.FindOne(x => x.Name == layoutName);
            if(layout != null)
            {
                ActivedRecorderCount = layout.Infos.Length;
                for (int i = 0; i < ActivedRecorderCount; i++)
                {
                    var recorder = recorders[i];
                    recorder.ViewModel.SetInfo(layout.Infos[i]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool DeleteLayout(string layoutName)
    {
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();

            var id = collection.FindOne(x => x.Name == layoutName)?.Id;

            if (id == null)
            {
                return false;
            }

            return collection.Delete(id);
        }
    }

    public string[] GetLayoutNames()
    {
        lock (databaseService)
        {
            using var db = databaseService.GetLiteDatabase();
            var collection = db.GetCollection<RecorderLayout>();

            return collection.Query().Select(x => x.Name).ToArray();
        }
    }
}

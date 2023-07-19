using LiteDB;

namespace SignLanguageRecorder.Services
{
    public class RecorderLayoutService
    {
        private readonly DatabaseService databaseService;

        public RecorderLayoutService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public void SaveLayout(string layoutName, RecorderInfo[] infos)
        {
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

        public RecorderLayout LoadLayout(string layoutName)
        {
            lock (databaseService)
            {
                using var db = databaseService.GetLiteDatabase();
                var collection = db.GetCollection<RecorderLayout>();

                return collection.FindOne(x => x.Name == layoutName);
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
}

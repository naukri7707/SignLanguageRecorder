using Python.Runtime;

namespace SignLanguageRecorder.Services
{
    public class JointsRecognizerService
    {
        private PyModule scope;

        private PyDict members;
        private readonly PreferencesService preferencesService;
        private PythonService pythonService;

        private PyObject getSkeletonImagePyMethod;

        private PyObject createSkeletonVideoPyMethod;

        public JointsRecognizerService(PreferencesService preferencesService, PythonService pythonService)
        {
            this.preferencesService = preferencesService;
            this.pythonService = pythonService;

            using (Py.GIL())
            {
                var scriptPath = Path.Combine(preferencesService.PythonFolder, "Scripts", "JointsRecognizer.py");

                scope = pythonService.CreateScope(
                    nameof(JointsRecognizerService),
                    scriptPath,
                    out members
                    );
                //
                getSkeletonImagePyMethod = members.GetItem("get_skeleton_image");
                createSkeletonVideoPyMethod = members.GetItem("create_skeleton_video");
            }
        }

        public async void GetSkeletonImage(ImageSource imageSource)
        {
            using (Py.GIL())
            {
                // 必須在釋放前取得 image 的 byte[] 否則將無法讀寫
                using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                // 傳送給 python 處理
                var csSrcBytes = memoryStream.ToArray().ToPython();
                var pyHandledBytes = getSkeletonImagePyMethod.Invoke(csSrcBytes);

                // 取得處理後的 byte[]
                var csHandledBytes = pyHandledBytes.AsManagedObject(typeof(byte[])) as byte[];
            }
        }

        public async void CreateSkeletonVideo(string sourceFilePath, string destinationFilePath, Action onCompleted = null)
        {
            using (Py.GIL())
            {
                await Task.Yield();
                var src = sourceFilePath.ToPython();
                var dst = destinationFilePath.ToPython();
                createSkeletonVideoPyMethod.Invoke(src, dst);
                onCompleted?.Invoke();
            }
        }
    }
}

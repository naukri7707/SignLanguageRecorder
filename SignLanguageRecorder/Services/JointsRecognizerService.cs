using Python.Runtime;
using System.Diagnostics;

namespace SignLanguageRecorder.Services
{
    public class JointsRecognizerService
    {
        public bool IsBusy { get; private set; }

        public bool IsInitialized { get; private set; }

        private PyModule scope;

        private PyDict members;

        private readonly PreferencesService preferencesService;

        private readonly PythonService pythonService;

        private PyObject getSkeletonImagePyMethod;

        private PyObject createSkeletonVideoPyMethod;

        public JointsRecognizerService(PreferencesService preferencesService, PythonService pythonService)
        {
            this.preferencesService = preferencesService;
            this.pythonService = pythonService;
        }

        public async Task Initialize()
        {
            if (IsBusy)
            {
                throw new ServiceIsBusyException();
            }

            if (IsInitialized)
            {
                return;
            }

            await Task.Yield();
            IsBusy = true;
            var scriptPath = Path.Combine(preferencesService.PythonFolder, "Scripts", "JointsRecognizer.py");
            (scope, members) = await pythonService.CreateScope(
                nameof(JointsRecognizerService),
                scriptPath
                );
            //
            getSkeletonImagePyMethod = members.GetItem("get_skeleton_image");
            createSkeletonVideoPyMethod = members.GetItem("create_skeleton_video");

            IsInitialized = true;
            IsBusy = false;
        }

        public async Task GetSkeletonImage(ImageSource imageSource)
        {
            if (IsBusy)
            {
                throw new ServiceIsBusyException();
            }

            if (!IsInitialized)
            {
                throw new ServiceNotInitializedException();
            }

            await Task.Yield();
            using (Py.GIL())
            {
                IsBusy = true;
                // 必須在釋放前取得 image 的 byte[] 否則將無法讀寫
                using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                // 傳送給 python 處理
                var csSrcBytes = memoryStream.ToArray().ToPython();
                var pyHandledBytes = getSkeletonImagePyMethod.Invoke(csSrcBytes);

                // 取得處理後的 byte[]
                var csHandledBytes = pyHandledBytes.AsManagedObject(typeof(byte[])) as byte[];
                IsBusy = false;
            }
        }

        public async Task CreateSkeletonVideo(string sourceFilePath, string destinationFilePath)
        {
            if (IsBusy)
            {
                throw new ServiceIsBusyException($"{nameof(JointsRecognizerService)} 繁忙中，請稍號再試。");
            }

            if (!IsInitialized)
            {
                throw new ServiceNotInitializedException();
            }

            await Task.Yield();
            using (Py.GIL())
            {
                IsBusy = true;
                var src = sourceFilePath.ToPython();
                var dst = destinationFilePath.ToPython();
                createSkeletonVideoPyMethod.Invoke(src, dst);
                IsBusy = false;
            }
        }
    }
}

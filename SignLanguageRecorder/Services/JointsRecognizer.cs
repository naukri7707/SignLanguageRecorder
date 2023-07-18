using Python.Runtime;

namespace SignLanguageRecorder.Services
{
    public class JointsRecognizer
    {
        private PyModule scope;

        private PyDict members;

        private PythonService pythonService;

        private PyObject getJointsPyMethod;

        public JointsRecognizer(PythonService pythonService)
        {
            this.pythonService = pythonService;
            scope = pythonService.CreateScope(
                nameof(JointsRecognizer),
                @"C:\Users\Naukri\source\repos\EmitTest\EmitTest\Python\Test.py",
                out members
                );
            //
            getJointsPyMethod = members.GetItem("get_joints");
        }

        public async void GetJoints(ImageSource imageSource)
        {
            // 再釋放前取得 image 的 byte[] 否則將無法讀寫
            using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            // 傳送給 python 處理
            var csSrcBytes = memoryStream.ToArray().ToPython();
            var pyHandledBytes = getJointsPyMethod.Invoke(csSrcBytes);

            // 取得處理後的 byte[]
            var csHandledBytes = pyHandledBytes.AsManagedObject(typeof(byte[])) as byte[];
        }

    }
}

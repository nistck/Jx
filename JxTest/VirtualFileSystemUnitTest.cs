using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jx.FileSystem;

namespace JxTest
{
    [TestClass]
    public class VirtualFileSystemUnitTest
    {
        [TestMethod]
        public void TestVFS()
        {
            string logPath = string.Format("user:Logs/JxMain.log");
            //initialize file sytem of the engine
            if (!VirtualFileSystem.Init(logPath, true, null, null, null, null))
                return;


        }
    }
}

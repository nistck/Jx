using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx;
using Jx.FileSystem;

namespace JxMain
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Bootstrap();
            Application.Run(new MainForm());
        }

        private static void Bootstrap()
        {
            string logPath = string.Format("user:Logs/{0}.log", ExecutableName);
            //initialize file sytem of the engine
            if (!VirtualFileSystem.Init(logPath, true, null, null, null, null))
                return;
            Log.Info(">> Log Path: {0}", logPath);
        }

        public static string ExecutableName
        {
            get
            {
                string executablePath = Application.ExecutablePath;
                int iPos = executablePath.LastIndexOf('\\');
                int jPos = executablePath.IndexOf('.', iPos + 1);
                string result = jPos == -1 ? executablePath.Substring(iPos + 1) : executablePath.Substring(iPos + 1, jPos - iPos - 1);
                return result;
            }
        }
    }
}

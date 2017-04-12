using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JxRes
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
            Application.Run(new MainForm());
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

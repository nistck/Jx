using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.BT
{
    public delegate void BTLogHandler(string message, params object[] args);
    public static class BTDebug
    {
        public static bool EnabledDefault { get; set; } = true;

        private static void Print(string type, string message, params object[] args)
        {
            if (!EnabledDefault)
                return; 

            type = type ?? "Info";
            message = message ?? "";
            string text = string.Format(message, args);
            Console.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] [{1}] {2}", DateTime.Now, type, text);
        }

        public static event BTLogHandler OnFatal; 
        public static void Fatal(string message, params object[] args)
        {
            OnFatal?.Invoke(message, args);
            Print("Fatal", message, args); 
        }

        public static event BTLogHandler OnWarning;
        public static void Warning(string message, params object[] args)
        {
            OnWarning?.Invoke(message, args);
            Print("Warning", message, args);
        }

        public static event BTLogHandler OnInfo;
        public static void Info(string message, params object[] args)
        {
            OnInfo?.Invoke(message, args);
            Print("Info", message, args);
        }

        public static event BTLogHandler OnError; 
        public static void Error(string message, params object[] args)
        {
            OnError?.Invoke(message, args);
            Print("Error", message, args);
        }
         
    }
}

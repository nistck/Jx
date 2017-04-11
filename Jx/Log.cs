using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Jx
{
    public static class Log
    {
         
        public static void _Init(Thread t, string filePath)
        {

        }

        public static readonly string NEW_LINE = "\r\n";
        private static long tick0 = DateTime.Now.Ticks;

        public static long Tick0
        {
            get
            {
                long ts = DateTime.Now.Ticks - tick0;
                return ts < 0 ? 0 : ts;
            }
            set { tick0 = value; }
        }

        private static string GetCallerInfo(StackFrame f)
        {
            if (f == null)
                return "<NATIVE>";

            MethodBase mb = f.GetMethod();
            string caller = string.Format("{0}/{1} (LINE: {2}, COL: {3})",
                mb.DeclaringType.FullName, mb.Name, f.GetFileLineNumber(), f.GetFileColumnNumber());
            return caller;
        }

        public static void LoggingInfo(int callerIndex, int callerLevel, string type, object format, params object[] args)
        {
#if DEBUG
            if (format == null)
                return;

            callerLevel = callerLevel < 0 ? 0 : callerLevel;

            StackTrace ss = new StackTrace(true);
            string caller = null;

            int callerBaseIndex = callerIndex + 1;
            StackFrame[] sf = ss.GetFrames();
            if (sf != null && sf.Length > callerBaseIndex)
            {
                StackFrame f = sf[callerBaseIndex];
                caller = GetCallerInfo(f);
            }

            // 1, time + caller
            string basicInfo = string.Format("[{0:HH:mm:ss.fff}, {1} ms] {2}", DateTime.Now, Tick0 / 10000, caller);

            // 2, Log Message
            string text = string.Format(format.ToString(), args);
            string logInfo = string.Format("[{0}] {1}", type ?? "INFO", text);

            // 3, call stack
            StringBuilder stackBuf = new StringBuilder();
            for (int i = callerBaseIndex + 1; i < callerBaseIndex + callerLevel + 1 && i < sf.Length; i++)
            {
                StackFrame f = sf[i];
                string _callerInfo = GetCallerInfo(f);
                if (stackBuf.Length > 0)
                    stackBuf.Append(NEW_LINE);

                string sfInfo = string.Format(" >> 栈#{0:00} {1}", i - callerBaseIndex, _callerInfo);
                stackBuf.Append(sfInfo);
            }

            StringBuilder messageBuf = new StringBuilder();
            messageBuf.Append(basicInfo)
                .Append(NEW_LINE).Append(logInfo)
                ;
            if (stackBuf.Length > 0)
                messageBuf.Append(NEW_LINE).Append(stackBuf);
            messageBuf.Append(NEW_LINE);

            string message = messageBuf.ToString();
            Console.WriteLine(message);
#endif
        }


        public static void LoggingWarning(int callerIndex, int callerLevel, string type, object format, params object[] args)
        {
#if DEBUG
            if (format == null)
                return;

            callerLevel = callerLevel < 0 ? 0 : callerLevel;

            StackTrace ss = new StackTrace(true);
            string caller = null;

            int callerBaseIndex = callerIndex + 1;
            StackFrame[] sf = ss.GetFrames();
            if (sf != null && sf.Length > callerBaseIndex)
            {
                StackFrame f = sf[callerBaseIndex];
                caller = GetCallerInfo(f);
            }

            // 1, time + caller
            string basicInfo = string.Format("[{0:HH:mm:ss.fff}, {1} ms] {2}", DateTime.Now, Tick0 / 10000, caller);

            // 2, Log Message
            string text = string.Format(format.ToString(), args);
            string logInfo = string.Format("[{0}] {1}", type ?? "WARNING", text);

            // 3, call stack
            StringBuilder stackBuf = new StringBuilder();
            for (int i = callerBaseIndex + 1; i < callerBaseIndex + callerLevel + 1 && i < sf.Length; i++)
            {
                StackFrame f = sf[i];
                string _callerInfo = GetCallerInfo(f);
                if (stackBuf.Length > 0)
                    stackBuf.Append(NEW_LINE);

                string sfInfo = string.Format(" >> 栈#{0:00} {1}", i - callerBaseIndex, _callerInfo);
                stackBuf.Append(sfInfo);
            }

            StringBuilder messageBuf = new StringBuilder();
            messageBuf.Append(basicInfo)
                .Append(NEW_LINE).Append(logInfo)
                ;
            if (stackBuf.Length > 0)
                messageBuf.Append(NEW_LINE).Append(stackBuf);
            messageBuf.Append(NEW_LINE);

            string message = messageBuf.ToString();
            Console.WriteLine(message);
#endif
        }

        public static void Fatal(object format, params object[] args)
        {
            LoggingInfo(1, 0, "FATAL", format, args);
        }

        public static void Error(object format, params object[] args)
        {
            LoggingInfo(1, 0, "ERROR", format, args);
        }

        public static void Info(object format, params object[] args)
        {
            LoggingInfo(1, 0, "INFO", format, args);
        }

        public static void Warning(object format, params object[] args)
        {
            LoggingWarning(1, 0, "WARNING", format, args);
        }

        public static void Debug(object format, params object[] args)
        {
            LoggingInfo(1, 0, "DEBUG", format, args);
        }

        /// <summary>
        /// 请使用 Debug 函数
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Obsolete]
        public static void DebugInfo(object format, params object[] args)
        {
            LoggingInfo(1, 0, "DEBUG", format, args);
        }

        public static void Debug2(object format, params object[] args)
        {
            LoggingInfo(2, 0, "DEBUG", format, args);
        }

        public static void Debug3(object format, params object[] args)
        {
            LoggingInfo(3, 0, "DEBUG", format, args);
        }

        public static void Info(string message)
        {
            LoggingInfo(1, 0, "INFO", message);
        }

        public static void Warning(string message)
        {
            LoggingWarning(1, 0, "WARNING", message);
        }

        public static void Error(string message)
        {
            LoggingInfo(1, 0, "ERROR", message); 
        }

        public static void Fatal(string message)
        {
            LoggingInfo(1, 0, "FATAL", message);
        }
    }
}

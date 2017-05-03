using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jx
{
    public delegate void LongOperationNotifyHandler(string text);

    public static class LongOperationNotifier
    {
        private static readonly List<string> messages = new List<string>();
        private static readonly AutoResetEvent messageEvent = new AutoResetEvent(false);
        private static Thread messageThread = null;
        private static bool messageThreadBoot = false; 
 
        public static event LongOperationNotifyHandler LongOperationNotify;

        public static void Setup()
        {
            lock(messages)
            {
                if( messageThread == null )
                {
                    messageThread = new Thread(new ThreadStart(handleMessage));
                    messageThreadBoot = true;

                    messageThread.Start();                    
                }
            }
        }

        public static void Shutdown()
        {
            lock(messages)
            {
                if( messageThread != null)
                {
                    messageThreadBoot = false;
                    messageThread.Interrupt();
                    messageThread.Abort();
                    messageThread = null;
                }
            }
        }

        private static void handleMessage()
        {
            try
            {
                while (messageThreadBoot)
                {
                    messageEvent.WaitOne();

                    string message = null; 
                    lock(messages)
                    {
                        if( messages.Count > 0 )
                        {
                            message = messages[0];
                            messages.RemoveAt(0);
                        }
                    }

                    if ( message != null && LongOperationNotify != null)
                        LongOperationNotify(message);
                }
            }
            catch (Exception) { }
        }

        public static void Notify(string text, params object[] args)
        {
            if (text == null)
                return; 

            lock(messages)
            {
                string message = string.Format(text, args);
                if (messageThread != null)
                {
                    messages.Add(message);
                    messageEvent.Set();
                }
                else
                {
                    if (LongOperationNotify != null)
                        LongOperationNotify(message);
                }
            }
        }


    }
}

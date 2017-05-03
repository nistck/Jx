using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Jx.UI
{
    /// <summary>
    /// Defined types of messages: Success/Warning/Error.
    /// </summary>
    public enum TypeOfMessage
    {
        Success,
        Warning,
        Error, 
    }
    /// <summary>
    /// Initiate instance of SplashScreen
    /// </summary>
    public static class SplashScreen
    {
        private static SplashScreenForm sf = null;
        private static Thread splashThread = null;

        public static void Show(string splashScreenImagePath)
        {
            splashThread = new Thread(new ParameterizedThreadStart(_ShowSplashScreen));
            splashThread.IsBackground = true; 
            splashThread.Start(splashScreenImagePath); 
        }

        public static void Hide()
        {
            if( splashThread != null )
            {
                try
                {
                    splashThread.Interrupt();
                    splashThread.Abort();
                    splashThread = null; 
                }
                catch (Exception) { }
            }

            if (sf != null)
            {
                sf.CloseSplashScreen();
                sf = null;
            }
        }

        private static void _ShowSplashScreen(object state) 
        {
            ShowSplashScreen(state as string);
        }

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        public static void ShowSplashScreen(string splashImagePath)
        {
            if (sf == null)
            {
                sf = new SplashScreenForm();
                sf.SetSplashImagePath(splashImagePath);
                sf.ShowSplashScreen();
            }
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public static void CloseSplashScreen()
        {
            if (sf != null)
            {
                sf.CloseSplashScreen();
                sf = null;
            }
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="Text">Message</param>
        public static void UpdateStatusText(string Text)
        {
            if (sf != null)
                sf.UpdateStatusText(Text);

        }
 
        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="Text">Message</param>
        /// <param name="tom">Type of Message</param>
        public static void UdpateStatusTextWithStatus(string Text,TypeOfMessage tom)
        {
            
            if (sf != null)
                sf.UdpateStatusTextWithStatus(Text, tom);
        }
    }

}

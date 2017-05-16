using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Jx.FileSystem;
using Jx.UI.Controls.FCTB;

namespace Jx.UI.Forms
{
    public partial class ConsoleForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static ConsoleForm instance = null; 

        public static ConsoleForm DefaultInstance
        {
            get { return instance; }
        }

        private TextStyle bracketStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
        private TextStyle timeStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private TextStyle textStyle = new TextStyle(Brushes.White, null, FontStyle.Regular);

        public ConsoleForm()
        {
            if( instance == null )
                instance = this; 
            InitializeComponent();
        }

        ~ConsoleForm ()
        {
            instance = null;
        }

        public void Write(string format, Style style, params object[] args)
        {
            if (format == null)
                return;

 
        }

        public void WriteLine(string format, Style style, params object[] args)
        {
            Write(format, style, args);
            Write("\n");
        }

        public void Write(string format, params object[] args)
        {
            Write(format, textStyle, args);       
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(format, textStyle, args);
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
 
        } 
    }
}

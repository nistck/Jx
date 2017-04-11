using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx.UI.PGEx;
using Jx.FileSystem;

namespace Jx
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Bootstrap();
        }
        
        private void Bootstrap()
        {
            string logPath = string.Format("user:Logs/JxMain.log");
            //initialize file sytem of the engine
            if (!VirtualFileSystem.Init(logPath, true, null, null, null, null))
                return;

            Log.Info(">> Log Path: {0}", logPath);

            EngineApp.Init(new MyEngineApp());

            bool created = EngineApp.Instance.Create(); 
            if( created)
            {
                EngineApp.Instance.Run();
            }
            EngineApp.Shutdown();
        }
    }
}

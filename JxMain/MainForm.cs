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

using Jx;
using Jx.UI.Controls.PGEx;
using Jx.FileSystem;
using Jx.UI;
using Jx.UI.Forms;
using Jx.EntitySystem;
using Jx.MapSystem;

namespace JxMain
{
    public partial class MainForm : Form
    {
        private ImageCache imageCache = null; 

        public MainForm()
        {
            this.Hide();
            LongOperationNotifier.LongOperationNotify += LongOperationCallbackManager_LongOperationNotify;

            string p0 = Path.GetDirectoryName(Application.ExecutablePath);
            string filePath = Path.Combine(p0, @"Resources\Splash.jpg");
            SplashScreen.Show(filePath);

            InitializeComponent();
        }

        private void LongOperationCallbackManager_LongOperationNotify(string text)
        {
            if (text == null)
                return;

            SplashScreen.UpdateStatusText(text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            imageCache = new ImageCache(IL16);
            tsbLoad.Image = imageCache["load"];
            tsbUnload.Image = imageCache["unload"];

            Console.WriteLine(System.Threading.SynchronizationContext.Current);
            Bootstrap(timerEntitySystemWorld.Interval);

            #region Splash Screen
            LongOperationNotifier.LongOperationNotify -= LongOperationCallbackManager_LongOperationNotify;
            this.Show();
            SplashScreen.Hide();
            this.Activate();

            this.WindowState = FormWindowState.Maximized;
            #endregion 

        }

        private void Bootstrap(int loopInterval)
        {
            JxEngineApp.Init(new JxMainApp(loopInterval));

            bool created = JxEngineApp.Instance.Create();
            if (created)
            {
                JxEngineApp.Instance.Run();
            }
            //EngineApp.Shutdown();
        }

        private void tsbLoad_Click(object sender, EventArgs e)
        {
            string p = @"Maps\NewMap\Map.map";
            bool loadResult = MapWorld.Instance.MapLoad(p);
            if (!loadResult)
                return; 
            timerEntitySystemWorld.Enabled = true;
            Log.Info("加载地图: {0}", p);
        }

        private void tsbUnload_Click(object sender, EventArgs e)
        {
            if (Map.Instance == null)
                return;
            string filePath = Map.Instance.VirtualFileName;

            timerEntitySystemWorld.Enabled = false;
            MapWorld.Instance.MapDestroy();
            Log.Info("地图销毁: {0}", filePath);
        }

        private void timerEntitySystemWorld_Tick(object sender, EventArgs e)
        {
            if (EntitySystemWorld.Instance != null)
                EntitySystemWorld.Instance.Tick();
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            tsbLoad.Enabled = !MapWorld.MapLoaded;
            tsbUnload.Enabled = MapWorld.MapLoaded;
            MapInfo.Text = MapWorld.MapLoaded ? string.Format("地图: {0}", Map.Instance.VirtualFileName) : "<无地图>";
        }
    }
}

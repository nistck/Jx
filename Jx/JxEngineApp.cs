using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Jx
{
    public class JxEngineApp
    {
        public delegate void TickDelegate();

        private static JxEngineApp instance = null;

        public static JxEngineApp Instance
        {
            get { return instance; }
        }

        private int loopInterval = 0;
 
        private float time = 0.0f;
        private float lastTime = 0.0f;

        private readonly object threadLock = new object(); 
        private Thread engineThread = null;
        private bool engineThreadQuit = false;

        public event TickDelegate Tick;

        /// <summary>
        /// 时间间隔，单位：毫秒
        /// </summary>
        /// <param name="loopInterval"></param>
        public JxEngineApp(int loopInterval)
        {
            this.loopInterval = loopInterval;
        }
 
        public int LoopInterval
        {
            get { return loopInterval < 0 ? 0 : loopInterval; }
        }

        /// <summary>
        /// 引擎时间, 单位: 毫秒
        /// </summary>
        public float Time
        {
            get { return this.time; }
            private set { this.time = value; }
        }
 
        public static bool Init(JxEngineApp overridedObject, IntPtr mainModuleData)
        {
            if (overridedObject == null)
            {
                Log.Fatal("EngineApp: Init: overridedObject == null.");
                return false;
            }
            if (instance != null)
            {
                Log.Fatal("EngineApp: Init: instance != null.");
                return false;
            }
            instance = overridedObject;
            bool flag = instance.DoInit(mainModuleData);
            if (!flag)
                Shutdown();            
            return flag;
        }

        public static bool Init(JxEngineApp overridedObject)
        {
            return Init(overridedObject, IntPtr.Zero);
        }

        public static void Shutdown()
        {
            if (instance != null)
            {
                instance.OnShutdown();
                instance.shutdown();
                instance = null;
            }
        }

        public bool Create()
        {
            bool result = OnCreate(); 
            return result;
        }

        public void Destroy()
        {
            OnDestroy();
        }

        public void Run()
        {
            lock(threadLock)
            {
                if (engineThread != null)
                    return; 
            }

            engineThread = new Thread(new ThreadStart(MainLoop));
            engineThread.Name = "JxEngineApp Main";
            engineThread.IsBackground = true;
            engineThread.Start();
        } 
 
        private void MainLoop()
        { 
            while (!engineThreadQuit && LoopInterval > 0)
            {
                try
                {
                    Thread.Sleep(LoopInterval);
                    Clock.Tick();
                    time += LoopInterval;
                    try { _Tick();  } catch (Exception) { }
                }
                catch { break; }
            }
        }

        /// <summary>
        /// 退出主线程
        /// </summary>
        public void SetMainLoopQuit()
        {
            if(engineThread != null)
            {
                engineThread.Interrupt();
                engineThread.Abort();
                engineThread = null; 
            }
            engineThreadQuit = true;
        }

        protected virtual bool OnCreate()
        {
            return true;
        }

        protected virtual void OnDestroy()
        {

        }

        /// <summary>
        /// EngineApp 准备退出 (Instance != null)
        /// </summary>
        protected virtual void OnShutdown()
        {

        }

        private void shutdown()
        {
            /*
            bb.A a = bb.A.Normal;
            if (this.Ug.IsWindowInitialized())
            {
                try
                {
                    a = this.Ug.GetWindowState();
                }
                catch
                {
                }
            }
            this.Destroy();
            if (this.UH)
            {
                if (!string.IsNullOrEmpty(EngineApp.tf))
                {
                    this.tF.Save();
                }
                if (EngineApp.tH)
                {
                    string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual("user:Configs/Engine.config");
                    string text;
                    TextBlock textBlock = TextBlockUtils.LoadFromRealFile(realPathByVirtual, out text);
                    if (textBlock == null)
                    {
                        textBlock = new TextBlock();
                    }
                    TextBlock textBlock2 = textBlock.FindChild("Renderer");
                    if (textBlock2 == null)
                    {
                        textBlock2 = textBlock.AddChild("Renderer");
                    }
                    Vec2I vec2I = this.VideoMode;
                    if (a == bb.A.Minimized && this.UJ != Vec2I.Zero)
                    {
                        vec2I = this.UJ;
                    }
                    if (EngineApp.th)
                    {
                        textBlock2.SetAttribute("fullScreen", this.FullScreen.ToString());
                        if (a == bb.A.Maximized && !this.FullScreen)
                        {
                            textBlock2.DeleteAttribute("videoMode");
                        }
                        else if (!this.FullScreen || this.ts)
                        {
                            textBlock2.SetAttribute("videoMode", vec2I.ToString());
                        }
                    }
                    try
                    {
                        string directoryName = Path.GetDirectoryName(realPathByVirtual);
                        if (directoryName != "" && !Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        using (StreamWriter streamWriter = new StreamWriter(realPathByVirtual))
                        {
                            streamWriter.Write(textBlock.DumpToString());
                        }
                    }
                    catch
                    {
                        Log.Warning("Unable to save file \"{0}\".", realPathByVirtual);
                    }
                }
            }
            NativeMemoryManager.a();
            //*/
        }
         
        private bool DoInit(IntPtr mainModuleData)
        {
 
            /*
            this.Ug = bb.Get();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            NativeLibraryManager.PreLoadLibrary("NativeMemoryManager");
            this.Ug.Init(mainModuleData);
            Log.InvisibleInfo("Powered by NeoAxis 3D Engine");
            string arg;
            if (PlatformInfo.Platform == PlatformInfo.Platforms.Windows)
            {
                arg = "Microsoft Windows";
            }
            else if (PlatformInfo.Platform == PlatformInfo.Platforms.MacOSX)
            {
                arg = "Apple Mac OS X";
            }
            else
            {
                if (PlatformInfo.Platform != PlatformInfo.Platforms.Android)
                {
                    Log.Fatal("EngineApp: InitInternal: Unknown platform.");
                    return false;
                }
                arg = "Google Android";
            }
            Log.InvisibleInfo("Operating System: {0} {1} ({2}-bit)", arg, PlatformInfo.OSVersion, PlatformInfo.System64Bit ? "64" : "32");
            Log.InvisibleInfo("Runtime Framework: " + RuntimeFramework.GetDisplayName());
            Log.InvisibleInfo("NeoAxis 3D Engine version: " + EngineVersionInformation.Version.ToString());
            Log.InvisibleInfo("Engine {0}-bit version", PlatformInfo.Engine64Bit ? "64" : "32");
            Log.InvisibleInfo("Application type: " + this.ApplicationType.ToString());
            this.tl = EngineApp.th;
            this.tS = this.Ug.GetScreenSize();
            this.tU = this.Ug.GetSystemTime();
            this.tW = (uint)this.tU;
            Array values = Enum.GetValues(typeof(EKeys));
            this.Uc = new EKeys[values.Length];
            for (int i = 0; i < this.Uc.Length; i++)
            {
                this.Uc[i] = (EKeys)values.GetValue(i);
            }
            string text;
            if (!string.IsNullOrEmpty(EngineApp.tf) && !this.tF.Init(EngineApp.tf, out text))
            {
                if (this.te == EngineApp.ApplicationTypes.ResourceEditor || this.te == EngineApp.ApplicationTypes.MapEditor)
                {
                    EditorMessageBox.Result result = EditorMessageBox.Show(text, "Warning", EditorMessageBox.Buttons.OKCancel, EditorMessageBox.Icon.Exclamation);
                    if (result == EditorMessageBox.Result.OK)
                    {
                        return true;
                    }
                }
                return false;
            }
            //*/
            return true;
        }

        private void NotifyTick()
        {
            try
            {
                if (Tick != null)
                    Tick();
            }
            catch (Exception) { }
        }

        private void _Tick()
        {
            NotifyTick();
            float time = this.Time;
            float delta = time - this.lastTime;
            if( delta != 0.0f)
            {
                this.lastTime = this.Time;
                OnTick(delta);
            }
        }

        protected virtual void OnTick(float delta)
        {
            
        }
    }
}

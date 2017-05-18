using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

using Jx;
using Jx.FileSystem;
using Jx.EntitySystem;
using Jx.MapSystem;
using Jx.Ext;
using Jx.Editors;

namespace JxDesign
{
    public class MapWorld
    {
        private static MapWorld instance = null; 
        private static List<string> recentlyLoadedMap = new List<string>();

 
        public static MapWorld Instance
        {
            get {
                if( instance == null )
                {
                    instance = new MapWorld();
                    instance.initConfig();
                }
                return instance;
            }
        }

        private static void RecordRecentlyLoadedMap(string p)
        {
            if (p == null)
                return;
            if (recentlyLoadedMap.Contains(p))
                recentlyLoadedMap.Remove(p);

            recentlyLoadedMap.Add(p);

#if _MAP_WORLD_
            XLog.debug("MapWorld.RecordRecentlyLoadedMap: {0}", p); 
#endif
        }

        private static void UpdateRecentlyLoadedMapIntoMenu()
        {
#if _MAP_WORLD_
            XLog.debug("MapWorld.UpdateRecentlyLoadedMapIntoMenu.."); 
#endif
        }

        public static List<string> RecentlyLoadedMapNames
        {
            get
            {
                List<string> result = new List<string>();
                result.AddRange(recentlyLoadedMap);
                return result;
            }
        }

        //-///////////////////////////////////////////////////////////////////
        public bool Modified { get; set; }
        public string MapTypeName { get; private set; } 

        private void initConfig()
        {            
            string p = string.Format("Base/Constants/{0}.config", Program.ExecutableName);
            Log.Info(">> 配置文件: {0}", p);

            if (VirtualFile.Exists(p))
            {
                TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(p);
                if (textBlock != null)
                    this.MapTypeName = textBlock.GetAttribute("mapTypeForNewMaps", typeof(DefaultMapType).Name);
            }

            Log.Info(">> 缺省地图类型: {0}", this.MapTypeName);
        }

        public bool New()
        {
            bool bx = SaveCurrent();
            if (!bx)
                return false;
            return CreateNew();
        }

        public bool SaveCurrent()
        {
            if (Map.Instance != null && Modified)
            {
                DialogResult dialogResult = ConfirmSave();
                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }
                if (dialogResult == DialogResult.Yes && !Save(Map.Instance.VirtualFileName))
                {
                    return false;
                }
            }
            return true;
        }

        public NewConfig AskForNewMapConfig()
        {
            NewConfig result = new NewConfig(); 
            result.State = false;


            result.MapDirectory = @"Map\Dx";
            result.State = true;

            /*
            dZ dZ = new dZ();
            if (dZ.ShowDialog() == DialogResult.OK)
            {
                result.MapName = dZ.MapName;
                result.CreateObjects = dZ.CreateObjects;
                result.HeightmapTerrain = dZ.HeightmapTerrain;
                result.Sun = dZ.Sun;
                result.SkyBox = dZ.SkyBox;
                result.Fog = dZ.Fog;
                result.MapCompositorManager = dZ.MapCompositorManager;
                result.SpawnPoint = dZ.SpawnPoint;

                result.State = true;
            }
            //*/

            return result;
        }

        public void DestroyWorld()
        {
            EntityWorld.Instance.ClearEntitySelection(true, true);
            UndoSystem.Instance.Clear();
 
            this.CloseLogicEditor();
            MapSystemWorld.MapDestroy();
            EntitySystemWorld.Instance.WorldDestroy();

            MainForm.Instance.NotifyUpdate();
        }
 
        public bool Load(string p)
        {
#if _MAP_WORLD_
            XLog.debug("MapWorld.Load, loading {0}", p); 
#endif
            bool result;
            using (new CursorKeeper(Cursors.WaitCursor))
            {
                if (!resetWorld())
                    return false;

                if (!MapSystemWorld.MapLoad(p))
                {
                    result = false;
                }
                else
                {
                    RecordRecentlyLoadedMap(p);
                    UpdateRecentlyLoadedMapIntoMenu();
                    result = true;
                }
                MainForm.Instance.NotifyUpdate(); 
                Modified = false;
            }

#if _MAP_WORLD_
            XLog.debug("MapWorld.Load, result = {0}", result); 
#endif
            return result;
        }

        private bool resetWorld()
        {
#if _MAP_WORLD_
            XLog.debug("MapWorld.resetWorld.."); 
#endif
            DestroyWorld();
            WorldType defaultWorldType = EntitySystemWorld.Instance.DefaultWorldType;
            if (!EntitySystemWorld.Instance.WorldCreate(WorldSimulationTypes.Editor, defaultWorldType))
            {
                Log.Fatal("EntitySystemWorld.Instance.WorldCreate failed.");
                return false;
            }
            return true;
        }

        public bool CreateNew()
        {
            NewConfig cfg = AskForNewMapConfig();
            if (!cfg.State)
                return false;

            if (!resetWorld())
                return false;

            if (string.IsNullOrEmpty(MapTypeName))
            {
                Log.Warning("MainForm: MapNew: Map type is not defined. (Base\\Constants\\MapEditor.config: \"mapTypeForNewMaps\" attribute)");
                return false;
            }
            MapType mapType = EntityTypes.Instance.GetByName(MapTypeName) as MapType;
            if (mapType == null)
            {
                Log.Fatal("MainForm: MapNew: Map type \"{0}\" is not defined or it is not a MapType (Base\\Constants\\MapEditor.config: \"mapTypeForNewMaps\" attribute).", this.MapTypeName);
                return false;
            }
            if (!MapSystemWorld.MapCreate(mapType))
            {
                Log.Fatal("MapSystemWorld.MapCreate failed.");
                return false;
            }
            /*
            if (cfg.CreateObjects)
            {
                if (cfg.HeightmapTerrain)
                {
                    Entity entity = Entities.Instance.Create("HeightmapTerrain", Map.Instance);
                    entity.PostCreate();
                }
                if (cfg.Sun)
                {
                    Sun sun = (Sun)Entities.Instance.Create("Sun", Map.Instance);
                    sun.Position = new Vec3(0f, 0f, 7f);
                    sun.Rotation = new Angles(319f, 6f, 94f).ToQuat();
                    sun.SpecularColor = new ColorValue(1f, 1f, 1f);
                    sun.PostCreate();
                }
                if (cfg.SkyBox)
                {
                    Entity entity2 = Entities.Instance.Create("SkyBox", Map.Instance);
                    entity2.PostCreate();
                }
                if (cfg.Fog)
                {
                    Fog fog = (Fog)Entities.Instance.Create("Fog", Map.Instance);
                    fog.Mode = FogMode.Exp2;
                    fog.ExpDensity = 0.004f;
                    fog.Color = new ColorValue(0.5764706f, 0.7607843f, 0.854901969f);
                    fog.PostCreate();
                }
                if (cfg.MapCompositorManager)
                {
                    Entity entity3 = Entities.Instance.Create("MapCompositorManager", Map.Instance);
                    entity3.PostCreate();
                }
                if (cfg.SpawnPoint)
                {
                    Entity entity4 = Entities.Instance.Create("SpawnPoint", Map.Instance);
                    entity4.PostCreate();
                }
                Map.Instance.EditorCameraPosition = new Vec3(2.551017f, -8.274409f, 7.529152f);
                Map.Instance.EditorCameraDirection = new SphereDir(1.961945f, -0.4550026f);
            }
            //*/
            MainForm.Instance.NotifyUpdate();

            Directory.CreateDirectory(VirtualFileSystem.GetRealPathByVirtual(cfg.MapDirectory));
            string text = Path.Combine(cfg.MapDirectory, "Map.map");
            Save(text);
            Modified = false;
            return true;
        }

        private void CloseLogicEditor()
        { 
            /*
            if (LogicEditorDockingForm.Instance != null)
            {
                LogicEditorDockingForm.Instance.Close();
            }
            //*/
        }

        public bool SaveAs()
        {
            return Save(null);
        }

        public string ChooseSavePath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Map.map";
            saveFileDialog.Filter = ToolsLocalization.Translate("Various", "Map files (*.map)|*.map|All files (*.*)|*.*");
            if (string.IsNullOrEmpty(Map.Instance.VirtualFileName))
            {
                saveFileDialog.InitialDirectory = VirtualFileSystem.GetRealPathByVirtual("Maps");
            }
            else
            {
                saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(VirtualFileSystem.GetRealPathByVirtual(Map.Instance.VirtualFileName));
            }
            saveFileDialog.RestoreDirectory = true;

            string result = null;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                result = saveFileDialog.FileName;

#if _MAP_WORLD_
            XLog.debug("MapWorld.ChooseSavePath => {0}", result); 
#endif
            return null;
        }

        public bool Save(string virtualPathByReal)
        {
            bool result;
            using (new CursorKeeper(Cursors.WaitCursor))
            {
                if (string.IsNullOrEmpty(virtualPathByReal))
                {
                    string p = ChooseSavePath();
                    if (p == null)
                        return false;

                    virtualPathByReal = VirtualFileSystem.GetVirtualPathByReal(p);
                    if (string.IsNullOrEmpty(virtualPathByReal))
                    {
                        Log.Warning(ToolsLocalization.Translate("Various", "Unable to save file. You cannot save map outside \"Data\" directory."));
                        result = false;
                        return result;
                    }
                }
                EntityWorld.Instance.ResetBeforeMapSave();
                UndoSystem.Instance.Clear();
                Map.Instance.GetDataForEditor().ClearDeletedEntities();
                bool flag;
                try
                {

                    MainForm.Instance.WatchFileSystem = false;
                    flag = MapSystemWorld.MapSave(virtualPathByReal, true);
                }
                finally
                {
                    MainForm.Instance.WatchFileSystem = true;
                }
                if (!flag)
                {
                    result = false;
                }
                else
                { 
                    Modified = false;
                    result = true;
                }
                MainForm.Instance.NotifyUpdate();
            }
            return result;
        }


        /// <summary>
        /// 打开一个 保存对话框 让用户选择
        /// </summary>
        /// <returns></returns>
        public DialogResult ConfirmSave()
        {
            if (Map.Instance == null)
                return DialogResult.No;

            string format = ToolsLocalization.Translate("Various", "Save map \"{0}\"?");
            string arg;
            if (!string.IsNullOrEmpty(Map.Instance.VirtualFileName))
                arg = Map.Instance.VirtualFileName;
            else
                arg = ToolsLocalization.Translate("Various", "Untitled");

            string text = string.Format(format, arg);
            string caption = ToolsLocalization.Translate("Various", "Map Editor");
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }


        private MapWorld() { }
    }
}

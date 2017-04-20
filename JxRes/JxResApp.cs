using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jx;
using Jx.Editors;
using Jx.EntitySystem;
using Jx.Ext;
using Jx.FileSystem;
using JxRes.Types;
using JxRes.UI;
using JxRes.Editors;

namespace JxRes
{
    internal class JxResApp : EngineApp
    {
        private static JxResApp instance;

        public new static JxResApp Instance
        {
            get { return instance; }
        }
         
        private ResourceObjectEditor atm;
        private string atn;
        private long atO;
        private bool ato;
        private bool atP;

        public ResourceObjectEditor ResourceObjectEditor
        {
            get { return atm; }
        }

        protected override bool OnCreate()
        {
            Log.Info(">> 初始化 EntitySystemWorld...");
            if (!EntitySystemWorld.Init(new EntitySystemWorld()))
            {
                Log.Info(">> EntitySystemWorld 初始化失败!");
                return false;
            }

            WorldType worldType = EntitySystemWorld.Instance.DefaultWorldType;
            if (!EntitySystemWorld.Instance.WorldCreate(WorldSimulationTypes.Editor, worldType))
            {
                Log.Info(">> 创建世界失败, WorldType: {0}", worldType);
                return false;
            }

            if (MainForm.Instance != null)
            {
                if (MainForm.Instance.ResourcesForm != null)
                {
                    MainForm.Instance.ResourcesForm.ResourceChange += new ResourcesForm.ResourceChangeDelegate(this.OnResourceChange);
                    MainForm.Instance.ResourcesForm.IsResourceEditModeActive += new ResourcesForm.IsResourceEditModeActiveDelegate(this.OnIsResourceEditMode);
                    MainForm.Instance.ResourcesForm.ResourceBeginEditMode += new ResourcesForm.ResourceBeginEditModeDelegate(this.OnResourceBeginEditMode);
                    MainForm.Instance.ResourcesForm.ResourceRename += new ResourcesForm.ResourceRenameDelegate(this.OnResourceRename);
                }

                if( MainForm.Instance.PropertiesForm != null)
                {
                    MainForm.Instance.PropertiesForm.ReadOnly = true;
                }
            }
            ResourceUtils.OnUITypeEditorEditValue += ResourceUtils_OnUITypeEditorEditValue;

            AddonManager.PreInit();
            InitResourceTypeManager();

            UndoSystem.Init(64);

            instance = this;
            return true;
        }

        private void ResourceUtils_OnUITypeEditorEditValue(ResourceUtils.ResourceUITypeEditorEditValueEventHandler e)
        {
            ResourceType byName = ResourceTypeManager.Instance.GetByName(e.ResourceTypeName);
            if (byName == null)
            {
                Log.Fatal("Resource type is not defined \"{0}\"", e.ResourceTypeName);
                return;
            }
   
            ChooseResourceForm chooseResourceForm = new ChooseResourceForm(byName, true, e.ShouldAddDelegate, e.ResourceName, e.SupportRelativePath);
            if (chooseResourceForm.ShowDialog() == DialogResult.OK)
            {
                e.ResourceName = chooseResourceForm.FilePath;
                e.Modified = true;
            } 
        }

        private void OnResourceRename(string path, ref bool ptr, ref string ptr2)
        {
            string text = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Substring(1);
                ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text);
                if (byExtension != null && byExtension.IsSpecialRenameResourceMode())
                {
                    ptr = true;
                    bool flag = false;
                    try
                    {
                        if (MainForm.Instance.ResourcesForm.WatchFileSystem)
                        {
                            MainForm.Instance.ResourcesForm.WatchFileSystem = false;
                            flag = true;
                        }
                        ptr2 = byExtension.DoUserRenameResource(path);
                    }
                    finally
                    {
                        if (flag)
                        {
                            MainForm.Instance.ResourcesForm.WatchFileSystem = true;
                        }
                    }
                }
            }
        }

        private void OnIsResourceEditMode(ResourcesForm.ActiveEventArgs activeEventArgs)
        {
            if (this.atm != null && this.atm.EditModeActive)
            {
                activeEventArgs.Active = true;
            }
        }

        private void OnResourceBeginEditMode(EventArgs eventArgs)
        {
            if (this.atm == null)
            {
                string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(this.atn);
                Shell32Api.ShellExecuteEx(null, realPathByVirtual);
                return;
            }
            if (!this.atm.AllowEditMode)
            {
                string realPathByVirtual2 = VirtualFileSystem.GetRealPathByVirtual(this.atn);
                Shell32Api.ShellExecuteEx(null, realPathByVirtual2);
                return;
            }
            if (VirtualFile.IsInArchive(this.atm.FileName))
            {
                Log.Warning(ToolsLocalization.Translate("Various", "This file is inside an archive. Unable to edit it."));
                return;
            }
            this.atm.BeginEditMode();

            Log.Info("MainForm.Instance.EngineAppControl.Focus();");            
        }

        private void OnResourceChange(string fileName, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = !this.ChangeResourceObjectEditor(fileName);
        }


        public bool ChangeResourceObjectEditor(string fileName)
        {
            this.atn = fileName;
            this.atO = 0L;
            try
            {
                if (!string.IsNullOrEmpty(this.atn))
                {
                    this.atO = VirtualFile.GetLength(this.atn);
                }
            }
            catch
            {
            }
            this.ato = false;
            try
            {
                if (!string.IsNullOrEmpty(this.atn))
                {
                    this.ato = VirtualFile.IsArchive(this.atn);
                }
            }
            catch
            {
            }
            this.atP = false;
            try
            {
                if (!string.IsNullOrEmpty(this.atn))
                {
                    this.atP = VirtualFile.IsInArchive(this.atn);
                }
            }
            catch
            {
            }
            if (this.atm != null)
            {
                if (fileName != null && string.Compare(this.atm.FileName, fileName, true) == 0)
                {
                    return true;
                }
                if (this.atm.EditModeActive && !this.atm.EndEditMode())
                {
                    return false;
                }
                this.atm.Dispose();
                this.atm = null;
            }
            if (fileName != null)
            {
                string text = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Substring(1);
                    if (ResourceTypeManager.Instance != null)
                    {
                        ResourceType byExtension = ResourceTypeManager.Instance.GetByExtension(text);
                        if (byExtension != null)
                        {
                            Type resourceObjectEditorType = byExtension.ResourceObjectEditorType;
                            ConstructorInfo constructor = resourceObjectEditorType.GetConstructor(new Type[0]);
                            this.atm = (ResourceObjectEditor)constructor.Invoke(new object[0]);
                            this.atm.Create(byExtension, fileName);
                        }
                    }
                }
            }
            if (MainForm.Instance != null && this.atm == null)
            {
                MainForm.Instance.PropertiesForm.SelectObjects(null);
            }
            return true;
        }

        private void InitResourceTypeManager()
        {
            ResourceTypeManager.Init();

            ResourceTypeManager.Instance.Register(new EntityTypeResourceType("EntityType", "Entity Type", new string[]
            {
                "type"
            }, DefaultResourceTypeImages.GetByName("Config_16")));

            ResourceTypeManager.Instance.Register(new ConfigurationResourceType("Configuration", "Configuration File", new string[]
            {
                "config"
            }, DefaultResourceTypeImages.GetByName("Config_16")));


        }
    }
}

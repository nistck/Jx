using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

using Jx;
using Jx.FileSystem;
using Jx.Editors;
using Jx.EntitySystem;
using JxRes.Editors;
using JxRes.UI;

namespace JxRes.Types
{
    internal class EntityTypeResourceType : ResourceType
    {
        public override Type ResourceObjectEditorType
        {
            get
            {
                return typeof(EntityTypeResourceEditor);
            }
        }
        public override bool AllowNewResource
        {
            get
            {
                return true;
            }
        }
        public EntityTypeResourceType(string name, string displayName, string[] extensions, Image icon)
            : base(name, displayName, extensions, icon)
        {
            //EngineApp.Instance.Config.RegisterClassParameters(base.GetType());
        }

        protected override void OnNewResource(string directory)
        {
            base.OnNewResource(directory);
            EntityTypeNewResourceDialog cd = new EntityTypeNewResourceDialog(directory);
            if (cd.ShowDialog(MainForm.Instance) != DialogResult.OK)
                return;
            
            string text = "";
            if (directory != "")
            {
                text = text + directory + "\\";
            }
            text = text + cd.TypeName + ".type";
            TextBlock textBlock = new TextBlock();
            TextBlock typeBlock = textBlock.AddChild("type", cd.TypeName);
            typeBlock.SetAttribute("class", cd.TypeClass.EntityClassType.Name);
            try
            {
                using (Stream stream = new FileStream(VirtualFileSystem.GetRealPathByVirtual(text), FileMode.Create))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.Write(textBlock.DumpToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                return;
            }
            if (EntityTypes.Instance.LoadTypeFromFile(text) != null)
            {
                MainForm.Instance.ResourcesForm.UpdateAddResource(text);
                MainForm.Instance.ResourcesForm.SelectNodeByPath(text);
                return;
            }
            Log.Fatal("OnNewResource: EntityTypes.Instance.LoadType: Internal error.");
        }

        protected override bool OnLoadResource(string path)
        {
            return EntityTypes.Instance.LoadTypeFromFile(path) != null && base.OnLoadResource(path);
        }

        protected override bool OnUnloadResource(string path)
        {
            EntityType entityType = EntityTypes.Instance.FindByFilePath(path);
            if (entityType != null)
            {
                EntityTypes.Instance.DestroyType(entityType);
                EntityTypes.Instance.ChangeAllReferencesToType(entityType, null);
            }
            return base.OnUnloadResource(path);
        }

        public override bool IsSpecialRenameResourceMode()
        {
            return true;
        }

        public override bool OnResourceRenamed(string path, string oldPath)
        {
            try
            {
                EntityType entityType = EntityTypes.Instance.FindByFilePath(oldPath);
                if (entityType == null)
                {
                    bool result = false;
                    return result;
                }
                string name = entityType.Name;
                if (name == "")
                {
                    bool result = false;
                    return result;
                }
                EntityTypes.Instance.DestroyType(entityType);
                EntityType entityType2 = EntityTypes.Instance.LoadTypeFromFile(path);
                if (entityType2 == null)
                {
                    Log.Fatal("EntityTypeResourceType: OnRenameResource: EntityTypes.LoadType failed.");
                }
                EntityTypes.Instance.ChangeAllReferencesToType(entityType, entityType2);
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                bool result = false;
                return result;
            }
            return true;
        }

        private void A(string text, string virtualPath, string data)
        {
            TextBlock textBlock;
            try
            {
                string message;
                textBlock = TextBlockUtils.LoadFromVirtualFile(text, out message);
                if (textBlock == null)
                {
                    throw new Exception(message);
                }
                if (textBlock.Children.Count == 0)
                {
                    throw new Exception("Invalid format");
                }
                TextBlock textBlock2 = textBlock.Children[0];
                if (string.Compare(textBlock2.Name, "type", true) != 0)
                {
                    throw new Exception("Invalid format");
                }
                textBlock2.Data = data;
            }
            catch (Exception ex)
            {
                Log.Warning(this.A("Unable to load entity type \"{0}\" ({1})."), text, ex.Message);
                return;
            }
            string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(virtualPath);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(realPathByVirtual))
                {
                    streamWriter.Write(textBlock.DumpToString());
                }
            }
            catch (Exception ex2)
            {
                Log.Warning(this.A("Unable to save file \"{0}\" ({1})."), realPathByVirtual, ex2.Message);
            }
        }

        protected override string OnUserRenameResource(string path)
        {
            EntityType entityType = EntityTypes.Instance.FindByFilePath(path);
            if (entityType == null)
            {
                return null;
            }
            string resourceName = entityType.Name;
            if (resourceName == "")
            {
                return null;
            }
            string directory = Path.GetDirectoryName(path);
            OKCancelTextBoxDialog oKCancelTextBoxDialog = new OKCancelTextBoxDialog(
                resourceName, this.A("Name:"), this.A("Rename Resource"), delegate (string newName)
            {
                string text4 = newName.Trim();
                if (text4 == resourceName)
                {
                    return true;
                }
                if (!PathUtils.IsCorrectFileName(text4 + ".type"))
                {
                    Log.Warning(this.A("Invalid name."));
                    return false;
                }
                if (EntityTypes.Instance.GetByName(text4) != null)
                {
                    Log.Warning(this.A("The resource with such name is already exists."));
                    return false;
                }
                string text5 = Path.Combine(directory, text4 + ".type");
                if (string.Compare(path, text5, true) != 0 && VirtualFile.Exists(text5))
                {
                    Log.Warning(this.A("The file with MENU_Close_Clicked name \"{0}\" is already exists."), path);
                    return false;
                }
                return true;
            });
            //oKCancelTextBoxDialog.UpdateFonts(MainForm.fontForm);
            if (oKCancelTextBoxDialog.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            string result;
            try
            {
                string text = oKCancelTextBoxDialog.TextBoxText.Trim();
                if (resourceName == text)
                {
                    result = path;
                }
                else
                {
                    string text2 = text + ".type";
                    string text3 = Path.Combine(directory, text2);
                    this.A(path, text3, text);
                    List<EntityType> list = EntityTypes.Instance.FindTypesWhoHasReferenceToType(entityType);
                    list.Remove(entityType);
                    if (!this.OnResourceRenamed(text3, path))
                    {
                        result = null;
                    }
                    else
                    {
                        File.Delete(VirtualFileSystem.GetRealPathByVirtual(path));
                        bool flag = false;
                        try
                        {
                            if (MainForm.Instance.ResourcesForm.WatchFileSystem)
                            {
                                MainForm.Instance.ResourcesForm.WatchFileSystem = false;
                                flag = true;
                            }
                            foreach (EntityType current in list)
                            {
                                if (!current.ManualCreated)
                                {
                                    EntityTypes.Instance.SaveTypeToFile(current);
                                }
                            }
                        }
                        finally
                        {
                            if (flag)
                            {
                                MainForm.Instance.ResourcesForm.WatchFileSystem = true;
                            }
                        }
                        result = text2;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
                result = null;
            }
            return result;
        }

        protected override bool OnOutsideAddResource(string path)
        {
            string data;
            try
            {
                string message;
                TextBlock textBlock = TextBlockUtils.LoadFromVirtualFile(path, out message);
                if (textBlock == null)
                {
                    throw new Exception(message);
                }
                if (textBlock.Children.Count == 0)
                {
                    throw new Exception("Invalid format");
                }
                TextBlock textBlock2 = textBlock.Children[0];
                if (string.Compare(textBlock2.Name, "type", true) != 0)
                {
                    throw new Exception("Invalid format");
                }
                data = textBlock2.Data;
            }
            catch (Exception ex)
            {
                Log.Warning(this.A("Unable to load entity type \"{0}\" ({1})."), path, ex.Message);
                return false;
            }
            if (data != "")
            {
                int num = 1;
                string text;
                while (true)
                {
                    text = data;
                    if (num != 1)
                    {
                        text += num.ToString();
                    }
                    if (EntityTypes.Instance.GetByName(text) == null)
                    {
                        break;
                    }
                    num++;
                }
                this.A(path, path, text);
            }
            return base.OnOutsideAddResource(path);
        }

        private string A(string text)
        {
            return ToolsLocalization.Translate("EntityTypeResourceType", text);
        }
    }

}

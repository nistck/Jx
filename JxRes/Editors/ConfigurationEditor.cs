using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jx;
using Jx.FileSystem;
using Jx.Editors;

namespace JxRes.Editors
{
    internal class ConfigurationEditor : ResourceObjectEditor
    {
        private TextBlock textBlock;

        public override void Create(ResourceType resourceType, string fileName)
        {
            base.Create(resourceType, fileName);
            this.textBlock = TextBlockUtils.LoadFromVirtualFile(base.FileName);
            if (this.textBlock != null)
            {
                base.AllowEditMode = true;
            }

            /*
            MainForm.Instance.PropertiesForm.SelectObjects(new object[]
            {
                this
            });
            //*/
        }

        protected override void OnBeginEditMode()
        {
            base.OnBeginEditMode();
            /*
            TextBlockEditor textBlockEditor = new TextBlockEditor(this, this.aWW);
            textBlockEditor.ShowDialog();
            while (!base.EndEditMode())
            {
                textBlockEditor = new TextBlockEditor(this, this.aWW);
                textBlockEditor.ShowDialog();
            }

            //*/
        }

        protected override bool OnSave()
        {
            //MainForm.Instance.ResourcesForm.WatchFileSystem = false;
            string value = this.textBlock.DumpToString();
            try
            {
                string realPathByVirtual = VirtualFileSystem.GetRealPathByVirtual(base.FileName);
                using (StreamWriter streamWriter = new StreamWriter(realPathByVirtual))
                {
                    streamWriter.Write(value);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to save file \"{0}\" ({1}).", base.FileName, ex.Message);
                return false;
            }
            finally
            {
                //MainForm.Instance.ResourcesForm.WatchFileSystem = true;
            }
            return base.OnSave();
        }

        protected override bool OnEndEditMode()
        {
            return base.OnEndEditMode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using Jx;
using Jx.FileSystem;
using Jx.Editors;

namespace JxRes.Editors 
{
    public class ResourceObjectEditor : IDisposable
    {
        protected static DateTime lastSaveTime = DateTime.Now;
        private ResourceType resourceType;
        private string fileName;
        private bool allowEditMode;
        private bool editModeActive;
        private bool modified; 
        private bool aqD; 
        private bool aqE;
        private bool isInArchive;
        private Color backgroundColor;
 
        public event EventHandler ModifiedSet;

        [Browsable(false)]
        public ResourceType ResourceType
        {
            get
            {
                return resourceType;
            }
        }

        [Browsable(false)]
        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        [Browsable(false)]
        public bool AllowEditMode
        {
            get
            {
                return allowEditMode;
            }
            set
            {
                this.allowEditMode = value;
            }
        }

        [Browsable(false)]
        public bool EditModeActive
        {
            get
            {
                return editModeActive;
            }
        }
        [Browsable(false)]
        public bool Modified
        {
            get
            {
                return this.modified;
            }
            set
            {
                this.modified = value;
                this.OnModifiedSet();
                if (ModifiedSet != null)
                    ModifiedSet(this, EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
            }
        }
        public virtual void Create(ResourceType resourceType, string fileName)
        {
            this.resourceType = resourceType;
            this.fileName = fileName;
            this.isInArchive = VirtualFile.IsInArchive(fileName);
        }
        public virtual void Dispose()
        {
            /*
            if (MainForm.Instance != null && MainForm.Instance.PropertiesForm != null)
            {
                MainForm.Instance.PropertiesForm.ModalDialogCollectionEditorOK -= new PropertiesForm.ModalDialogCollectionEditorOKDelegate(this.A);
                MainForm.Instance.PropertiesForm.PropertyValueChanged -= new PropertiesForm.PropertyValueChangeDelegate(this.A);
            }
            ChooseResourceForm.CurrentHelperDirectoryName = null;
            //*/
        }

        protected virtual void OnBeginEditMode()
        {
            if( !this.editModeActive )
            {
                MainForm.Instance.PropertiesForm.PropertyValueChanged += PropertiesForm_PropertyValueChanged;
            }
            this.editModeActive = true;
            MainForm.Instance.PropertiesForm.ReadOnly = false;

            /*
            if (!this.editModeActive)
            {
                MainForm.Instance.PropertiesForm.ModalDialogCollectionEditorOK += new PropertiesForm.ModalDialogCollectionEditorOKDelegate(this.A);
                MainForm.Instance.PropertiesForm.PropertyValueChanged += new PropertiesForm.PropertyValueChangeDelegate(this.A);
            }
            this.editModeActive = true;
            MainForm.Instance.PropertiesForm.ReadOnly = false;
            if (UndoSystem.Instance != null)
            {
                UndoSystem.Instance.Clear();
            }
            //*/
        }

        private void PropertiesForm_PropertyValueChanged(object sender, GridItem item, object oldValue)
        {
            this.Modified = true;
        }

        protected virtual bool OnEndEditMode()
        {
            if (this.modified)
            {
                string format = ToolsLocalization.Translate("Various", "Save resource \"{0}\"?");
                string text = string.Format(format, this.ToString());
                string caption = ToolsLocalization.Translate("Various", "Resource Editor");
                DialogResult dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }
                if (dialogResult == DialogResult.Yes && !this.DoSave())
                {
                    return false;
                }
            }
            this.editModeActive = false;
            MainForm.Instance.PropertiesForm.ReadOnly = true;
            MainForm.Instance.PropertiesForm.PropertyValueChanged -= PropertiesForm_PropertyValueChanged;

            return true;

            /*
            if (this.modified)
            {
                string format = ToolsLocalization.Translate("Various", "Save resource \"{0}\"?");
                string text = string.Format(format, this.ToString());
                string caption = ToolsLocalization.Translate("Various", "Resource Editor");
                DialogResult dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }
                if (dialogResult == DialogResult.Yes && !this.DoSave())
                {
                    return false;
                }
            }
            if (UndoSystem.Instance != null)
            {
                UndoSystem.Instance.Clear();
            }
            MainForm.Instance.PropertiesForm.ModalDialogCollectionEditorOK -= new PropertiesForm.ModalDialogCollectionEditorOKDelegate(this.A);
            MainForm.Instance.PropertiesForm.PropertyValueChanged -= new PropertiesForm.PropertyValueChangeDelegate(this.A);
            MainForm.Instance.ResourcesForm.Focus();
            MainForm.Instance.ResourcesForm.TreeViewSelect();
            this.editModeActive = false;
            MainForm.Instance.PropertiesForm.ReadOnly = true;
            return true;
            //*/
        }

        private void A()
        {
            this.Modified = true;
        }

        private void A(object obj, GridItem gridItem, object obj2)
        {
            this.Modified = true;
        }

        protected virtual bool OnSave()
        {
            return true;
        }

        /*
        protected virtual bool OnKeyDown(KeyEvent e)
        {
            return false;
        }

        protected virtual bool OnKeyPress(KeyPressEvent e)
        {
            return false;
        }

        protected virtual bool OnKeyUp(KeyEvent e)
        {
            return false;
        }

        protected virtual bool OnMouseDown(EMouseButtons button)
        {
            if (button == EMouseButtons.Left && this.OnIsAllowRectangleSelection())
            {
                this.aqD = true;
                this.aqE = false;
                this.aqd = this.OnGetMousePositionForRectangleSelection();
                return true;
            }
            return false;
        }

        protected virtual bool OnMouseUp(EMouseButtons button)
        {
            if (button == EMouseButtons.Left && this.aqD)
            {
                if (this.aqE && this.OnIsAllowRectangleSelection())
                {
                    this.OnRectangleSelectionComplete();
                }
                this.aqD = false;
            }
            return false;
        }
        //*/

        protected virtual void OnMouseMove(/*Vec2 mouse*/)
        {
            /*
            if (this.aqD)
            {
                Vec2 vec = (this.OnGetMousePositionForRectangleSelection() - this.aqd) * EngineApp.Instance.VideoMode.ToVec2();
                if (Math.Abs(vec.X) >= 3f || Math.Abs(vec.Y) >= 3f)
                {
                    this.aqE = true;
                }
            }
            //*/
        }

        protected virtual bool OnMouseWheel(int delta)
        {
            return false;
        }

        protected virtual void OnTick(float delta)
        {
        }
         
        public void BeginEditMode()
        {
            this.OnBeginEditMode();
        }

        public bool EndEditMode()
        {
            bool flag = this.OnEndEditMode();
            if (flag)
            {
                this.modified = false;
            }
            return flag;
        }

        /*
        public bool DoKeyDown(KeyEvent e)
        {
            return this.OnKeyDown(e);
        }

        public bool DoKeyPress(KeyPressEvent e)
        {
            return this.OnKeyPress(e);
        }

        public bool DoKeyUp(KeyEvent e)
        {
            return this.OnKeyUp(e);
        }

        public bool DoMouseDown(EMouseButtons button)
        {
            return this.OnMouseDown(button);
        }

        public bool DoMouseUp(EMouseButtons button)
        {
            return this.OnMouseUp(button);
        }

        public void DoMouseMove(Vec2 mouse)
        {
            this.OnMouseMove(mouse);
        }

        public bool DoMouseWheel(int delta)
        {
            return this.OnMouseWheel(delta);
        }
        //*/

        public void DoTick(float delta)
        {
            this.OnTick(delta);
        }
        
        public override string ToString()
        {
            string str = ToolsLocalization.Translate("Various", this.resourceType.DisplayName);
            return str + ": " + this.fileName;
        }
        protected virtual void OnModifiedSet()
        {
        }
        public virtual void OnContextMenuCreate(ContextMenuStrip menu)
        {
        } 

        public bool DoSave()
        {
            if (!this.OnSave())
            {
                return false;
            }
            //ResourceEditorEngineApp.Instance.AddScreenMessage(ToolsLocalization.Translate("Various", "Resource saved"));
            this.modified = false;
            ResourceObjectEditor.lastSaveTime = DateTime.Now;
            return true;
        }
        public virtual bool IsSupportUndo()
        {
            return false;
        }
        public virtual bool IsDisableUndoActionsTemporarily()
        {
            return false;
        }
        public virtual bool IsEnableMenuItemClone()
        {
            return false;
        }
        public virtual bool IsEnableMenuItemDelete()
        {
            return false;
        }
        public virtual void OnMenuItemClone()
        {
        }
        public virtual void OnMenuItemDelete()
        {
        }
        public virtual object OnResourceEditorInterfaceMessage(string message, object param)
        {
            return null;
        }
 
        protected virtual bool OnToolsProcessKeyDownHotKeys(Keys keyCode, Keys modifiers, bool processCharactersWithoutModifiers)
        {
            return false;
        }
        public bool CallOnToolsProcessKeyDownHotKeys(Keys keyCode, Keys modifiers, bool processCharactersWithoutModifiers)
        {
            return this.OnToolsProcessKeyDownHotKeys(keyCode, modifiers, processCharactersWithoutModifiers);
        }
    }

}

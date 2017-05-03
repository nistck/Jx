using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Reflection;

using Jx.UI.Editors;
using Jx.Ext;

using Jx.UI.Controls.PGEx;

namespace Jx.UI.Forms
{
    public partial class PropertiesForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public delegate void ModalDialogCollectionEditorOKDelegate();
        public delegate void PropertyValueChangeDelegate(object sender, GridItem item, object oldValue);
        public delegate void SelectedGridItemChangedDelegate(object sender, GridItem newSelection);
        public delegate void ProcessCmdKeyEventDelegate(PropertiesForm sender, ref Message msg, Keys keyData, ref bool handled);
        public delegate void ContextMenuOpeningDelegate(ContextMenuStrip menu);

        public event ModalDialogCollectionEditorOKDelegate ModalDialogCollectionEditorOK;
        public event PropertyValueChangeDelegate PropertyValueChanged;
        public event SelectedGridItemChangedDelegate SelectedGridItemChanged;
        public event EventHandler PropertyItemDoubleClick;
        public event ProcessCmdKeyEventDelegate ProcessCmdKeyEvent;
        public event ContextMenuOpeningDelegate ContextMenuOpening;
          
        private bool showPropertyGridNextTimerTick;
        private Type forRefreshExtendedFunctionalityDescriptorType;
        private object forRefreshExtendedFunctionalityDescriptorOwner;
        private ExtendedFunctionalityDescriptor extendedFunctionalityDescriptor;
        private List<Form> openedCollectionEditorForms = new List<Form>(); 
 
        private Font propertyGrid1FontOriginal;
        private string propertyGrid1FontCurrent = "";
        private Font extendedPropertiesFontOriginal;
        private string extendedPropertiesFontCurrent = "";

        private ImageCache imageCache = null;

        public PropertiesForm()
        {
            InitializeComponent();
        }

        private void PropertiesForm_Load(object sender, EventArgs e)
        {
            imageCache = new ImageCache(IL16);

            this.resetToolStripMenuItem.Image = imageCache["reset"];

            UpdateSplitterPosition();
        }

        public bool ReadOnly
        {
            get
            {
                return this.jxPropertyGrid.ReadOnly;
            }
            set
            {
                this.jxPropertyGrid.ReadOnly = value;
            }
        }

        public GridItem SelectedGridItem
        {
            get
            {
                return this.jxPropertyGrid.SelectedGridItem;
            }
        }


        public void OnClose()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
            base.OnClosed(e);
        } 

        private static PropertyGrid GetParentPropertyGridOfCollectionEditor(CollectionEditor editor)
        {
            PropertyGrid result = null;
            try
            {
                PropertyInfo property = editor.GetType().GetProperty("Context", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (property == null)
                    return null;

                object value = property.GetValue(editor, null);
                if (value == null)
                    return null;

                PropertyInfo property2 = value.GetType().GetProperty("OwnerGrid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (property2 == null)
                    return null;
                result = (property2.GetValue(value, null) as PropertyGrid);
            }
            catch { }
            return result;
        } 

        private bool IsThisCollectionEditorForm(Form form)
        {
            if (form.GetType().Name != "CollectionEditorCollectionForm")
            {
                return false;
            }
            FieldInfo field = form.GetType().GetField("editor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                return false;
            }
            CollectionEditor collectionEditor = (CollectionEditor)field.GetValue(form);
            if (collectionEditor == null)
            {
                return false;
            }
            PropertyGrid parentPropertyGridOfCollectionEditor = PropertiesForm.GetParentPropertyGridOfCollectionEditor(collectionEditor);
            return parentPropertyGridOfCollectionEditor != null && (parentPropertyGridOfCollectionEditor == jxPropertyGrid || (this.openedCollectionEditorForms.Count != 0 && parentPropertyGridOfCollectionEditor.Parent != null && parentPropertyGridOfCollectionEditor.Parent.Parent == this.openedCollectionEditorForms[this.openedCollectionEditorForms.Count - 1]));
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.showPropertyGridNextTimerTick)
            {
                this.jxPropertyGrid.Visible = true;
                this.showPropertyGridNextTimerTick = false;
            }
            Form activeForm = Form.ActiveForm;
            if (activeForm != null && this.IsThisCollectionEditorForm(activeForm) && !this.openedCollectionEditorForms.Contains(activeForm))
            {
                activeForm.FormClosed += new FormClosedEventHandler(this.form_FormClosed);
                this.openedCollectionEditorForms.Add(activeForm);
            }
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = (Form)sender;
            if (form.DialogResult == DialogResult.OK)
            {
                PropertyGrid_ModalDialogCollectionEditorOK();
            }
            openedCollectionEditorForms.Remove(form);
        }

        private void PropertyGrid_ModalDialogCollectionEditorOK()
        {
            this.jxPropertyGrid.Refresh();
            if (ModalDialogCollectionEditorOK != null)
            {
                ModalDialogCollectionEditorOK();
            }
        }


        private void propertyGrid1_GridItemDoubleClick(object sender, EventArgs e)
        {
            if (this.PropertyItemDoubleClick != null)
            {
                this.PropertyItemDoubleClick(sender, e);
            }
        }
        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (this.SelectedGridItemChanged != null)
            {
                this.SelectedGridItemChanged(sender, e.NewSelection);
            }
        }

        public object[] GetSelectedObjects()
        {
            return this.jxPropertyGrid.SelectedObjects;
        }


        public bool SelectObjects(object[] objects, bool updateExtendedFunctionalityDescriptor)
        {
            if (objects != null && objects.Length == 0)
                objects = null;
            
            if (objects == null && this.jxPropertyGrid.SelectedObject == null)
                return false;
            
            if (objects != null && this.jxPropertyGrid.SelectedObjects != null && objects.Length == this.jxPropertyGrid.SelectedObjects.Length)
            {
                int num = 0;
                while (num < objects.Length && objects[num] == this.jxPropertyGrid.SelectedObjects[num])
                    num++;
                if (num == objects.Length)
                    return false;
            }

            if (updateExtendedFunctionalityDescriptor && this.extendedFunctionalityDescriptor != null)
            {
                this.extendedFunctionalityDescriptor.Dispose();
                this.extendedFunctionalityDescriptor = null;
            }
            if (objects != null)
            {
                this.jxPropertyGrid.SelectedObjects = objects;
            }
            else
            {
                this.jxPropertyGrid.SelectedObject = null;
            }
            if (updateExtendedFunctionalityDescriptor)
            {
                this.SplitView.Panel2.Controls.Clear();
                this.forRefreshExtendedFunctionalityDescriptorType = null;
                this.forRefreshExtendedFunctionalityDescriptorOwner = null;
                if (objects != null && objects.Length == 1)
                {
                    object obj = objects[0];
                    ExtendedFunctionalityDescriptorCustomTypeDescriptor extendedFunctionalityDescriptorCustomTypeDescriptor = obj as ExtendedFunctionalityDescriptorCustomTypeDescriptor;
                    if (extendedFunctionalityDescriptorCustomTypeDescriptor != null)
                    {
                        object extendedFunctionalityDescriptorObject = extendedFunctionalityDescriptorCustomTypeDescriptor.GetExtendedFunctionalityDescriptorObject();
                        if (extendedFunctionalityDescriptorObject != null)
                        {
                            Type extendedFunctionalityDescriptorType = extendedFunctionalityDescriptorCustomTypeDescriptor.GetExtendedFunctionalityDescriptorType(extendedFunctionalityDescriptorObject);
                            if (extendedFunctionalityDescriptorType != null)
                            {
                                this.CreateExtendedFunctionalityDescriptor(extendedFunctionalityDescriptorType, extendedFunctionalityDescriptorObject);
                            }
                        }
                    }
                }
                this.UpdateSplitterPosition();
            }
            return true;
        }


        public void SelectObjects(params object[] objects)
        {
            this.SelectObjects(objects, true);
        }

        public ExtendedFunctionalityDescriptor CreateExtendedFunctionalityDescriptor(Type extendedFunctionalityDescriptorType, object owner)
        {
            if (this.extendedFunctionalityDescriptor != null)
            {
                this.extendedFunctionalityDescriptor.Dispose();
                this.extendedFunctionalityDescriptor = null;
            }
            this.forRefreshExtendedFunctionalityDescriptorType = extendedFunctionalityDescriptorType;
            this.forRefreshExtendedFunctionalityDescriptorOwner = owner;
            if (this.jxPropertyGrid.SelectedObjects.Length != 1)
            {
                throw new Exception("propertyGrid1.SelectedObjects.Length != 1");
            }
            this.SplitView.Panel2.Controls.Clear();
            ConstructorInfo constructor = extendedFunctionalityDescriptorType.GetConstructor(new Type[]
            {
                typeof(Control),
                typeof(object)
            });
            this.extendedFunctionalityDescriptor = (ExtendedFunctionalityDescriptor)constructor.Invoke(new object[]
            {
                this.SplitView.Panel2,
                owner
            });
            this.SplitView.Panel2Collapsed = this.SplitView.Panel2.Controls.Count == 0;

            this.UpdateSplitterPosition();
            return this.extendedFunctionalityDescriptor;
        }

        public void DestroyExtendedFunctionalityDescriptor()
        {
            if (this.extendedFunctionalityDescriptor != null)
            {
                this.extendedFunctionalityDescriptor.Dispose();
                this.extendedFunctionalityDescriptor = null;
            }
            this.UpdateSplitterPosition();
        }

        public void SelectObject(object obj)
        {
            jxPropertyGrid.SelectedObject = obj;
        }

        public void RefreshProperties()
        {
            this.jxPropertyGrid.Refresh();
        } 

        private void UpdateSplitterPosition()
        {
            int num = 0;
            foreach (Control control in this.SplitView.Panel2.Controls)
            {
                int num2 = control.Location.Y + control.Size.Height + 8 + 8;
                if (num2 > num)
                {
                    num = num2;
                }
            }
            try
            {
                if (!this.SplitView.Panel1Collapsed && !this.SplitView.Panel2Collapsed)
                {
                    int num3 = this.SplitView.Size.Height - num;
                    if (num3 >= 0)
                    {
                        if (this.SplitView.Orientation == Orientation.Vertical)
                        {
                            if (num3 < this.SplitView.Panel1MinSize)
                            {
                                num3 = this.SplitView.Panel1MinSize;
                            }
                            if (num3 + this.SplitView.SplitterWidth > this.SplitView.Width - this.SplitView.Panel2MinSize)
                            {
                                num3 = this.SplitView.Width - this.SplitView.Panel2MinSize - this.SplitView.SplitterWidth;
                            }
                        }
                        else
                        {
                            if (num3 < this.SplitView.Panel1MinSize)
                            {
                                num3 = this.SplitView.Panel1MinSize;
                            }
                            if (num3 + this.SplitView.SplitterWidth > this.SplitView.Height - this.SplitView.Panel2MinSize)
                            {
                                num3 = this.SplitView.Height - this.SplitView.Panel2MinSize - this.SplitView.SplitterWidth;
                            }
                        }
                        if (num3 >= 0)
                        {
                            this.SplitView.SplitterDistance = num3;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void jxPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (PropertyValueChanged != null)
                PropertyValueChanged(s, e.ChangedItem, e.OldValue);
        }

        public void AddPropertyTabType(Type tabType)
        {
            this.jxPropertyGrid.PropertyTabs.AddTabType(tabType);
        }

        private void contextMenuStripPropertyGrid_Opening(object sender, CancelEventArgs e)
        {
            GridItem selectedGridItem = this.jxPropertyGrid.SelectedGridItem;
            bool enabled = false;
            if (selectedGridItem != null && selectedGridItem.PropertyDescriptor != null)
            {
                PropertyDescriptor propertyDescriptor = selectedGridItem.PropertyDescriptor;
                try
                {
                    if (propertyDescriptor.GetType().Name == "MergePropertyDescriptor")
                    {
                        FieldInfo field = propertyDescriptor.GetType().GetField("descriptors", BindingFlags.Instance | BindingFlags.NonPublic);
                        PropertyDescriptor[] array = (PropertyDescriptor[])field.GetValue(propertyDescriptor);
                        for (int i = 0; i < array.Length; i++)
                        {
                            PropertyDescriptor propertyDescriptor2 = array[i];
                            if (propertyDescriptor2.CanResetValue(this.jxPropertyGrid.SelectedObjects[i]))
                            {
                                enabled = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        object component = null;
                        GridItem parent = selectedGridItem.Parent;
                        if (parent.GridItemType == GridItemType.Category || parent.GridItemType == GridItemType.Root)
                        {
                            component = this.jxPropertyGrid.SelectedObject;
                        }
                        else if (parent.GridItemType == GridItemType.Property)
                        {
                            component = parent.Value;
                        }
                        if (propertyDescriptor.CanResetValue(component))
                        {
                            enabled = true;
                        }
                    }
                }
                catch
                {
                    enabled = true;
                }
            }
            this.contextMenuStripPropertyGrid.Items[0].Enabled = enabled;
            while (this.contextMenuStripPropertyGrid.Items.Count > 1)
            {
                this.contextMenuStripPropertyGrid.Items.RemoveAt(this.contextMenuStripPropertyGrid.Items.Count - 1);
            }
            if (this.ContextMenuOpening != null)
            {
                this.ContextMenuOpening(this.contextMenuStripPropertyGrid);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridItem selectedGridItem = this.jxPropertyGrid.SelectedGridItem;
            if (selectedGridItem != null && selectedGridItem.PropertyDescriptor != null)
            {
                PropertyDescriptor arg_1D_0 = selectedGridItem.PropertyDescriptor;
                this.jxPropertyGrid.ResetSelectedProperty();
                if (this.PropertyValueChanged != null)
                    this.PropertyValueChanged(this.jxPropertyGrid, selectedGridItem, null);
                
            }
        }

        public JxPropertyGrid GetPropertyGrid()
        {
            return this.jxPropertyGrid;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.ProcessCmdKeyEvent != null)
            {
                bool flag = false;
                this.ProcessCmdKeyEvent(this, ref msg, keyData, ref flag);
                if (flag)
                {
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void MainFormNowInitialized()
        {
            this.showPropertyGridNextTimerTick = true;
        }

    }
}

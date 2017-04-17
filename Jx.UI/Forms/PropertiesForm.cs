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

        private List<Form> openedCollectionEditorForms = new List<Form>();
        private bool showPropertyGridNextTimerTick;

        public PropertiesForm()
        {
            InitializeComponent();
        }

        private void PropertiesForm_Load(object sender, EventArgs e)
        {
            
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

        public void SelectObjects(object[] objects, bool invalidate = true)
        {
            jxPropertyGrid.SelectedObjects = objects;
        }

        public void SelectObject(object obj)
        {
            jxPropertyGrid.SelectedObject = obj;
        }

        public void RefreshProperties()
        {

        }

        private void jxPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (PropertyValueChanged != null)
                PropertyValueChanged(s, e.ChangedItem, e.OldValue);
        }


        private static PropertyGrid GetParentPropertyGridOfCollectionEditor(CollectionEditor editor)
        {
            PropertyGrid result;
            try
            {
                PropertyInfo property = editor.GetType().GetProperty("Context", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (property == null)
                {
                    result = null;
                }
                else
                {
                    object value = property.GetValue(editor, null);
                    if (value == null)
                    {
                        result = null;
                    }
                    else
                    {
                        PropertyInfo property2 = value.GetType().GetProperty("OwnerGrid", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (property2 == null)
                        {
                            result = null;
                        }
                        else
                        {
                            result = (property2.GetValue(value, null) as PropertyGrid);
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }
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
    }
}

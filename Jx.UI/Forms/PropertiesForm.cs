using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Jx.UI.Editors
{
    public abstract class BasePropertyEditor : UITypeEditor
    {
        protected IWindowsFormsEditorService IEditorService;
        private Control m_EditControl;
        private bool m_EscapePressed;

        protected abstract Control GetEditControl(ITypeDescriptorContext context, object currentValue);
        protected abstract object EndEdit(Control editControl, ITypeDescriptorContext context, object value);

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            try
            {
                Control c = GetEditControl(context, context.PropertyDescriptor.GetValue(context.Instance));
                if (c is Form)
                    return UITypeEditorEditStyle.Modal;
            }
            catch (Exception) { }
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                if (context != null && provider != null)
                {
                    IEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                    if (IEditorService != null)
                    {
                        string property = context.PropertyDescriptor.Name;

                        m_EditControl = GetEditControl(context, value);

                        if (m_EditControl != null)
                        {
                            m_EditControl.PreviewKeyDown -= M_EditControl_PreviewKeyDown;
                            m_EditControl.PreviewKeyDown += M_EditControl_PreviewKeyDown;

                            m_EscapePressed = false;

                            if (m_EditControl is Form)
                                IEditorService.ShowDialog((Form)m_EditControl);
                            else
                                IEditorService.DropDownControl(m_EditControl);

                            if (m_EscapePressed)
                                return value;

                            return EndEdit(m_EditControl, context, value);
                        }
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                try
                {
                    m_EditControl.PreviewKeyDown -= M_EditControl_PreviewKeyDown;
                }
                catch (Exception) { }
            }

            return base.EditValue(context, provider, value);
        }

        private void M_EditControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            m_EscapePressed = (e.KeyCode == Keys.Escape);
        }

        public IWindowsFormsEditorService EditorService
        {
            get { return IEditorService; }
        }

        public void CloseDropdownWindow()
        {
            if (IEditorService != null)
                IEditorService.CloseDropDown();
        } 
    }
}

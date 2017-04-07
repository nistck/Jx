using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Jx.UI.PGEx
{

    public class JxUIDateTimeEditor : JxUITimeEditor
    {
        public JxUIDateTimeEditor()
            : base("yyyy-MM-dd HH:mm:ss")
        {
        }
    }

    public class JxUITimeOnlyEditor : JxUITimeEditor
    {
        public JxUITimeOnlyEditor()
            : base("HH:mm:ss")
        {

        }
    }

    public abstract class JxUITimeEditor : UITypeEditor
    {
        IWindowsFormsEditorService editorService;
        DateTimePicker picker = new DateTimePicker();

        public JxUITimeEditor(string format)
        {
            this.Format = format; 
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = format; //"hh:mm";
            picker.ShowUpDown = true;
            picker.ValueChanged += new EventHandler(picker_ValueChanged);
        }

        public string Format { get; private set; }


        void picker_ValueChanged(object sender, EventArgs e)
        {
            this.editorService.CloseDropDown();
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            if (provider != null)
            {
                this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (this.editorService != null)
            {
                DateTimePicker tmp = new DateTimePicker();              
                tmp.Value = (DateTime)value;
                tmp.Format = DateTimePickerFormat.Custom;
                tmp.CustomFormat = this.Format;
                tmp.Text = tmp.Value.ToString(this.Format);
                picker = tmp;

                this.editorService.DropDownControl(picker); 
                value = picker.Value;
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

    }
}

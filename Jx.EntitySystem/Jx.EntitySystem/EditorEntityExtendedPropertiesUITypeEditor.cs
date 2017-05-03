using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace Jx.EntitySystem
{
	public class EditorEntityExtendedPropertiesUITypeEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			DesignerInterface.Instance.EntityExtendedPropertiesUITypeEditorEditValue();
			return null;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}

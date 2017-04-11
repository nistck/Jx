using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Jx.EntitySystem
{
	public class EditorEntityUITypeEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Entity ownerEntity = null;
			EntityCustomTypeDescriptor entityCustomTypeDescriptor = context.Instance as EntityCustomTypeDescriptor;
			if (entityCustomTypeDescriptor != null)
			{
				ownerEntity = entityCustomTypeDescriptor.Entity;
			}
			EntityExtendedProperties entityExtendedProperties = context.Instance as EntityExtendedProperties;
			if (entityExtendedProperties != null)
			{
				ownerEntity = entityExtendedProperties.Owner;
			}
			Type propertyType = context.PropertyDescriptor.PropertyType;
			Entity result = (Entity)value;
			if (MapEditorInterface.Instance.EntityUITypeEditorEditValue(ownerEntity, propertyType, ref result))
			{
				return result;
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}

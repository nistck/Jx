using Jx.Ext;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace Jx.EntitySystem
{
	public class EditorEntityTypeUITypeEditor : UITypeEditor
	{
 
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context.PropertyDescriptor.IsReadOnly)
			{
				return value;
			}
			string text = "";
			if (value != null)
			{
				text = ((EntityType)value).FilePath;
			}
			Type propertyType = context.PropertyDescriptor.PropertyType;
			Predicate<string> shouldAddDelegate = delegate(string path)
			{
				EntityType entityType2 = EntityTypes.Instance.FindByFilePath(path);
				return entityType2 != null && propertyType.IsAssignableFrom(entityType2.GetType());
			};
			if (ResourceUtils.DoUITypeEditorEditValueDelegate("EntityType", ref text, shouldAddDelegate, false))
			{
				EntityType entityType;
				if (text != "")
				{
					entityType = EntityTypes.Instance.FindByFilePath(text);
					if (entityType == null)
					{
						Log.Fatal("EditorEntityTypeUITypeEditor: EditValue: Invalid Type");
					}
				}
				else
				{
					entityType = null;
				}
				return entityType;
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			EntityPropertyDescriptor entityPropertyDescriptor = context.PropertyDescriptor as EntityPropertyDescriptor;
			if (entityPropertyDescriptor != null && entityPropertyDescriptor.Name == "Type" && typeof(EntityType).IsAssignableFrom(entityPropertyDescriptor.PropertyType))
			{
				return UITypeEditorEditStyle.None;
			}
			return UITypeEditorEditStyle.Modal;
		}
	}
}

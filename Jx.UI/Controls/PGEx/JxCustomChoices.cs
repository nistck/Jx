using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Jx.UI.Controls.PGEx
{ 
    [Serializable()]
    public class JxCustomChoices : ArrayList
	{				
		public JxCustomChoices(ArrayList array, bool IsSorted)
		{
			this.AddRange(array);
			if (IsSorted)
			{
				this.Sort();
			}
		}
		
		public JxCustomChoices(ArrayList array)
		{
			this.AddRange(array);
		}
		
		public JxCustomChoices(string[] array, bool IsSorted)
		{
			this.AddRange(array);
			if (IsSorted)
			{
				this.Sort();
			}
		}
		
		public JxCustomChoices(string[] array)
		{
			this.AddRange(array);
		}
		
		public JxCustomChoices(int[] array, bool IsSorted)
		{
			this.AddRange(array);
			if (IsSorted)
			{
				this.Sort();
			}
		}
		
		public JxCustomChoices(int[] array)
		{
			this.AddRange(array);
		}
		
		public JxCustomChoices(double[] array, bool IsSorted)
		{
			this.AddRange(array);
			if (IsSorted)
			{
				this.Sort();
			}
		}
		
		public JxCustomChoices(double[] array)
		{
			this.AddRange(array);
		}
		
		public JxCustomChoices(object[] array, bool IsSorted)
		{
			this.AddRange(array);
			if (IsSorted)
			{
				this.Sort();
			}
		}
		
		public JxCustomChoices(object[] array)
		{
			this.AddRange(array);
		}
		
		public ArrayList Items
		{
			get
			{
				return this;
			}
		}
		
		public class CustomChoicesTypeConverter : TypeConverter
		{
			
			private CustomChoicesAttributeList oChoices = null;
			public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
			{
				bool returnValue;
				CustomChoicesAttributeList Choices =  (CustomChoicesAttributeList) context.PropertyDescriptor.Attributes[typeof(CustomChoicesAttributeList)];
				if (oChoices != null)
				{
					return true;
				}
				if (Choices != null)
				{
					oChoices = Choices;
					returnValue = true;
				}
				else
				{
					returnValue = false;
				}
				return returnValue;
			}
			public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context)
			{
				bool returnValue;
				CustomChoicesAttributeList Choices =  (CustomChoicesAttributeList) context.PropertyDescriptor.Attributes[typeof(CustomChoicesAttributeList)];
				if (oChoices != null)
				{
					return true;
				}
				if (Choices != null)
				{
					oChoices = Choices;
					returnValue = true;
				}
				else
				{
					returnValue = false;
				}
				return returnValue;
			}
			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
			{
				CustomChoicesAttributeList Choices =  (JxCustomChoices.CustomChoicesAttributeList) context.PropertyDescriptor.Attributes[typeof(CustomChoicesAttributeList)];
				if (oChoices != null)
				{
					return oChoices.Values;
				}
				return base.GetStandardValues(context);
			}
		}
		
		public class CustomChoicesAttributeList : Attribute
		{
			
			private ArrayList oList = new ArrayList();
			
			public ArrayList Item
			{
				get
				{
					return this.oList;
				}
			}
			
			public TypeConverter.StandardValuesCollection Values
			{
				get
				{
					return new TypeConverter.StandardValuesCollection(this.oList);
				}
			}
			
			public CustomChoicesAttributeList(string[] List)
			{
				oList.AddRange(List);
			}
			
			public CustomChoicesAttributeList(ArrayList List)
			{
				oList.AddRange(List);
			}
			
			public CustomChoicesAttributeList(ListBox.ObjectCollection List)
			{
				oList.AddRange(List);
			}
		}
	}
}

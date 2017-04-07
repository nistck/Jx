using System;
using System.Collections;
using System.Reflection;

namespace Jx.UI.PGEx
{
    public class JxCustomPropertyCollectionSet : CollectionBase
	{	
		public virtual int Add(JxCustomPropertyCollection value)
		{
			return base.List.Add(value);
		}
		
		public virtual int Add()
		{
			return base.List.Add(new JxCustomPropertyCollection());
		}
		
		public virtual JxCustomPropertyCollection this[int index]
		{
			get
			{
				return ((JxCustomPropertyCollection) base.List[index]);
			}
			set
			{
				base.List[index] = value;
			}
		}
		
		public virtual object ToArray()
		{
			ArrayList list = new ArrayList();
			list.AddRange(base.List);
			return list.ToArray(typeof(JxCustomPropertyCollection));
		}
		
	}
}

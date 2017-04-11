using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jx
{
    public interface _IWrappedPropertyDescriptor
    {
        object GetWrappedOwner();
        PropertyInfo GetWrappedProperty();
    }
}

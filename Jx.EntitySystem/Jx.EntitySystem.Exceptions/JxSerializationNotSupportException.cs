using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem.Jx.EntitySystem.Exceptions
{
    public class JxSerializationNotSupportException : JxException
    {
        public JxSerializationNotSupportException(Type type, string externInfo = null)
        {
            this.SerializingType = type;
            this.ExternInfo = externInfo;
        }

        public Type SerializingType { get; private set; }
        public string ExternInfo { get; private set; }

        public override string ToString()
        {
            // Serialization for type \"{0}\" are not supported ({1}).
            if( string.IsNullOrEmpty(ExternInfo) )
            {
                string result_0 = string.Format("Serialization for type \"{0}\" are not supported.", SerializingType);
                return result_0;
            }

            string result_1 = string.Format("Serialization for type \"{0}\" are not supported ({1}).", SerializingType, ExternInfo);
            return result_1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jx.BT
{
    [BTProperty("读属性", BTConstants.GROUP_DECORATOR)]
    public class BTGetProperty : BTDecorator
    {
        private string memberName = null;
        private BTMemberType memberType = BTMemberType.Property;
        private string dataId = null; 
 
        public BTGetProperty()
        {
        }

        public BTGetProperty(string memberName, BTMemberType memberType, string dataId, BTNode child = null)
            : base(child)
        {
            this.memberName = memberName;
            this.memberType = memberType;
            this.dataId = dataId; 
        }

        public string MemberName
        {
            get { return this.memberName; }
            set { this.memberName = value; }
        }

        public BTMemberType MemberType
        {
            get { return memberType; }
            set { this.memberType = value; }
        }

        public string DataId
        {
            get { return dataId; }
            set { this.dataId = value; }
        }

        private bool skipTick = false;
        private FieldInfo _field = null;
        private PropertyInfo _property = null;

        protected override BTResult OnTick(BTContext context)
        {            
            BTResult r = m_Child.Tick_(context);
            if (skipTick)
                return r;  

            Type childType = m_Child.GetType();

            if ( memberType == BTMemberType.Field )
            {
                if(_field == null)
                    _field = childType.GetField(MemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (_field != null)
                {
                    object fv = _field.GetValue(m_Child);
                    if (DataId != null)
                        context.Database?.SetData(DataId, fv);
                }
                else
                    skipTick = true;
            }
            else if( memberType == BTMemberType.Property )
            {
                if( _property == null )
                    _property = childType.GetProperty(MemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (_property != null && _property.CanRead)
                {
                    object pv = _property.GetGetMethod().Invoke(m_Child, null);
                    if (DataId != null)
                        context.Database?.SetData(DataId, pv);
                }
                else
                    skipTick = true; 
            } 

            return r;
        }

        public override string ToString()
        {
            string text = string.Format("{0}, 类型: {1}, 存储: {2}", memberName, memberType, dataId);
            return text;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace Jx.BT
{
    [BTProperty("写属性", BTConstants.GROUP_DECORATOR)]
    public class BTSetProperty : BTDecorator
    {
        public BTSetProperty()
        {

        }

        public BTSetProperty(string memberName, BTMemberType memberType, string dataId, BTNode child = null)
            : base(child)
        {
            this.memberName = memberName;
            this.memberType = memberType;
            this.dataId = dataId;
        }

        private string memberName = null;
        private BTMemberType memberType = BTMemberType.Property;
        private string dataId = null;
 
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

            if (memberType == BTMemberType.Field)
            {
                if (_field == null)
                    _field = childType.GetField(MemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (_field != null)
                {
                    object fv = context.Database?.GetData<object>(DataId);
                    _field.SetValue(m_Child, fv);  
                }
                else
                    skipTick = true;
            }
            else if (memberType == BTMemberType.Property)
            {
                if (_property == null)
                    _property = childType.GetProperty(MemberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (_property != null && _property.CanWrite)
                {
                    object pv = context.Database?.GetData<object>(DataId);
                    _property.GetSetMethod().Invoke(m_Child, new object[] { pv });
                }
                else
                    skipTick = true;
            }

            return r;
        }
    }
}

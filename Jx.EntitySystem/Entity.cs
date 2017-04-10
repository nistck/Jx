using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.EntitySystem
{
    public class Entity : JxObject
    {
 
        [Flags]
        public enum FieldSerializeSerializationTypes
        {
            Map = 1,
            World = 2
        }
        /// <summary>
        /// Specifies that a field will be serialized. This class cannot be inherited.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field)]
        public sealed class FieldSerializeAttribute : Attribute
        {
            private string propertyName;
            private Entity.FieldSerializeSerializationTypes serializationTypes = Entity.FieldSerializeSerializationTypes.Map | Entity.FieldSerializeSerializationTypes.World;
            /// <summary>
            /// Gets the property name.
            /// </summary>
            public string PropertyName
            {
                get
                {
                    return this.propertyName;
                }
            }
            public Entity.FieldSerializeSerializationTypes SupportedSerializationTypes
            {
                get
                {
                    return this.serializationTypes;
                }
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <remarks>
            /// The property name will be taken by Reflection from property. If it will be used obfuscator there can be problems with renamed names of properties later.
            /// </remarks>
            public FieldSerializeAttribute()
            {
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            /// <param name="supportedSerializationTypes">The supported serialization types.</param>
            public FieldSerializeAttribute(string propertyName, Entity.FieldSerializeSerializationTypes supportedSerializationTypes)
            {
                this.propertyName = propertyName;
                this.serializationTypes = supportedSerializationTypes;
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            public FieldSerializeAttribute(string propertyName)
            {
                this.propertyName = propertyName;
            }
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="supportedSerializationTypes">The supported serialization types.</param>
            public FieldSerializeAttribute(Entity.FieldSerializeSerializationTypes supportedSerializationTypes)
            {
                this.serializationTypes = supportedSerializationTypes;
            }
        }
        [AttributeUsage(AttributeTargets.Field)]
        public sealed class TypeFieldAttribute : Attribute
        {
        }
        public enum NetworkDirections
        {
            ToClient,
            ToServer
        }
        [AttributeUsage(AttributeTargets.Method)]
        public sealed class NetworkReceiveAttribute : Attribute
        {
            private Entity.NetworkDirections networkDirection;
            private ushort messageIdentifier;
            public Entity.NetworkDirections Direction
            {
                get
                {
                    return this.networkDirection;
                }
            }
            public ushort MessageIdentifier
            {
                get
                {
                    return this.messageIdentifier;
                }
            }
            public NetworkReceiveAttribute(Entity.NetworkDirections direction, ushort messageIdentifier)
            {
                this.networkDirection = direction;
                this.messageIdentifier = messageIdentifier;
            }
        }
        public class TagInfo
        {
            [Entity.FieldSerializeAttribute("name")]
            private string name = "";
            [Entity.FieldSerializeAttribute("value")]
            private string value = "";
            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                }
            }
            public string Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = value;
                }
            }
            public TagInfo()
            {
            }
            public TagInfo(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
            public override string ToString()
            {
                return string.Format("{0} = {1}", this.name, this.value);
            }
        }
    }
}

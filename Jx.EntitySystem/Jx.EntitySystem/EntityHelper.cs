using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using Jx.FileSystem;
using Jx.Ext;
using Jx.EntitySystem.LogicSystem;

namespace Jx.EntitySystem
{
    internal static class EntityHelper
    {
        public const string NULL_ITEM = "_nullItem";

        private static readonly Dictionary<Type, List<FieldInfo>> typeFieldsSerializationDic = new Dictionary<Type, List<FieldInfo>>();

        /// <summary>
        /// 获得字段上的FieldSerializeAttribute标记
        /// </summary>
        /// <param name="entityOrEntityType">true, Entity; false, EntityType</param>
        /// <param name="fieldInfo">字段</param>
        /// <returns></returns>
        public static string GetFieldSerializeName(bool entityOrEntityType, FieldInfo fieldInfo)
        {
            string result = fieldInfo.Name;
            if (entityOrEntityType)
            {
                Entity.FieldSerializeAttribute attr = fieldInfo.GetCustomAttribute<Entity.FieldSerializeAttribute>(true);
                if (attr != null && !string.IsNullOrEmpty(attr.PropertyName))
                    result = attr.PropertyName;
            }
            else
            {
                EntityType.FieldSerializeAttribute attr = fieldInfo.GetCustomAttribute<EntityType.FieldSerializeAttribute>(true);
                if (attr != null && !string.IsNullOrEmpty(attr.PropertyName))
                    result = attr.PropertyName;
            }
            return result;
        }
        public static string ConvertToString(Type type, object value, string errorString)
        {
            if (typeof(EntityType).IsAssignableFrom(type))
            {
                EntityType entityType = (EntityType)value;
                return entityType.Name;
            }

            if (typeof(Entity).IsAssignableFrom(type))
            {
                Entity entity = (Entity)value;
                return entity.UIN.ToString();
            }

            if (typeof(Type) == type)
                return ((Type)value).FullName;

            if (SimpleTypesUtils.IsSimpleType(type))
                return value.ToString();

            if (errorString != null)
                Log.Fatal("Entity System: Serialization for type \"{0}\" are not supported ({1}).", type.ToString(), errorString);

            return null;
        }

        /// <summary>
        /// 数据转换: 将<paramref name="strValue"/>转化成<paramref name="type"/>，输出<paramref name="outValue"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="strValue"></param>
        /// <param name="errorString"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool ConvertFromString(Type type, string strValue, string errorString, out object outValue)
        {
            outValue = null; 
            try
            {
                #region Simple Type
                if (SimpleTypesUtils.IsSimpleType(type))
                { 
                    if (strValue == "")
                    { 
                        if (type == typeof(string))
                        {
                            outValue = "";
                            return true;
                        }
                        if (errorString != null)
                            Log.Error("Entity System: Serialization error. The invalid value \"{0}\" for type \"{1}\" ({2}).", strValue, type, errorString); 
                        return false;
                    }

                    outValue = SimpleTypesUtils.GetSimpleTypeValue(type, strValue);
                    return outValue != null;
                }
                #endregion

                #region Entity
                if (typeof(Entity).IsAssignableFrom(type))
                {
                    if (strValue == "null" || strValue == "")
                        return true;

                    Entity loadingEntityBySerializedUIN = Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(strValue));
                    bool isEntityExists = (EntitySystemWorld.Instance.IsSingle() || EntitySystemWorld.Instance.IsEditor()) && loadingEntityBySerializedUIN == null;
                    if (isEntityExists)
                    {
                        if (errorString != null)
                            Log.Error("Entity System: Serialization error. The entity with UIN \"{0}\" is not exists ({1}).", strValue, errorString); 
                        return false;
                    }

                    outValue = loadingEntityBySerializedUIN;
                    return true;
                }
                #endregion

                #region EntityType
                if (typeof(EntityType).IsAssignableFrom(type))
                {
                    if (strValue == "null" || strValue == "")
                        return true;

                    EntityType byName = EntityTypes.Instance.GetByName(strValue);
                    if (byName == null)
                    {
                        if (errorString != null)
                            Log.Error("Entity System: Serialization error. The entity type is not defined \"{0}\" ({1}).", strValue, errorString);

                        return false;
                    }
                    outValue = byName;
                    return true;
                }
                #endregion

                #region typeof(Type) == type
                if (typeof(Type) == type)
                {
                    if (strValue == "null" || strValue == "")
                        return true;

                    if (strValue.Contains("Engine.UISystem.E"))
                    {
                        string[] rControl = new string[]
                        {
                                            "VideoBox",
                                            "Button",
                                            "CheckBox",
                                            "ComboBox",
                                            "Control",
                                            "EditBox",
                                            "ListBox",
                                            "ScrollBar",
                                            "TabControl",
                                            "TextBox"
                        };
                        for (int i = 0; i < rControl.Length; i++)
                        {
                            string str = rControl[i];
                            bool flag17 = strValue == "Engine.UISystem.E" + str;
                            if (flag17)
                            {
                                strValue = "Engine.UISystem." + str;
                                break;
                            }
                        }
                    }
                    Type typeControl = null;
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int j = 0; j < assemblies.Length; j++)
                    {
                        Assembly assembly = assemblies[j];
                        typeControl = assembly.GetType(strValue);
                        if (typeControl != null)
                            break;
                    }
                    if (typeControl == null)
                    {
                        if (errorString != null)
                            Log.Error("Entity System: Serialization error. The entity type is not found \"{0}\" ({1}).", strValue, errorString);

                        return false;
                    }

                    outValue = typeControl;
                    return true;
                }
                #endregion

                if (errorString != null)
                    Log.Fatal("Entity System: Serialization for type \"{0}\" are not supported ({1}).", type.ToString(), errorString);
                return false;
            }
            catch (FormatException ex)
            { 
                if (errorString != null)
                {
                    Log.Error("Entity System: Serialization error: \"{0}\" ({1}).", ex.Message, errorString);
                }
                return false;
            } 
        }
         
        /// <summary>
        /// 设置目标<paramref name="targetObj"/>的字段<paramref name="fieldInfo"/>的值为<paramref name="list"/>
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="list"></param>
        private static void SetArrayFieldValue(object targetObj, FieldInfo fieldInfo, List<object> list)
        {
            if (fieldInfo == null)
                return;
            if (fieldInfo.FieldType.IsArray)
            {
                ConstructorInfo constructor = fieldInfo.FieldType.GetConstructor(new Type[] { typeof(int) });
                object r = constructor.Invoke(new object[] { list.Count });
                fieldInfo.SetValue(targetObj, r);

                MethodInfo method = fieldInfo.FieldType.GetMethod("SetValue", new Type[]
                {
                    typeof(object),
                    typeof(int)
                });

                for (int i = 0; i < list.Count; i++)
                    method.Invoke(r, new object[] { list[i], i });
            }
            else
            {
                object vList = fieldInfo.GetValue(targetObj);
                if (vList == null)
                {
                    vList = fieldInfo.FieldType.InvokeMember("", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                    fieldInfo.SetValue(targetObj, vList);
                }
                else
                {
                    MethodInfo methodClear = fieldInfo.FieldType.GetMethod("Clear");
                    methodClear.Invoke(vList, new object[0]);
                }
                MethodInfo methodAdd = fieldInfo.FieldType.GetMethod("Add");
                object[] args = new object[1];
                foreach (object current in list)
                {
                    args[0] = current;
                    methodAdd.Invoke(vList, args);
                }
            }
        }

        /// <summary>
        /// 获得类型<paramref name="type"/>可序列化的字段
        /// </summary>
        /// <param name="entitySerialize">true, Entity; false, EntityType</param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<FieldInfo> GetTypeSerializableFields(bool entitySerialize, Type type)
        {
            List<FieldInfo> list = new List<FieldInfo>();
            if (type == null)
                return list;

            if(typeFieldsSerializationDic.ContainsKey(type))
            {
                list.AddRange(typeFieldsSerializationDic[type]);
                return list;
            }

            Type typeCurrent = type;
            while (typeCurrent != null)
            {
                var q = typeCurrent.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(_f => IsFieldSerializable(_f, entitySerialize))
                    ;
                list.AddRange(q); 
                typeCurrent = typeCurrent.BaseType;
            }

            typeFieldsSerializationDic[type] = list;
            return list.ToList();
        }

        private static bool LoadArrayFieldValue(bool entitySerialize, object targetObj, FieldInfo fieldInfo, TextBlock textBlock, string text)
        {
            bool isArray = fieldInfo.FieldType.IsArray;
            Type typeElement;
            if (isArray)
                typeElement = fieldInfo.FieldType.GetElementType();
            else
                typeElement = fieldInfo.FieldType.GetGenericArguments()[0];

            string name = GetFieldSerializeName(entitySerialize, fieldInfo);
            TextBlock blockChild = textBlock.FindChild(name);

            #region typeElement == typeof(string) 
            if (typeElement == typeof(string))
            { 
                if (blockChild != null)
                {
                    List<object> list = new List<object>(blockChild.Children.Count);
                    foreach (TextBlock current in blockChild.Children)
                    {
                        string currentValue = current.GetAttribute("value");
                        if( string.IsNullOrEmpty(currentValue) && string.Compare(current.GetAttribute(NULL_ITEM), "true", true) == 0)
                            currentValue = null;
                        
                        list.Add(currentValue);
                    }
                    SetArrayFieldValue(targetObj, fieldInfo, list);
                }
                return true;
            }
            #endregion

            #region Simple Type
            bool typeIsEntityType = typeof(EntityType).IsAssignableFrom(typeElement);
            bool typeIsEntity = typeof(Entity).IsAssignableFrom(typeElement);
            if ((SimpleTypesUtils.IsSimpleType(typeElement) | typeIsEntityType | typeIsEntity) || typeof(Type) == typeElement)
            {
                if (!entitySerialize & typeIsEntity)
                {
                    Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", text);
                    return false;
                }

                if (textBlock.IsAttributeExist(name))
                {
                    string nameAttr = textBlock.GetAttribute(name);
                    string[] nameArr;
                    if (nameAttr.Length != 0)
                    {
                        char c = ';';
                        bool flag11 = typeElement.IsPrimitive | typeIsEntity;
                        if (flag11)
                            c = ' ';
                        nameArr = nameAttr.Split(new char[] { c });
                    }
                    else
                        nameArr = new string[0];

                    List<object> values = new List<object>(nameArr.Length);
                    for (int i = 0; i < nameArr.Length; i++)
                    {
                        string strValue = nameArr[i];
                        object item;
                        bool loadFailure = !ConvertFromString(typeElement, strValue, text, out item);
                        if (loadFailure)
                            return false;

                        values.Add(item);
                    }
                    SetArrayFieldValue(targetObj, fieldInfo, values);
                }
                return true;
            }
            #endregion

            if (blockChild != null)
            {
                List<FieldInfo> typeElementFields = GetTypeSerializableFields(entitySerialize, typeElement);
                List<object> children = new List<object>(blockChild.Children.Count);
                foreach (TextBlock textChild in blockChild.Children)
                {
                    object elementObject;
                    if (string.Compare(textChild.GetAttribute(NULL_ITEM), "true", true) != 0)
                    {
                        elementObject = typeElement.InvokeMember("", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                        if (elementObject == null)
                        {
                            Log.Fatal("EntitySystem: Serialization: InvokeMember failed for \"{0}\"", typeElement);
                            return false;
                        }

                        using (List<FieldInfo>.Enumerator itElementField = typeElementFields.GetEnumerator())
                        {
                            while (itElementField.MoveNext())
                            {
                                FieldInfo currentField = itElementField.Current;
                                bool loadOk = LoadFieldValue(entitySerialize, elementObject, currentField, textChild, text);
                                if (!loadOk)
                                    return false;
                            }
                            children.Add(elementObject);
                            continue;
                        }
                    }
                    children.Add(null); 
                }
                SetArrayFieldValue(targetObj, fieldInfo, children);
            }
            return true;
        }

        public static bool LoadFieldValue(bool entitySerialize, object owner, FieldInfo field, TextBlock block, string errorString)
        {
            string fieldName = GetFieldSerializeName(entitySerialize, field);
            errorString += string.Format(", property: \"{0}\"", fieldName);
            bool isList = field.FieldType.IsGenericType && field.FieldType.Name == typeof(List<>).Name;
            bool isArray = field.FieldType.IsArray;  

            if (isList || isArray)
            { 
                if (isArray && field.FieldType.GetArrayRank() != 1)
                {
                    Log.Fatal("Entity System: Serialization of arrays are supported only for one dimensions arrays ({0}).", errorString);
                    return false;
                }
                bool loadResult = LoadArrayFieldValue(entitySerialize, owner, field, block, errorString);
                return loadResult;
            }

            bool isEntityType = typeof(EntityType).IsAssignableFrom(field.FieldType);
            bool isEntity = typeof(Entity).IsAssignableFrom(field.FieldType);
            bool typeSupport = (SimpleTypesUtils.IsSimpleType(field.FieldType) | isEntityType | isEntity) || typeof(Type) == field.FieldType;
            if (typeSupport)
            {
                if (!entitySerialize & isEntity)
                {
                    Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", errorString);
                    return false;
                }
                bool fieldExists = block.IsAttributeExist(fieldName);
                if (fieldExists)
                {
                    string fieldValue = block.GetAttribute(fieldName);
                    object value;
                    bool convertResult = ConvertFromString(field.FieldType, fieldValue, errorString, out value);
                    if (!convertResult)
                        return false;
                    field.SetValue(owner, value);
                }
                return true;
            }

            TextBlock textBlock = block.FindChild(fieldName);
            if (textBlock == null)
                return true;

            object obj = field.GetValue(owner);
            if (obj == null)
            {
                obj = field.FieldType.InvokeMember("", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                field.SetValue(owner, obj);
            }
            Type type = field.FieldType;
            if (type == typeof(LogicEntityObject))
                type = field.GetValue(owner).GetType();

            while (type != null)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < fields.Length; j++)
                {
                    FieldInfo fieldInfo = fields[j];
                    if (!IsFieldSerializable(fieldInfo, entitySerialize))
                        continue;  

                    bool loadFailure = !LoadFieldValue(entitySerialize, obj, fieldInfo, textBlock, errorString);
                    if (loadFailure)
                        return false;
                }
                type = type.BaseType;
            }

            if (obj.GetType().IsValueType)
                field.SetValue(owner, obj);

            return true;
        } 

        public static List<object> GetFieldValue(object obj, FieldInfo fieldInfo)
        {
            List<object> items = new List<object>();
            if (obj == null || fieldInfo == null)
                return items;

            object value = fieldInfo.GetValue(obj);
            bool isArray = fieldInfo.FieldType.IsArray;
            bool isList = fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.Name == typeof(List<>).Name;

            if (isArray)
            {
                PropertyInfo property = fieldInfo.FieldType.GetProperty("Length");
                MethodInfo method = fieldInfo.FieldType.GetMethod("GetValue", new Type[]
                {
                    typeof(int)
                });
                int num = (int)property.GetValue(value, null);
                object[] array2 = new object[1];
                for (int i = 0; i < num; i++)
                {
                    array2[0] = i;
                    object o = method.Invoke(value, array2);
                    items.Add(o);
                }
            }
            else if( isList )
            {
                PropertyInfo property2 = fieldInfo.FieldType.GetProperty("Count");
                PropertyInfo property3 = fieldInfo.FieldType.GetProperty("Item");
                int num2 = (int)property2.GetValue(value, null);
                object[] array3 = new object[1];
                for (int j = 0; j < num2; j++)
                {
                    array3[0] = j;
                    object o1 = property3.GetValue(value, array3);
                    items.Add(o1);
                }
            }
            else
            {
                items.Add(value);
            }
            return items;
        }

        private static bool SaveCollectionFieldValue(
            bool entitySerialize, object obj, FieldInfo fieldInfo, TextBlock textBlock, 
            object defaultObj, string errorMessage)
        {
            object value = fieldInfo.GetValue(obj);
            bool isArray = fieldInfo.FieldType.IsArray;

            List<object> items = new List<object>();
            #region 获得Collection中的元素 -> items
            if (isArray)
            {
                PropertyInfo propertyLength = fieldInfo.FieldType.GetProperty("Length");
                MethodInfo methodGetValue = fieldInfo.FieldType.GetMethod("GetValue", new Type[]
                {
                    typeof(int)
                });
                int arrayLength = (int)propertyLength.GetValue(value, null); 
                for (int i = 0; i < arrayLength; i++)
                {
                    object o = methodGetValue.Invoke(value, new object[] { i });
                    items.Add(o);
                }
            }
            else
            {
                PropertyInfo propertyCount = fieldInfo.FieldType.GetProperty("Count");
                PropertyInfo propertyItem = fieldInfo.FieldType.GetProperty("Item");
                int listCount = (int)propertyCount.GetValue(value, null);
                for (int j = 0; j < listCount; j++)
                {
                    object o1 = propertyItem.GetValue(value, new object[] { j });
                    items.Add(o1);
                }
            }
            #endregion

            bool entityTypeDefaultSerialize = !entitySerialize && obj is EntityType && defaultObj != null;

            #region EntityType 如果是缺省值，就不保存
            if (entityTypeDefaultSerialize)
            {  
                int itemsCount = 0;
                if (isArray)
                {
                    PropertyInfo propertyLength = fieldInfo.FieldType.GetProperty("Length");
                    itemsCount = (int)propertyLength.GetValue(defaultObj, null); 
                }
                else
                {
                    PropertyInfo propertyCount = fieldInfo.FieldType.GetProperty("Count");
                    itemsCount = (int)propertyCount.GetValue(defaultObj, null); 
                } 
                if (items.Count == 0 & itemsCount == 0)
                    return true;
            }
            #endregion

            Type typeElement = typeof(object);
            if (isArray)
                typeElement = fieldInfo.FieldType.GetElementType();
            else
                typeElement = fieldInfo.FieldType.GetGenericArguments()[0];
            
            string fieldSerializeName = GetFieldSerializeName(entitySerialize, fieldInfo);   
            if (typeElement == typeof(string))
            {
                TextBlock stringsBlock = textBlock.AddChild(fieldSerializeName); 
                for (int k = 0; k < items.Count; k++)
                {
                    object item = items[k];
                    TextBlock textBlock3 = stringsBlock.AddChild("item"); 
                    if (item != null)
                        textBlock3.SetAttribute("value", (string)item);
                    else
                        textBlock3.SetAttribute(NULL_ITEM, true.ToString());
                }
                return true;
            }

            bool isEntityType = typeof(EntityType).IsAssignableFrom(typeElement);
            bool isEntity = typeof(Entity).IsAssignableFrom(typeElement);
            bool simpleTypeSerializing = (SimpleTypesUtils.IsSimpleType(typeElement) | isEntityType | isEntity) || typeof(Type) == typeElement;
            if (simpleTypeSerializing)
            {
                if (!entitySerialize & isEntity)
                {
                    Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", errorMessage);
                    return false;
                }
                char itemSeperator = typeElement.IsPrimitive | isEntity? ' ' :  ';';
                
                StringBuilder stringBuilder = new StringBuilder();
                for (int l = 0; l < items.Count; l++)
                {
                    object item = items[l]; 
                    if (isEntity && item != null)
                    {
                        Entity entity = (Entity)item; 
                        if (!entity.AllowSaveHerit())
                        {
                            Log.Fatal("Entity System: Serialization error. The reference to entity which does not allow serialization ({0}). Field to serialize: \"{1}\".", entity.ToString(), errorMessage);
                            return false;
                        }
                    }

                    if (l != 0)
                        stringBuilder.Append(itemSeperator);
                     
                    if (item != null)
                        stringBuilder.Append(ConvertToString(typeElement, item, errorMessage + ": " + fieldSerializeName));
                    else
                        stringBuilder.Append("null");
                }
                textBlock.SetAttribute(fieldSerializeName, stringBuilder.ToString());
                return true;
            }

            TextBlock textBlockField = textBlock.AddChild(fieldSerializeName);
            List<FieldInfo> list = GetTypeSerializableFields(entitySerialize, typeElement);
            for(int m = 0; m < items.Count; m ++) 
            {
                object item = items[m];

                TextBlock textBlockItem = textBlockField.AddChild("item");
                if ( item == null )
                {
                    textBlockItem.SetAttribute(NULL_ITEM, true.ToString());
                    continue;
                }
                for(int k = 0; k < list.Count; k ++)
                {
                    FieldInfo elementField = list[k];
                    bool _saveResult = SaveFieldValue(entitySerialize, item, elementField, textBlockItem, errorMessage);
                    if (!_saveResult)
                        return false;
                }
            }
            return true;
        }

        public static bool IsEntityFieldSerializable(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                return false;

            Entity.FieldSerializeAttribute attr = fieldInfo.GetCustomAttribute<Entity.FieldSerializeAttribute>(true);
            if (attr == null)
                return false; 

            bool typeSupported = EntitySystemWorld.Instance.IsEntityFieldSerializable(attr.SupportedSerializationTypes);
            return typeSupported; 
        }

        public static bool IsFieldSerializable(FieldInfo fieldInfo, bool entitySerialize)
        {
            if (entitySerialize)
            {
                if (!IsEntityFieldSerializable(fieldInfo))
                    return false;
            }
            else
            {
                EntityType.FieldSerializeAttribute attrFound = fieldInfo.GetCustomAttribute<EntityType.FieldSerializeAttribute>(true);
                if (attrFound == null)
                    return false; 
            }
            return true;
        }

        public static bool SaveFieldValue(bool entitySerialize, object owner, FieldInfo field, TextBlock block, string errorString)
        {
            if (field == null)
                return false;

            object defaultValue = field.GetDefaultValue();
            bool result = SaveFieldValue(entitySerialize, owner, field, block, defaultValue, errorString);
            return result; 
        }

        public static bool SaveFieldValue(bool entitySerialize, object owner, FieldInfo field, TextBlock block, object defaultValue, string errorString)
        {
            if (field == null)
                return false; 

            object value = field.GetValue(owner);
            if (value == null)
                return true; 
 
            string serializeName = GetFieldSerializeName(entitySerialize, field);
            errorString += string.Format(", property: \"{0}\"", serializeName);

            bool IsList = field.FieldType.IsGenericType && field.FieldType.Name == typeof(List<>).Name;
            bool isArray = field.FieldType.IsArray;
            bool FieldIsCollection = IsList | isArray;
            if (FieldIsCollection)
            { 
                if (isArray && field.FieldType.GetArrayRank() != 1)
                {
                    Log.Fatal("Entity System: Serialization of arrays are supported only for one dimensions arrays ({0}).", errorString);
                    return false;
                }
                bool saveResult = SaveCollectionFieldValue(entitySerialize, owner, field, block, defaultValue, errorString);
                return saveResult;
            }

            bool isEntityType = typeof(EntityType).IsAssignableFrom(field.FieldType);
            bool isEntity = typeof(Entity).IsAssignableFrom(field.FieldType);
            bool simpleSerialization = (SimpleTypesUtils.IsSimpleType(field.FieldType) | isEntityType | isEntity) || typeof(Type) == field.FieldType;
            if (simpleSerialization)
            {
                if (!entitySerialize & isEntity)
                {
                    Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", errorString);
                    return false;
                }

                if (isEntity)
                {
                    Entity entity = (Entity)value;
                    if (!entity.AllowSaveHerit())
                    {
                        Log.Fatal("Entity System: Serialization error. The reference to entity which does not allow serialization ({0}). Field to serialize: \"{1}\".", entity.ToString(), errorString);
                        return false;
                    }
                }
                string saveValueString = ConvertToString(field.FieldType, value, errorString);
                string defaultSaveValueString = null;
                if (defaultValue != null)
                {
                    bool isEnum = field.FieldType.IsEnum;
                    if (isEnum)
                        defaultSaveValueString = Enum.GetName(field.FieldType, defaultValue);
                    else
                        defaultSaveValueString = defaultValue.ToString();
                }

                bool shouldSave = (defaultSaveValueString != null && defaultSaveValueString != saveValueString) || (defaultValue == null && saveValueString != "");
                if (shouldSave)
                    block.SetAttribute(serializeName, saveValueString);
                return true;
            } 

            TextBlock blockSerialize = block.AddChild(serializeName);
            Type type = field.FieldType;
            if (field.FieldType == typeof(LogicEntityObject))
                type = field.GetValue(owner).GetType();

            while (type != null)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for(int m = 0; m < fields.Length; m ++)
                {
                    FieldInfo fieldInfo = fields[m];
                    if (!IsFieldSerializable(fieldInfo, entitySerialize))
                        continue;                   
                    bool saveResult = SaveFieldValue(entitySerialize, value, fieldInfo, blockSerialize, errorString);
                    if (!saveResult)
                        return false;
                } 
                type = type.BaseType;
            }
            return true;
        }
    }
}

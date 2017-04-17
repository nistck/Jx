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
        private static string GetFieldSerializeName(bool entityOrEntityType, FieldInfo fieldInfo)
        {
            string result = fieldInfo.Name;
            if (entityOrEntityType)
            {
                Entity.FieldSerializeAttribute[] r = (Entity.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(Entity.FieldSerializeAttribute), true);
                if (r.Length != 0 && r[0].PropertyName != null)
                    result = r[0].PropertyName;
            }
            else
            {
                EntityType.FieldSerializeAttribute[] r = (EntityType.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(EntityType.FieldSerializeAttribute), true);
                if (r.Length != 0 && r[0].PropertyName != null)
                    result = r[0].PropertyName;
            }
            return result;
        }

        public static bool GetLoadStringValue(Type type, string strValue, string errorString, out object outValue)
        {
            outValue = null;
            bool result;
            try
            { 
                if (SimpleTypesUtils.IsSimpleType(type))
                { 
                    if (strValue == "")
                    { 
                        if (type == typeof(string))
                        {
                            outValue = "";
                            result = true;
                        }
                        else
                        { 
                            if (errorString != null)
                            {
                                Log.Error("Entity System: Serialization error. The invalid value \"{0}\" for type \"{1}\" ({2}).", strValue, type, errorString);
                            }
                            result = false;
                        }
                    }
                    else
                    {
                        outValue = SimpleTypesUtils.GetSimpleTypeValue(type, strValue); 
                        result = outValue != null;
                    }
                }
                else
                {
                    if (typeof(Entity).IsAssignableFrom(type))
                    {
                        if (strValue == "null" || strValue == "")
                        {
                            result = true;
                        }
                        else
                        {
                            Entity loadingEntityBySerializedUIN = Entities.Instance.GetLoadingEntityBySerializedUIN(uint.Parse(strValue));
                            bool flag8 = (EntitySystemWorld.Instance.IsSingle() || EntitySystemWorld.Instance.IsEditor()) && loadingEntityBySerializedUIN == null;
                            if (flag8)
                            { 
                                if (errorString != null)
                                {
                                    Log.Error("Entity System: Serialization error. The entity with UIN \"{0}\" is not exists ({1}).", strValue, errorString);
                                }
                                result = false;
                            }
                            else
                            {
                                outValue = loadingEntityBySerializedUIN;
                                result = true;
                            }
                        }
                    }
                    else
                    { 
                        if (typeof(EntityType).IsAssignableFrom(type))
                        { 
                            if (strValue == "null" || strValue == "")
                            {
                                result = true;
                            }
                            else
                            {
                                EntityType byName = EntityTypes.Instance.GetByName(strValue); 
                                if (byName == null)
                                { 
                                    if (errorString != null)
                                    {
                                        Log.Error("Entity System: Serialization error. The entity type is not defined \"{0}\" ({1}).", strValue, errorString);                                        
                                    }
                                    result = false;
                                }
                                else
                                {
                                    outValue = byName;
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            bool flag14 = typeof(Type) == type;
                            if (flag14)
                            {
                                bool flag15 = strValue == "null" || strValue == "";
                                if (flag15)
                                {
                                    result = true;
                                }
                                else
                                {
                                    bool flag16 = strValue.Contains("Engine.UISystem.E");
                                    if (flag16)
                                    {
                                        string[] array = new string[]
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
                                        string[] array2 = array;
                                        for (int i = 0; i < array2.Length; i++)
                                        {
                                            string str = array2[i];
                                            bool flag17 = strValue == "Engine.UISystem.E" + str;
                                            if (flag17)
                                            {
                                                strValue = "Engine.UISystem." + str;
                                                break;
                                            }
                                        }
                                    }
                                    Type type2 = null;
                                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                                    Assembly[] array3 = assemblies;
                                    for (int j = 0; j < array3.Length; j++)
                                    {
                                        Assembly assembly = array3[j];
                                        type2 = assembly.GetType(strValue);
                                        bool flag18 = type2 != null;
                                        if (flag18)
                                        {
                                            break;
                                        }
                                    }
                                    bool flag19 = type2 == null;
                                    if (flag19)
                                    {
                                        bool flag20 = errorString != null;
                                        if (flag20)
                                        {
                                            Log.Error("Entity System: Serialization error. The entity type is not found \"{0}\" ({1}).", strValue, errorString);
                                        }
                                        result = false;
                                    }
                                    else
                                    {
                                        outValue = type2;
                                        result = true;
                                    }
                                }
                            }
                            else
                            {
                                bool flag21 = errorString != null;
                                if (flag21)
                                {
                                    Log.Fatal("Entity System: Serialization for type \"{0}\" are not supported ({1}).", type.ToString(), errorString);
                                    return false;
                                }
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                bool flag22 = errorString != null;
                if (flag22)
                {
                    Log.Error("Entity System: Serialization error: \"{0}\" ({1}).", ex.Message, errorString);
                }
                result = false;
            }
            return result;
        }

        private static void A(object obj, FieldInfo fieldInfo, List<object> list)
        {
            bool isArray = fieldInfo.FieldType.IsArray;
            if (isArray)
            {
                ConstructorInfo constructor = fieldInfo.FieldType.GetConstructor(new Type[]
                {
                    typeof(int)
                });
                object obj2 = constructor.Invoke(new object[]
                {
                    list.Count
                });
                fieldInfo.SetValue(obj, obj2);
                MethodInfo method = fieldInfo.FieldType.GetMethod("SetValue", new Type[]
                {
                    typeof(object),
                    typeof(int)
                });
                object[] array = new object[2];
                for (int i = 0; i < list.Count; i++)
                {
                    array[0] = list[i];
                    array[1] = i;
                    method.Invoke(obj2, array);
                }
            }
            else
            {
                object obj3 = fieldInfo.GetValue(obj);
                bool flag = obj3 == null;
                if (flag)
                {
                    obj3 = fieldInfo.FieldType.InvokeMember("", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                    fieldInfo.SetValue(obj, obj3);
                }
                else
                {
                    MethodInfo method2 = fieldInfo.FieldType.GetMethod("Clear");
                    method2.Invoke(obj3, new object[0]);
                }
                MethodInfo method3 = fieldInfo.FieldType.GetMethod("Add");
                object[] array2 = new object[1];
                foreach (object current in list)
                {
                    array2[0] = current;
                    method3.Invoke(obj3, array2);
                }
            }
        }
        private static List<FieldInfo> A(bool flag, Type type)
        {
            List<FieldInfo> list = new List<FieldInfo>(16);
            Type type2 = type;
            while (type2 != null)
            {
                FieldInfo[] fields = type2.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] array = fields;
                int i = 0;
                while (i < array.Length)
                {
                    FieldInfo fieldInfo = array[i];
                    if (flag)
                    {
                        Entity.FieldSerializeAttribute[] array2 = (Entity.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(Entity.FieldSerializeAttribute), true);
                        bool flag2 = array2.Length != 0;
                        if (flag2)
                        {
                            bool flag3 = EntitySystemWorld.Instance.isEntitySerializable(array2[0].SupportedSerializationTypes);
                            if (flag3)
                            {
                                goto IL_A1;
                            }
                        }
                    }
                    else
                    {
                        bool flag4 = fieldInfo.GetCustomAttributes(typeof(EntityType.FieldSerializeAttribute), true).Length != 0;
                        if (flag4)
                        {
                            goto IL_A1;
                        }
                    }
                    IL_98:
                    i++;
                    continue;
                    IL_A1:
                    list.Add(fieldInfo);
                    goto IL_98;
                }
                type2 = type2.BaseType;
            }
            return list;
        }
        private static bool A(bool flag, object obj, FieldInfo fieldInfo, TextBlock textBlock, string text)
        {
            bool isArray = fieldInfo.FieldType.IsArray;
            Type type;
            if (isArray)
            {
                type = fieldInfo.FieldType.GetElementType();
            }
            else
            {
                type = fieldInfo.FieldType.GetGenericArguments()[0];
            }
            string name = EntityHelper.GetFieldSerializeName(flag, fieldInfo);
            bool flag2 = type == typeof(string);
            bool result;
            if (flag2)
            {
                TextBlock textBlock2 = textBlock.FindChild(name);
                bool flag3 = textBlock2 != null;
                if (flag3)
                {
                    List<object> list = new List<object>(textBlock2.Children.Count);
                    foreach (TextBlock current in textBlock2.Children)
                    {
                        string text2 = current.GetAttribute("value");
                        bool flag4 = string.IsNullOrEmpty(text2) && current.GetAttribute("_nullItem") == true.ToString();
                        if (flag4)
                        {
                            text2 = null;
                        }
                        list.Add(text2);
                    }
                    EntityHelper.A(obj, fieldInfo, list);
                }
                result = true;
            }
            else
            {
                bool flag5 = typeof(EntityType).IsAssignableFrom(type);
                bool flag6 = typeof(Entity).IsAssignableFrom(type);
                bool flag7 = (SimpleTypesUtils.IsSimpleType(type) | flag5 | flag6) || typeof(Type) == type;
                if (flag7)
                {
                    bool flag8 = !flag & flag6;
                    if (flag8)
                    {
                        Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", text);
                        return false;
                    }
                    bool flag9 = textBlock.IsAttributeExist(name);
                    if (flag9)
                    {
                        string attribute = textBlock.GetAttribute(name);
                        bool flag10 = attribute.Length != 0;
                        string[] array;
                        if (flag10)
                        {
                            char c = ';';
                            bool flag11 = type.IsPrimitive | flag6;
                            if (flag11)
                            {
                                c = ' ';
                            }
                            array = attribute.Split(new char[]
                            {
                                c
                            });
                        }
                        else
                        {
                            array = new string[0];
                        }
                        List<object> list2 = new List<object>(array.Length);
                        string[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string strValue = array2[i];
                            object item;
                            bool flag12 = !EntityHelper.GetLoadStringValue(type, strValue, text, out item);
                            if (flag12)
                            {
                                bool flag13 = false;
                                result = flag13;
                                return result;
                            }
                            list2.Add(item);
                        }
                        EntityHelper.A(obj, fieldInfo, list2);
                    }
                    result = true;
                }
                else
                {
                    TextBlock textBlock3 = textBlock.FindChild(name);
                    bool flag14 = textBlock3 != null;
                    if (flag14)
                    {
                        List<FieldInfo> list3 = EntityHelper.A(flag, type);
                        List<object> list4 = new List<object>(textBlock3.Children.Count);
                        foreach (TextBlock current2 in textBlock3.Children)
                        {
                            bool flag15 = current2.GetAttribute("_nullItem") != true.ToString();
                            object obj2;
                            if (flag15)
                            {
                                obj2 = type.InvokeMember("", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                                bool flag16 = obj2 == null;
                                if (flag16)
                                {
                                    Log.Fatal("EntitySystem: Serialization: InvokeMember failed for \"{0}\"", type);
                                    return false;
                                }
                                using (List<FieldInfo>.Enumerator enumerator3 = list3.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        FieldInfo current3 = enumerator3.Current;
                                        bool flag17 = !EntityHelper.LoadFieldValue(flag, obj2, current3, current2, text);
                                        if (flag17)
                                        {
                                            bool flag18 = false;
                                            result = flag18;
                                            return result;
                                        }
                                    }
                                    goto IL_347;
                                }
                                goto IL_345;
                            }
                            goto IL_345;
                            IL_347:
                            list4.Add(obj2);
                            continue;
                            IL_345:
                            obj2 = null;
                            goto IL_347;
                        }
                        EntityHelper.A(obj, fieldInfo, list4);
                    }
                    result = true;
                }
            }
            return result;
        }

        public static bool LoadFieldValue(bool entitySerialize, object owner, FieldInfo field, TextBlock block, string errorString)
        {
            string text = EntityHelper.GetFieldSerializeName(entitySerialize, field);
            errorString += string.Format(", property: \"{0}\"", text);
            bool flag = field.FieldType.IsGenericType && field.FieldType.Name == typeof(List<>).Name;
            bool isArray = field.FieldType.IsArray;
            bool flag2 = flag | isArray;
            bool result;
            if (flag2)
            {
                bool flag3 = isArray && field.FieldType.GetArrayRank() != 1;
                if (flag3)
                {
                    Log.Fatal("Entity System: Serialization of arrays are supported only for one dimensions arrays ({0}).", errorString);
                    return false;
                }
                result = EntityHelper.A(entitySerialize, owner, field, block, errorString);
            }
            else
            {
                bool flag4 = typeof(EntityType).IsAssignableFrom(field.FieldType);
                bool flag5 = typeof(Entity).IsAssignableFrom(field.FieldType);
                bool flag6 = (SimpleTypesUtils.IsSimpleType(field.FieldType) | flag4 | flag5) || typeof(Type) == field.FieldType;
                if (flag6)
                {
                    bool flag7 = !entitySerialize & flag5;
                    if (flag7)
                    {
                        Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", errorString);
                        return false;
                    }
                    bool flag8 = block.IsAttributeExist(text);
                    if (flag8)
                    {
                        string attribute = block.GetAttribute(text);
                        object value;
                        bool flag9 = !EntityHelper.GetLoadStringValue(field.FieldType, attribute, errorString, out value);
                        if (flag9)
                        {
                            result = false;
                            return result;
                        }
                        field.SetValue(owner, value);
                    }
                    result = true;
                }
                else
                {
                    TextBlock textBlock = block.FindChild(text);
                    bool flag10 = textBlock != null;
                    if (flag10)
                    {
                        object obj = field.GetValue(owner);
                        bool flag11 = obj == null;
                        if (flag11)
                        {
                            obj = field.FieldType.InvokeMember("", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                            field.SetValue(owner, obj);
                        }
                        Type type = field.FieldType;
                        bool flag12 = field.FieldType == typeof(LogicEntityObject);
                        if (flag12)
                        {
                            type = field.GetValue(owner).GetType();
                        }
                        while (type != null)
                        {
                            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            FieldInfo[] array = fields;
                            int i = 0;
                            while (i < array.Length)
                            {
                                FieldInfo fieldInfo = array[i];
                                if (entitySerialize)
                                {
                                    Entity.FieldSerializeAttribute[] array2 = (Entity.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(Entity.FieldSerializeAttribute), true);
                                    bool flag13 = array2.Length != 0;
                                    if (flag13)
                                    {
                                        bool flag14 = EntitySystemWorld.Instance.isEntitySerializable(array2[0].SupportedSerializationTypes);
                                        if (flag14)
                                        {
                                            goto IL_288;
                                        }
                                    }
                                }
                                else
                                {
                                    bool flag15 = fieldInfo.GetCustomAttributes(typeof(EntityType.FieldSerializeAttribute), true).Length != 0;
                                    if (flag15)
                                    {
                                        goto IL_288;
                                    }
                                }
                                IL_27F:
                                i++;
                                continue;
                                IL_288:
                                bool flag16 = !EntityHelper.LoadFieldValue(entitySerialize, obj, fieldInfo, textBlock, errorString);
                                if (flag16)
                                {
                                    result = false;
                                    return result;
                                }
                                goto IL_27F;
                            }
                            type = type.BaseType;
                        }
                        bool isValueType = obj.GetType().IsValueType;
                        if (isValueType)
                        {
                            field.SetValue(owner, obj);
                        }
                    }
                    result = true;
                }
            }
            return result;
        }
        public static string GetSaveValueString(Type type, object value, string errorString)
        {
            bool flag = typeof(EntityType).IsAssignableFrom(type);
            string result;
            if (flag)
            {
                EntityType entityType = (EntityType)value;
                result = entityType.Name;
            }
            else
            {
                bool flag2 = typeof(Entity).IsAssignableFrom(type);
                if (flag2)
                {
                    Entity entity = (Entity)value;
                    result = entity.UIN.ToString();
                }
                else
                {
                    bool flag3 = typeof(Type) == type;
                    if (flag3)
                    {
                        Type type2 = (Type)value;
                        result = type2.FullName;
                    }
                    else
                    {
                        bool flag4 = SimpleTypesUtils.IsSimpleType(type);
                        if (flag4)
                        {
                            result = value.ToString();
                        }
                        else
                        {
                            bool flag5 = errorString != null;
                            if (flag5)
                            {
                                Log.Fatal("Entity System: Serialization for type \"{0}\" are not supported ({1}).", type.ToString(), errorString);
                                return null;
                            }
                            result = null;
                        }
                    }
                }
            }
            return result;
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

        private static bool SaveCollectionFieldValue(bool flag, object obj, FieldInfo fieldInfo, TextBlock textBlock, object obj2, string text)
        {
            object value = fieldInfo.GetValue(obj);
            bool isArray = fieldInfo.FieldType.IsArray;

            List<object> items = new List<object>();
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
            else
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
            bool flag2 = !flag && obj is EntityType && obj2 != null;
            bool result;
            if (flag2)
            {
                bool isArray2 = fieldInfo.FieldType.IsArray;
                bool flag3;
                if (isArray2)
                {
                    PropertyInfo property4 = fieldInfo.FieldType.GetProperty("Length");
                    int num3 = (int)property4.GetValue(obj2, null);
                    flag3 = (num3 == 0);
                }
                else
                {
                    PropertyInfo property5 = fieldInfo.FieldType.GetProperty("Count");
                    int num4 = (int)property5.GetValue(obj2, null);
                    flag3 = (num4 == 0);
                }
                bool flag4 = items.Count == 0 & flag3;
                if (flag4)
                {
                    result = true;
                    return result;
                }
            }
            bool isArray3 = fieldInfo.FieldType.IsArray;
            Type type;
            if (isArray3)
            {
                type = fieldInfo.FieldType.GetElementType();
            }
            else
            {
                type = fieldInfo.FieldType.GetGenericArguments()[0];
            }
            string text2 = EntityHelper.GetFieldSerializeName(flag, fieldInfo);
            bool flag5 = type == typeof(string);
            if (flag5)
            {
                TextBlock textBlock2 = textBlock.AddChild(text2); 
                for (int k = 0; k < items.Count; k++)
                {
                    object obj3 = items[k];
                    TextBlock textBlock3 = textBlock2.AddChild("item");
                    bool flag6 = obj3 != null;
                    if (flag6)
                    {
                        textBlock3.SetAttribute("value", (string)obj3);
                    }
                    else
                    {
                        textBlock3.SetAttribute("_nullItem", true.ToString());
                    }
                }
                result = true;
            }
            else
            {
                bool flag7 = typeof(EntityType).IsAssignableFrom(type);
                bool flag8 = typeof(Entity).IsAssignableFrom(type);
                bool flag9 = (SimpleTypesUtils.IsSimpleType(type) | flag7 | flag8) || typeof(Type) == type;
                if (flag9)
                {
                    bool flag10 = !flag & flag8;
                    if (flag10)
                    {
                        Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", text);
                        return false;
                    }
                    char value2 = ';';
                    bool flag11 = type.IsPrimitive | flag8;
                    if (flag11)
                    {
                        value2 = ' ';
                    }
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int l = 0; l < items.Count; l++)
                    {
                        object obj4 = items[l];
                        bool flag12 = flag8 && obj4 != null;
                        if (flag12)
                        {
                            Entity entity = (Entity)obj4;
                            bool flag13 = !entity.AllowSaveHerit();
                            if (flag13)
                            {
                                Log.Fatal("Entity System: Serialization error. The reference to entity which does not allow serialization ({0}). Field to serialize: \"{1}\".", entity.ToString(), text);
                                return false;
                            }
                        }
                        bool flag14 = l != 0;
                        if (flag14)
                        {
                            stringBuilder.Append(value2);
                        }
                        bool flag15 = obj4 != null;
                        if (flag15)
                        {
                            stringBuilder.Append(EntityHelper.GetSaveValueString(type, obj4, text + ": " + text2));
                        }
                        else
                        {
                            stringBuilder.Append("null");
                        }
                    }
                    textBlock.SetAttribute(text2, stringBuilder.ToString());
                    result = true;
                }
                else
                {
                    TextBlock textBlock4 = textBlock.AddChild(text2);
                    List<FieldInfo> list = EntityHelper.A(flag, type); 
                    int m = 0;
                    while (m < items.Count)
                    {
                        object obj5 = items[m];
                        TextBlock textBlock5 = textBlock4.AddChild("item");
                        bool flag16 = obj5 != null;
                        if (flag16)
                        {
                            using (List<FieldInfo>.Enumerator enumerator = list.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    FieldInfo current = enumerator.Current;
                                    object defaultValue = null;
                                    DefaultValueAttribute[] array6 = (DefaultValueAttribute[])current.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                                    bool flag17 = array6.Length != 0;
                                    if (flag17)
                                    {
                                        defaultValue = array6[0].Value;
                                    }
                                    bool flag18 = !EntityHelper.SaveFieldValue(flag, obj5, current, textBlock5, defaultValue, text);
                                    if (flag18)
                                    {
                                        result = false;
                                        return result;
                                    }
                                }
                                goto IL_4BF;
                            }
                            goto IL_4BD;
                        }
                        goto IL_4BD;
                        IL_4BF:
                        m++;
                        continue;
                        IL_4BD:
                        textBlock5.SetAttribute("_nullItem", true.ToString());
                        goto IL_4BF;
                    }
                    result = true;
                }
            }
            return result;
        }
        public static bool SaveFieldValue(bool entitySerialize, object owner, FieldInfo field, TextBlock block, object defaultValue, string errorString)
        {
            object value = field.GetValue(owner);
            bool flag = value == null;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                string text = EntityHelper.GetFieldSerializeName(entitySerialize, field);
                errorString += string.Format(", property: \"{0}\"", text);
                bool IsList = field.FieldType.IsGenericType && field.FieldType.Name == typeof(List<>).Name;
                bool isArray = field.FieldType.IsArray;
                bool FieldIsCollection = IsList | isArray;
                if (FieldIsCollection)
                {
                    bool flag4 = isArray && field.FieldType.GetArrayRank() != 1;
                    if (flag4)
                    {
                        Log.Fatal("Entity System: Serialization of arrays are supported only for one dimensions arrays ({0}).", errorString);
                        return false;
                    }
                    result = SaveCollectionFieldValue(entitySerialize, owner, field, block, defaultValue, errorString);
                }
                else
                {
                    bool flag5 = typeof(EntityType).IsAssignableFrom(field.FieldType);
                    bool flag6 = typeof(Entity).IsAssignableFrom(field.FieldType);
                    bool flag7 = (SimpleTypesUtils.IsSimpleType(field.FieldType) | flag5 | flag6) || typeof(Type) == field.FieldType;
                    if (flag7)
                    {
                        bool flag8 = !entitySerialize & flag6;
                        if (flag8)
                        {
                            Log.Fatal("Entity System: Serialization Entity classes in entity types is forbidden ({0}).", errorString);
                            return false;
                        }
                        bool flag9 = flag6;
                        if (flag9)
                        {
                            Entity entity = (Entity)value;
                            bool flag10 = !entity.AllowSaveHerit();
                            if (flag10)
                            {
                                Log.Fatal("Entity System: Serialization error. The reference to entity which does not allow serialization ({0}). Field to serialize: \"{1}\".", entity.ToString(), errorString);
                                return false;
                            }
                        }
                        string saveValueString = EntityHelper.GetSaveValueString(field.FieldType, value, errorString);
                        string text2 = null;
                        bool flag11 = defaultValue != null;
                        if (flag11)
                        {
                            bool isEnum = field.FieldType.IsEnum;
                            if (isEnum)
                            {
                                text2 = Enum.GetName(field.FieldType, defaultValue);
                            }
                            else
                            {
                                text2 = defaultValue.ToString();
                            }
                        }
                        bool flag12 = (text2 != null && text2 != saveValueString) || (defaultValue == null && saveValueString != "");
                        if (flag12)
                        {
                            block.SetAttribute(text, saveValueString);
                        }
                        result = true;
                    }
                    else
                    {
                        TextBlock block2 = block.AddChild(text);
                        Type type = field.FieldType;
                        bool flag13 = field.FieldType == typeof(LogicEntityObject);
                        if (flag13)
                        {
                            type = field.GetValue(owner).GetType();
                        }
                        while (type != null)
                        {
                            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            FieldInfo[] array = fields;
                            int i = 0;
                            while (i < array.Length)
                            {
                                FieldInfo fieldInfo = array[i];
                                if (entitySerialize)
                                {
                                    Entity.FieldSerializeAttribute[] array2 = (Entity.FieldSerializeAttribute[])fieldInfo.GetCustomAttributes(typeof(Entity.FieldSerializeAttribute), true);
                                    bool flag14 = array2.Length != 0;
                                    if (flag14)
                                    {
                                        bool flag15 = EntitySystemWorld.Instance.isEntitySerializable(array2[0].SupportedSerializationTypes);
                                        if (flag15)
                                        {
                                            goto IL_2DA;
                                        }
                                    }
                                }
                                else
                                {
                                    bool flag16 = fieldInfo.GetCustomAttributes(typeof(EntityType.FieldSerializeAttribute), true).Length != 0;
                                    if (flag16)
                                    {
                                        goto IL_2DA;
                                    }
                                }
                                IL_2D1:
                                i++;
                                continue;
                                IL_2DA:
                                object defaultValue2 = null;
                                DefaultValueAttribute[] array3 = (DefaultValueAttribute[])fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                                bool flag17 = array3.Length != 0;
                                if (flag17)
                                {
                                    defaultValue2 = array3[0].Value;
                                }
                                bool flag18 = !EntityHelper.SaveFieldValue(entitySerialize, value, fieldInfo, block2, defaultValue2, errorString);
                                if (flag18)
                                {
                                    result = false;
                                    return result;
                                }
                                goto IL_2D1;
                            }
                            type = type.BaseType;
                        }
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Reflection;
//using System.Collections.ObjectModel;

//namespace Globe.Xml.Serialization
//{
//    /// <summary>
//    /// Builds an object from a SerializableData object
//    /// </summary>
//    public class SerializableDataComposer : SerializableDataController
//    {
//        #region Constructors

//        /// <summary>
//        /// Default constructor.
//        /// </summary>
//        public SerializableDataComposer()
//        {
//        }

//        #endregion

//        #region Public Functions

//        /// <summary>
//        /// Gets a data object
//        /// </summary>
//        /// <param name="serializableData">Object from which create a data object</param>
//        /// <returns></returns>
//        virtual public object Compose(SerializableData serializableData)
//        {
//            object data = CreateObject(serializableData);
//            FillObject(ref data, serializableData);

//            return data;
//        }

//        #endregion

//        #region Protected Functions

//        /// <summary>
//        /// Creates an object of specified type name
//        /// </summary>
//        /// <param name="typeName">Type name of the object to build</param>
//        /// <returns>New object</returns>
//        virtual protected object CreateObject(SerializableData serializableData)
//        {
//            object obj = Activator.CreateInstance(serializableData.Assembly, serializableData.Type, null);

//            System.Runtime.Remoting.ObjectHandle objH = obj as System.Runtime.Remoting.ObjectHandle;
//            if (objH != null)
//                obj = objH.Unwrap();

//            return obj;
//        }

//        /// <summary>
//        /// Fills the object with fields values in SerializableData
//        /// </summary>
//        /// <param name="data">Object to fill</param>
//        /// <param name="serializableData">Fields values used to fill the object</param>
//        virtual protected void FillObject(ref object data, SerializableData serializableData)
//        {
//            foreach (SerializableData member in serializableData.SerializableDataCollection)
//                FillObjectField(data, data.GetType(), member);
//        }

//        /// <summary>
//        /// Fills a single field of the object
//        /// </summary>
//        /// <param name="data">Object to fill</param>
//        /// <param name="type">Type of the objet to fill</param>
//        /// <param name="serializableData">Field value</param>
//        virtual protected void FillObjectField(object data, Type type, SerializableData serializableData)
//        {
//            object rightObject = GetRightObject(serializableData);

//            if (IsCollection(rightObject) && !IsArray(rightObject.GetType()))
//            {
//                foreach (SerializableData internalData in serializableData.SerializableDataCollection)
//                {
//                    object parameters = Compose(internalData);
//                    InvokeAddingMethod(rightObject, new object[] { parameters });
//                }
//            }

//            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.SetProperty;

//            PropertyInfo property = GetProperty(type, serializableData.FieldName, flags);
//            FieldInfo field = GetField(type, serializableData.FieldName, flags);

//            try
//            {
//                if ((property != null && property.CanWrite) ||
//                    (field != null && !field.IsLiteral))
//                    type.InvokeMember(serializableData.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.SetProperty, null, data, new object[] { rightObject });
//                else if (!type.BaseType.Equals(typeof(object)) && type.BaseType != null)
//                    FillObjectField(data, type.BaseType, serializableData);
//            }
//            catch (Exception ex)
//            {
//                if (!type.BaseType.Equals(typeof(object)) && type.BaseType != null)
//                    FillObjectField(data, type.BaseType, serializableData);
//            }
//        }

//        /// <summary>
//        /// Gets the right object from SerializableData
//        /// </summary>
//        /// <param name="serializableData">SerializableData in which there are object type informations</param>
//        /// <returns>Right object</returns>
//        virtual protected object GetRightObject(SerializableData serializableData)
//        {
//            object rightObject = null;
//            Type rightType = Type.GetType(serializableData.AssemblyQualifiedName);

//            if (serializableData.Value != string.Empty)
//            {
//                try
//                {
//                    rightObject = Convert.ChangeType(serializableData.Value, rightType);
//                }
//                catch
//                {
//                    rightObject = CreateArray(rightType, serializableData);
//                }
//            }
//            else
//                rightObject = CreateArray(rightType, serializableData);

//            return rightObject;
//        }

//        virtual protected object CreateArray(Type type, SerializableData serializableData)
//        {
//            object rightObject = null;

//            try
//            {
//                if (IsArray(type))
//                {
//                    System.Array array = System.Array.CreateInstance(
//                        GetRightObject(serializableData.SerializableDataCollection[0]).GetType(),
//                        serializableData.SerializableDataCollection.Count);

//                    for (int i = 0; i < serializableData.SerializableDataCollection.Count; i++)
//                        array.SetValue(GetRightObject(serializableData.SerializableDataCollection[i]), i);

//                    rightObject = array;
//                }
//                else
//                    rightObject = CreateObject(serializableData);

//                FillObject(ref rightObject, serializableData);
//            }
//            catch
//            {
//                throw new XmlSerializationException(rightObject, serializableData);
//            }

//            return rightObject;
//        }

//        /// <summary>
//        /// Invokes the adding method to populate a collection
//        /// </summary>
//        /// <param name="invoker">Collection</param>
//        /// <param name="parameters">Parameters, a single object to insert</param>
//        virtual protected void InvokeAddingMethod(object invoker, object[] parameters)
//        {
//            try
//            {
//                invoker.GetType().InvokeMember("Add", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.InvokeMethod, null, invoker, parameters);
//            }
//            catch
//            {
//                throw new XmlSerializationException(invoker, null);
//            }
//        }

//        #endregion
//    }
//}

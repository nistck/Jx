using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Jx.Drawing.Serialization.XML
{
    /// <summary>
    /// Manages all exceptions thrown by serialization.
    /// </summary>
    public class XmlSerializationException : ApplicationException
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">data that causes the error.</param>
        /// <param name="serializableData">SerializableData associated to error.</param>
        public XmlSerializationException(object data, SerializableData serializableData)
        {
            _data = data;
            _serializableData = serializableData;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">data that causes the error.</param>
        /// <param name="serializableData">SerializableData associated to error.</param>
        /// <param name="propertyInfo">PropertyInfo associated to error.</param>
        /// <param name="fieldInfo">FieldInfo associated to error.</param>
        public XmlSerializationException(object data, SerializableData serializableData, PropertyInfo propertyInfo, FieldInfo fieldInfo)
        {
            _data = data;
            _serializableData = serializableData;
            _propertyInfo = propertyInfo;
            _fieldInfo = fieldInfo;
        }

        #endregion

        #region Properties

        object _data = null;
        /// <summary>
        /// Gets the object that thrown the exception.
        /// </summary>
        public object DataInfo
        {
            get { return _data; }
        }

        SerializableData _serializableData = null;
        /// <summary>
        /// Gets the SerializableData that thrown the exception.
        /// </summary>
        public SerializableData SerializableDataInfo
        {
            get { return _serializableData; }
        }

        PropertyInfo _propertyInfo = null;
        /// <summary>
        /// Gets the PropertyInfo that can have generated error.
        /// </summary>
        public PropertyInfo Property
        {
            get { return _propertyInfo; }
        }

        FieldInfo _fieldInfo = null;
        /// <summary>
        /// Gets the FieldInfo that can have generated error.
        /// </summary>
        public FieldInfo Field
        {
            get { return _fieldInfo; }
        }

        /// <summary>
        /// Gets the message for the error.
        /// </summary>
        public override string Message
        {
            get
            {
                string message = base.Message;
                message += _data != null ? GetFormattedText(Jx.Drawing.Properties.Resources.Data) + _data.ToString() : GetFormattedText(Jx.Drawing.Properties.Resources.NoData);
                message += _serializableData != null ? GetFormattedText(Jx.Drawing.Properties.Resources.SerializableData) +
                    GetFormattedText(Jx.Drawing.Properties.Resources.Assembly) + _serializableData.Assembly +
                    GetFormattedText(Jx.Drawing.Properties.Resources.AssemblyQualifiedName) + _serializableData.AssemblyQualifiedName +
                    GetFormattedText(Jx.Drawing.Properties.Resources.FieldName) + _serializableData.FieldName +
                    GetFormattedText(Jx.Drawing.Properties.Resources.TagName) + _serializableData.TagName +
                    GetFormattedText(Jx.Drawing.Properties.Resources.Type) + _serializableData.Type +
                    GetFormattedText(Jx.Drawing.Properties.Resources.Value) + _serializableData.Value +
                    GetFormattedText(Jx.Drawing.Properties.Resources.SerializableDataCollectionCount) + _serializableData.SerializableDataCollection.Count.ToString() : GetFormattedText(Jx.Drawing.Properties.Resources.NoSerializableData);

                string propertyMessage = string.Empty;

                if (_propertyInfo != null)
                {
                    propertyMessage = Jx.Drawing.Properties.Resources.PropertyInfo + " " + _propertyInfo.Name + ": " ;
                    propertyMessage += _propertyInfo.CanRead ? Jx.Drawing.Properties.Resources.Readeable : Jx.Drawing.Properties.Resources.NoReadeable + "\n";
                    propertyMessage += Jx.Drawing.Properties.Resources.PropertyInfo + " " + _propertyInfo.Name + ": " ;
                    propertyMessage += _propertyInfo.CanWrite ? Jx.Drawing.Properties.Resources.Writeable : Jx.Drawing.Properties.Resources.NoWriteable + "\n";
                }

                string fieldMessage = string.Empty;

                if (_fieldInfo != null)
                {
                    fieldMessage = Jx.Drawing.Properties.Resources.FieldInfo + " " + _fieldInfo.Name + ": " ;
                    fieldMessage += _fieldInfo.IsLiteral ? Jx.Drawing.Properties.Resources.Constant : Jx.Drawing.Properties.Resources.NoConstant + "\n";
                }

                message += propertyMessage + fieldMessage;

                return message;
            }
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Gets formatted text.
        /// </summary>
        /// <param name="text">Text to fromat.</param>
        /// <returns>Formatted text.</returns>
        protected string GetFormattedText(string text)
        {
            return "\n" + text + ": ";
        }

        #endregion
    }
}

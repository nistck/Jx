using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Jx.Xml.Serialization
{
    /// <summary>
    /// Reads the xml and fill the SerializableData class.
    /// </summary>
    public class XmlSerializeReader
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public XmlSerializeReader()
        {
        }

        #endregion

        #region Properties

        XmlDocument _xmlDocument = new XmlDocument();
        /// <summary>
        /// XmlDocument that contains the DOM struct.
        /// </summary>
        public XmlDocument XmlDocument
        {
            get { return _xmlDocument; }
            set { _xmlDocument = value; }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Reads an xml file.
        /// </summary>
        /// <param name="fileName">Xml file to read.</param>
        /// <param name="serializableData">SerializableData to fill.</param>
        /// <returns>Filled class</returns>
        public object ReadXml(string fileName, SerializableData serializableData)
        {
            _xmlDocument.Load(fileName);

            try
            {
                ReadXml(_xmlDocument.ChildNodes[1], serializableData);
            }
            catch
            {
                throw new XmlSerializationException(_xmlDocument, serializableData);
            }

            return serializableData;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Reads an XmlNode.
        /// </summary>
        /// <param name="xmlNode">XmlNode to read.</param>
        /// <param name="serializableData">SerializableData to fill.</param>
        virtual protected void ReadXml(XmlNode xmlNode, SerializableData serializableData)
        {
            XmlAttribute xmlAttributeName = xmlNode.Attributes["name"];
            XmlAttribute xmlAttributeType = xmlNode.Attributes["type"];
            XmlAttribute xmlAttributeAssembly = xmlNode.Attributes["assembly"];
            XmlAttribute xmlAttributeAssemblyQualifiedName = xmlNode.Attributes["assemblyQualifiedName"];
            string alias = xmlNode.Name;
            XmlAttribute xmlAttributeValue = xmlNode.Attributes["value"];

            serializableData.TagName = alias;
            serializableData.Type = xmlAttributeType.Value;
            serializableData.Assembly = xmlAttributeAssembly.Value;
            serializableData.AssemblyQualifiedName = xmlAttributeAssemblyQualifiedName.Value;
            serializableData.FieldName = xmlAttributeName.Value;
            serializableData.Value = xmlAttributeValue.Value;
            
            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            {
                SerializableData newSerializableData = new SerializableData();
                serializableData.SerializableDataCollection.Add(newSerializableData);
                ReadXml(xmlChildNode, newSerializableData);
            }
        }

        #endregion
    }
}

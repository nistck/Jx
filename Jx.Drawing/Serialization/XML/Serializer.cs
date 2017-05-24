using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Jx.Serialization.XML
{
    /// <summary>
    /// Manages the serialization of objects.
    /// </summary>
    public class Serializer
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Serializer()
        {
        }

        #endregion

        #region Properties

        XmlSerializeWriter _serializeWriter = new XmlSerializeWriter();
        /// <summary>
        /// Gets or sets the component used to write xml file.
        /// </summary>
        public XmlSerializeWriter SerializeWriter
        {
            get { return _serializeWriter; }
            set { _serializeWriter = value; }
        }

        XmlSerializeReader _serializeReader = new XmlSerializeReader();
        /// <summary>
        /// Gets or sets the component used to read xml file.
        /// </summary>
        public XmlSerializeReader SerializeReader
        {
            get { return _serializeReader; }
            set { _serializeReader = value; }
        }

        SerializableDataComposer _composer = new SerializableDataComposer();
        /// <summary>
        /// Gets or sets the component used to compose a SerialzableData from a serialzable object.
        /// </summary>
        public SerializableDataComposer Composer
        {
            get { return _composer; }
            set { _composer = value; }
        }

        SerializableDataDecomposer _decomposer = new SerializableDataDecomposer();
        /// <summary>
        /// Gets or sets the component used to decompose a serialzable object from a SerializableData.
        /// </summary>
        public SerializableDataDecomposer Decomposer
        {
            get { return _decomposer; }
            set { _decomposer = value; }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Reset serializer state.
        /// </summary>
        virtual public void Reset()
        {
            _composer.SerializableDataInfo.Reset();
            _decomposer.SerializableDataInfo.Reset();
            _serializeReader.XmlDocument.RemoveAll();
            _serializeWriter.XmlDocument.RemoveAll();
        }

        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <param name="fileName">Filename.</param>
        /// <param name="data">Data to serialize.</param>
        virtual public void Serialize(string fileName, object data)
        {
            _decomposer.Decompose(data);
            _serializeWriter.WriteXml(fileName, _decomposer.SerializableDataInfo);
        }

        /// <summary>
        /// Deserialize an object.
        /// </summary>
        /// <param name="fileName">Filename.</param>
        /// <returns>Deserialized data.</returns>
        virtual public object Deserialize(string fileName)
        {
            _serializeReader.ReadXml(fileName, _decomposer.SerializableDataInfo);
            return _composer.Compose(_decomposer.SerializableDataInfo);
        }

        #endregion
    }
}

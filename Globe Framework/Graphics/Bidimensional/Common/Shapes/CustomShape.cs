using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Globe.Xml.Serialization;

namespace Globe.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Generic shape.
    /// </summary>
    [XmlClassSerializable("customShape")]
    public class CustomShape : Shape
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomShape()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="customShape">customShape to copy.</param>
        public CustomShape(CustomShape customShape) : base (customShape)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="geometric">Reference GraphicsPath.</param>
        public CustomShape(GraphicsPath geometric) : base(geometric)
        {
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the shape.
        /// </summary>
        /// <returns>New shape.</returns>
        public override object Clone()
        {
            return new CustomShape(this);
        }

        #endregion

        #endregion
    }
}

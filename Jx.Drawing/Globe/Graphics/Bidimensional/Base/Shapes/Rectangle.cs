using System;
using System.Collections.Generic;
using System.Text;

using Jx.Graphics.Bidimensional.Common;
using Jx.Xml.Serialization;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Rectangle shape.
    /// </summary>
    [XmlClassSerializable("rectangle")]
    public class Rectangle : Shape
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Rectangle()
        {
            Geometric.AddRectangle(new System.Drawing.Rectangle(0, 0, 1, 1));
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="rectangle">Rectangle to copy.</param>
        public Rectangle(Rectangle rectangle) : base(rectangle)
        {
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the shape.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Rectangle(this);
        }

        #endregion

        #endregion
    }
}
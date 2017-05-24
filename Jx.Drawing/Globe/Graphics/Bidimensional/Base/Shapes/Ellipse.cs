using System;
using System.Collections.Generic;
using System.Text;

using Jx.Graphics.Bidimensional.Common;
using Jx.Xml.Serialization;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Ellipse shape.
    /// </summary>
    [XmlClassSerializable("ellipse")]
    public class Ellipse : Shape
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Ellipse()
        {
            Geometric.AddEllipse(new System.Drawing.Rectangle(0, 0, 1, 1));
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="ellipse">Ellispe to copy.</param>
        public Ellipse(Ellipse ellipse) : base(ellipse)
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
            return new Ellipse(this);
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Jx.Drawing.Common;
using Jx.Drawing.Serialization.XML;

namespace Jx.Drawing.Base
{
    /// <summary>
    /// Line shape.
    /// </summary>
    [XmlClassSerializable("line")]
    public class Line : Shape
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Line()
        {
            Geometric.AddLine(new PointF(0, 0), new PointF(1, 1));

            Appearance = new LineAppearance();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="line">Line to copy.</param>
        public Line(Line line) : base(line)
        {
            Appearance = new LineAppearance();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        public Line(PointF start, PointF end)
        {
            if (start.X == end.X)
                end = new PointF(end.X + 1, end.Y);

            if (start.Y == end.Y)
                end = new PointF(end.X, end.Y + 1);

            Geometric.AddLine(start, end);
            Appearance = new LineAppearance();
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
            return new Line(this);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Gets the shape hit position
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns></returns>
        public override HitPositions HitTest(Point point)
        {
            float width = Dimension.Width;
            float height = Dimension.Height;

            if (width == 1 ||  height == 1)
            {
                using (Pen pen = new Pen(Color.Black, Appearance.GrabberDimension))
                {
                    if (Geometric.IsOutlineVisible(point, pen))
                        return HitPositions.Center;
                }
            }

            return base.HitTest(point);
        }

        #endregion

        #endregion
    }
}

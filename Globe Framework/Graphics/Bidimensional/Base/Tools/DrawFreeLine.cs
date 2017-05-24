using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.ObjectModel;

using Globe.Graphics.Bidimensional.Common;

namespace Globe.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Draw a free line.
    /// </summary>
    public class DrawFreeLine : DrawSloppedLine
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DrawFreeLine()
        {
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            MousePressed = false;

            IShape shape = CreateDrawingShape();
            if (shape == null)
                return;

            document.Shapes.Add(shape);
            Points.Clear();
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            if (!MousePressed)
                return;

            PointF newPoint = new PointF(
                e.Location.X/* + document.GridResolution.Width*/,
                e.Location.Y/* + document.GridResolution.Height*/);

            newPoint = document.GridManager.GetRoundedPoint(newPoint);

            if (Points.Count > 0)
            {
                PointF point = Points[Points.Count - 1];
                if (Math.Abs(point.X - newPoint.X) < _offset && Math.Abs(point.Y - newPoint.Y) < _offset)
                    return;
            }

            Points.Add(newPoint);

            base.MouseMove(document, e);
        }

        #endregion

        #region Properties

        float _offset = 5;
        /// <summary>
        /// Gets or sets the offset between two near points.
        /// </summary>
        public float Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        #endregion
    }
}

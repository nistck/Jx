using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Jx.Graphics.Bidimensional.Common;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Abstract class used for drawing operation.
    /// </summary>
    public abstract class Draw : Tool
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Draw()
        {
            Ghost = new Jx.Graphics.Bidimensional.Common.Ghost(new Jx.Graphics.Bidimensional.Base.Rectangle());
        }

        #endregion

        #region Properties

		Color _drawingColor = Color.LightBlue;
        /// <summary>
        /// Gets or sets the drawing color.
        /// </summary>
		public Color DrawingColor
		{
			get { return _drawingColor; }
			set { _drawingColor = value; }
		}

		#endregion

        #region IActions Interface

        /// <summary>
        /// Mouse down function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseDown(IDocument document, MouseEventArgs e)
        {
            base.MouseDown(document, e);

            Ghost.MirrorPoint = e.Location;
            Ghost.Location = new PointF(MouseDownPoint.X, MouseDownPoint.Y);
            Ghost.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            Ghost.MouseUp(document, e);
            base.MouseUp(document, e);
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

			document.DrawingControl.Invalidate();

            Ghost.MouseMove(document, e);

            int left, right, top, bottom;

			if (MouseDownPoint.X > e.X)
			{
				left = e.X;
				right = MouseDownPoint.X;
			}
			else
			{
				left = MouseDownPoint.X;
				right = e.X;
			}
			if (MouseDownPoint.Y > e.Y)
			{
				top = e.Y;
				bottom = MouseDownPoint.Y;
			}
			else
			{
				top = MouseDownPoint.Y;
				bottom = e.Y;
			}

            Ghost.Location = document.GridManager.GetRoundedPoint(new Point(left, top));
            Ghost.Dimension = document.GridManager.GetRoundedSize(new Size(right - left, bottom - top));
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            Ghost.Paint(document, e);
        }

        #endregion
    }
}

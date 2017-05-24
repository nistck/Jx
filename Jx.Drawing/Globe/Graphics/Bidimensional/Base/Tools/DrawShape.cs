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
    /// Draws and instantiates a shape.
    /// </summary>
    public class DrawShape : Draw
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="geometric">Reference GraphicsPath.</param>
        public DrawShape(GraphicsPath geometric)
        {
            IShape shape = new CustomShape(geometric.Clone() as GraphicsPath);
            Ghost = new Ghost(shape);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shape">Shape to draw.</param>
        public DrawShape(IShape shape)
        {
            Ghost = new Ghost(shape);
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

            PointF point = document.GridManager.GetRoundedPoint(e.Location);
            Ghost.Location = point;
            Ghost.MirrorPoint = point;

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

            IShape shape = Ghost.Shape.Clone() as IShape;
            shape.Visible = true;
            shape.Selected = true;
            document.Shapes.Add(shape);

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

            base.MouseMove(document, e);
            Ghost.MouseMove(document, e);
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

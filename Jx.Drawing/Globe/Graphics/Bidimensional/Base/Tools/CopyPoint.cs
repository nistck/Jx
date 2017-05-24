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
    /// Copies a point.
    /// </summary>
    public class CopyPoint : Deform
    {
        Ellipse _newPoint = new Ellipse();

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CopyPoint()
        {
            _newPoint.Selected = true;
            _newPoint.Visible = false;
            _newPoint.Location = System.Windows.Forms.Control.MousePosition;
            _newPoint.Dimension = new SizeF(10, 10);
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
            if (System.Windows.Forms.Control.ModifierKeys != Keys.Control)
                return;

            base.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            UpdateShape();

            Shape = null;
            IndexPoint = -1;
            _newPoint.Visible = false;
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            UpdateCursor(document, document.Shapes, e.Location);

            if (Shape == null || IndexPoint == -1 || System.Windows.Forms.Control.ModifierKeys != Keys.Control)
                return;

            _newPoint.Visible = true;
            _newPoint.Center = document.GridManager.GetRoundedPoint(e.Location);
            document.DrawingControl.Invalidate();
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            _newPoint.Paint(document, e);
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Updates the selected shape with new point.
        /// </summary>
        virtual protected void UpdateShape()
        {
            if (Shape == null || IndexPoint == -1)
                return;

            Shape.Transformer.CopyPoint(IndexPoint, true, _newPoint.Center, Shape.Geometric.PathTypes[IndexPoint]);
        }

        #endregion
    }
}

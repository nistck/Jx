using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Jx.Graphics.Bidimensional.Common;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Complex tool to select, resize and move shapes.
    /// </summary>
    public class Pointer : Resize
    {
        Tool _tool = new Move();

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pointer()
        {
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
            if (e.Button != MouseButtons.Left)
                return;

            base.MouseDown(document, e);

            HitPositions hitPosition = SelectShape(document.Shapes, e.Location);
 
            switch (hitPosition)
            {
                case HitPositions.Center:
                    _tool = new Move();
                    break;
                case HitPositions.None:
                    _tool = new MultiSelect();
                    break;
                default:
                    _tool = new Resize();
                    break;
            }

            _tool.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            _tool.MouseUp(document, e);
            base.MouseUp(document, e);
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            _tool.MouseMove(document, e);
            base.MouseMove(document, e);

            UpdateCursor(document, document.Shapes, e.Location);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            _tool.Paint(document, e);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Updates the cursor during the tool actions.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="shapes">Shapes to manage.</param>
        /// <param name="point">Mouse point.</param>
        /// <returns>True if it is updated.</returns> 
        public override bool UpdateCursor(IDocument document, ShapeCollection shapes, Point point)
        {
            foreach (IShape shape in shapes)
            {
                if (
                    shape.HitTest(point) != HitPositions.Center &&
                    shape.HitTest(point) != HitPositions.All && 
                    shape.HitTest(point) != HitPositions.None)
                {
                    if (MousePressed)
                        return false;

                    return base.UpdateCursor(document, shapes, point);
                }
                else if (shape.HitTest(MouseDownPoint) == HitPositions.Center  && shape.Selected && MousePressed)
                {
                    document.ActiveCursor = Cursors.SizeAll;
                    return true;
                }
            }

            if (MousePressed)
                return false;

            document.ActiveCursor = Cursors.Default;
            return false;
        }

        #endregion
    }
}

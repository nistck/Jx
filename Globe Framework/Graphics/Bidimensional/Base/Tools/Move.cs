using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.ObjectModel;

using Globe.Graphics.Bidimensional.Common;

namespace Globe.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Moves selected shapes.
    /// </summary>
    public class Move : Select
    {
        Point _oldPoint = Point.Empty;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Move()
        {
            this.Ghost = new GhostCollection();
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

            (this.Ghost as GhostCollection).Ghosts = Select.GetSelectedShapes(document.Shapes);
            (this.Ghost as GhostCollection).ShapeMouseUp += new MouseUpOnShape(Move_ShapeMouseUp);
            this.Ghost.MouseDown(document, e);

            UpdateCursor(document, Select.GetSelectedShapes(document.Shapes), e.Location);
        }

        /// <summary>
        /// Mouse up function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
        public override void MouseUp(IDocument document, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Ghost.MouseUp(document, e);
            base.MouseUp(document, e);
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(Globe.Graphics.Bidimensional.Common.IDocument document, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(document, e);

            if (!MousePressed)
                return;

           PointF point = document.GridManager.GetRoundedPoint(e.Location);

            Ghost.Transformer.Translate(point.X - _oldPoint.X, point.Y - _oldPoint.Y);
            Ghost.MouseMove(document, e);

            _oldPoint = Point.Round(point);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            this.Ghost.Paint(document, e);
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
            bool updated = false;

            foreach (IShape shape in shapes)
            {
                if (shape.HitTest(MouseDownPoint) != HitPositions.None && shape.Selected && MousePressed)
                    updated = true;
                else
                    updated = false;
            }

            if (updated)
                document.ActiveCursor = Cursors.SizeAll;
            else
                document.ActiveCursor = Cursors.Default;

            return updated;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Gets ghostable shapes.
        /// </summary>
        /// <param name="shapes">Shape on drawing panel.</param>
        virtual protected ShapeCollection GetGhostableShapes(ShapeCollection shapes)
        {
            ShapeCollection ghostableShapes = new ShapeCollection();

            foreach (IShape shape in shapes)
            {
                if (shape.Selected && !shape.Locked)
                    ghostableShapes.Add(shape);
            }

            return ghostableShapes;
        }

        /// <summary>
        /// Manages ghost mouse up event.
        /// </summary>
        /// <param name="shape">Ghost to fire the event.</param>
        /// <param name="document">Reference document.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual protected void Move_ShapeMouseUp(IShape shape, IDocument document, MouseEventArgs e)
        {
            Ghost ghost = shape as Ghost;
            if (ghost == null)
                return;

            if (ghost is GhostCollection || ghost.ReferenceShape == null)
                return;

            ghost.ReferenceShape.Location = ghost.Location;
        }

        #endregion
    }
}

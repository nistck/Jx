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
    /// Moves a single shape point.
    /// </summary>
    public class Deform : Select
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Deform()
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
            base.MouseDown(document, e);

            SelectIndexPointShapeCouple(document.Shapes, e.Location);

            if (_shape != null)
            {
                this.Ghost.Shape = _shape;
                this.Ghost.MouseDown(document, e);
            }
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            base.MouseUp(document, e);

            if (_shape == null)
                return;

            Ghost.MouseUp(document, e);
            _shape.Transformer.Deform(_indexPoint, document.GridManager.GetRoundedPoint(e.Location));

            _shape = null;
            _indexPoint = -1;
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            UpdateCursor(document, document.Shapes, e.Location);

            if (_shape == null || _indexPoint == -1)
                return;

            Ghost.MouseMove(document, e);
            Ghost.Transformer.Deform(_indexPoint, document.GridManager.GetRoundedPoint(e.Location));

            document.DrawingControl.Invalidate();
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

        #region Properties

        int _indexPoint = -1;
        /// <summary>
        /// Gets or sets the index point of selected shape.
        /// </summary>
        protected int IndexPoint
        {
            get { return _indexPoint; }
            set { _indexPoint = value; }
        }

        IShape _shape = null;
        /// <summary>
        /// Gets or sets selected shape.
        /// </summary>
        protected IShape Shape
        {
            get { return _shape; }
            set { _shape = value; }
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
            if (MousePressed)
                return false;

            if (Select.LastSelectedShape == null)
                return false;

            RectangleF[] markers = Select.LastSelectedShape.GetMarkers();
            foreach (RectangleF marker in markers)
            {
                if (marker.Contains(point))
                {
                    document.ActiveCursor = Cursors.Cross;
                    return true;
                }
            }

            document.ActiveCursor = Cursors.Default;
            return false;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Selects shape and shape point.
        /// </summary>
        /// <param name="shapes">Shape collection.</param>
        /// <param name="point">Point to check.</param>
        protected void SelectIndexPointShapeCouple(ShapeCollection shapes, PointF point)
        {
            if (Select.LastSelectedShape == null)
                return;

            _shape = Select.LastSelectedShape;
            _indexPoint = _shape.GetMarkerIndex(point);

            //for (int i = shapes.Count - 1; i >= 0; i--)
            //{
            //    if (!shapes[i].Selected)
            //        continue;

            //    _indexPoint = shapes[i].GetMarkerIndex(point);
            //    if (_indexPoint == -1)
            //        continue;

            //    _shape = shapes[i];

            //    break;
            //}
        }

        #endregion
    }
}

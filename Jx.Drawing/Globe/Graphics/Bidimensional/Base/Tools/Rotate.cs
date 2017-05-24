using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Jx.Graphics.Bidimensional.Common;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Rotates selected shapes. Press "Ctrl + MouseDown" to free rotation.
    /// </summary>
    public class Rotate : Select
    {
        float _degree = 0;
        Jx.Graphics.Bidimensional.Base.Image _hand = new Jx.Graphics.Bidimensional.Base.Image();
        Point _oldMouseLocation = new Point();

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Rotate()
        {
            _hand.Bitmap = Resource.Rotate.ToBitmap();
            _hand.Selected = true;
           _hand.Dimension = new Size(9, 9);

            Ghost = new GhostCollection();
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
            if (Control.ModifierKeys != Keys.Control)
                base.MouseDown(document, e);
            else
            {
                _centered = false;
                MousePressed = true;
                MouseDownPoint = e.Location;

                SelectShape(document.Shapes, e.Location);

                foreach (IShape shape in document.Shapes)
                    shape.MouseDown(document, e);
            }

            (Ghost as GhostCollection).Ghosts = Select.GetSelectedShapes(document.Shapes);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            foreach (IShape shape in document.Shapes)
            {
                PointF point = PointF.Empty;

                if (_centered)
                    point = new PointF(shape.Location.X + shape.Dimension.Width/2 - 3, shape.Location.Y + shape.Dimension.Height/2 - 3);
                else
                    point = MouseDownPoint;

                shape.Transformer.Rotate(_degree, point);
            }

            base.MouseUp(document, e);

            _degree = 0;
            _centered = true;
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

            PointF point = PointF.Empty;

            if (_centered && Select.LastSelectedShape != null)
                point = new PointF(Select.LastSelectedShape.Location.X + Select.LastSelectedShape.Dimension.Width/2 - 3, Select.LastSelectedShape.Location.Y + Select.LastSelectedShape.Dimension.Height/2 - 3);
            else
                point = MouseDownPoint;

            float degree = GetDegree(point.X, _oldMouseLocation.X, e.X, _step);
            _degree += degree;

            Ghost.Transformer.Rotate(degree, point);
            Ghost.MouseMove(document, e);

            document.DrawingControl.Invalidate();

            _oldMouseLocation = e.Location;

            base.MouseMove(document, e);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            base.Paint(document, e);

            if (!MousePressed)
                return;

            PointF point = PointF.Empty;

            if (_centered && Select.LastSelectedShape != null)
                point = new PointF(Select.LastSelectedShape.Location.X + Select.LastSelectedShape.Dimension.Width/2 - 3, Select.LastSelectedShape.Location.Y + Select.LastSelectedShape.Dimension.Height/2 - 3);
            else
                point = MouseDownPoint;

            _hand.Location = point;
            _hand.Paint(document, e);

            this.Ghost.Paint(document, e);
        }

        #endregion

        #region Properties

        bool _centered = true;
        /// <summary>
        /// Gets the rotation type. If it is free or relative to shape center.
        /// </summary>
        public bool Centered
        {
            get { return _centered; }
        }

        float _step = 50f;
        /// <summary>
        /// Gets or sets the rotation step.
        /// </summary>
        public float Step
        {
            get { return _step; }

            set
            {
                _step = value;
                if (_step < 1)
                    _step = 1;
                if (_step > 360)
                    _step = 360;
            }
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
                if (shape.HitTest(point) != HitPositions.None)
                {
                    document.ActiveCursor = Cursors.Hand;
                    updated = true;
                    break;
                }
            }

            return updated;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Returns degree to rotate.
        /// </summary>
        /// <param name="referenceX">Rotation reference x.</param>
        /// <param name="oldX">Old mouse x position.</param>
        /// <param name="currentX">Current mouse x position.</param>
        /// <param name="step">Rotation step.</param>
        /// <returns>Degree.</returns>
        protected float GetDegree(float referenceX, float oldX, float currentX, float step)
        {
            float degree = (currentX - referenceX) / step;

            if (currentX > referenceX)
            {
                if (oldX > currentX)
                    degree = -degree;
            }
            else
            {
                if (oldX < currentX)
                    degree = -degree;
            }

            return degree;
        }

        #endregion
    }
}

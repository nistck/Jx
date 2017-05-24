using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.ObjectModel;

using Jx.Graphics.Bidimensional.Common;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Draw a slopped line.
    /// </summary>
    public class DrawSloppedLine : Tool
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DrawSloppedLine()
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
            base.MouseUp(document, e);

            if (e.Button == MouseButtons.Left)
                _points.Add(document.GridManager.GetRoundedPoint(e.Location));
            else if (e.Button == MouseButtons.Right)
            {
                IShape shape = CreateDrawingShape();
                if (shape == null)
                    return;

                document.Shapes.Add(shape);
                _points.Clear();
            }
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            base.MouseMove(document, e);

            document.DrawingControl.Invalidate();
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            if (_points.Count == 0)
                return;

            PointF[] points = ToPointF(_points);
            if (points == null)
                return;

            if (points.GetLength(0) > 1)
                e.Graphics.DrawLines(Pens.Black, points);
    
            e.Graphics.DrawLine(Pens.Black, _points[_points.Count - 1], document.GridManager.GetRoundedPoint(document.DrawingControl.PointToClient(Control.MousePosition)));
        }

        #endregion

        #region Properties

        IShape _shape = new CustomShape();
        /// <summary>
        /// Gets the drawn shape.
        /// </summary>
        protected IShape DrawingShape
        {
            get { return _shape; }
        }

        Collection<PointF> _points = new Collection<PointF>();
        /// <summary>
        /// Gets the drawn points.
        /// </summary>
        protected Collection<PointF> Points
        {
            get { return _points; }
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
            document.ActiveCursor = Cursors.Cross;
            return true;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Tranforms a PointF collection into PointF[].
        /// </summary>
        /// <param name="collection">Collection to transform.</param>
        /// <returns>Points.</returns>
        protected PointF[] ToPointF(Collection<PointF> collection)
        {
            if (collection == null || collection.Count == 0)
                return null;

            PointF[] points = new PointF[collection.Count];
            collection.CopyTo(points, 0);

            return points;
        }

        /// <summary>
        /// Creates a shape relative to actual points.
        /// </summary>
        /// <returns>New Shape.</returns>
        virtual protected IShape CreateDrawingShape()
        {
            if (_points.Count <= 1)
                return null;

            if (_points.Count == 2)
                _shape = new Jx.Graphics.Bidimensional.Base.Line(_points[0], _points[1]);
            else
                _shape.Geometric.AddLines(ToPointF(_points));

            IShape shape = _shape.Clone() as IShape;
            shape.Selected = true;

            return shape;
        }

        #endregion
    }
}

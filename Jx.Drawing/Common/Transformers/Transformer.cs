using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Manages the shape (not CompositeShape, but only base shape) tranformations (just like move, resize, rotate etc.).
    /// </summary>
    public class Transformer
    {
        #region Events and Delegates

        /// <summary>
        /// Fires when a movement occurs.
        /// </summary>
        virtual public event MovementHandler MovementOccurred;

        /// <summary>
        /// Fires when a translate movement occurs.
        /// </summary>
        virtual public event TranslateHandler TranslateOccurred;

        /// <summary>
        /// Fires when a scale movement occurs.
        /// </summary>
        virtual public event ScaleHandler ScaleOccurred;

        /// <summary>
        /// Fires when a rotate movement occurs.
        /// </summary>
        virtual public event RotateHandler RotateOccurred;

        /// <summary>
        /// Fires when a deform movement occurs.
        /// </summary>
        virtual public event DeformHandler DeformOccurred;

        /// <summary>
        /// Fires when a mirror horizontal movement occurs.
        /// </summary>
        virtual public event MirrorHorizontalHandler MirrorHorizontalOccurred;

        /// <summary>
        /// Fires when a mirror vertical movement occurs.
        /// </summary>
        virtual public event MirrorVerticalHandler MirrorVerticalOccurred;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shape">Shape to manage.</param>
        public Transformer(IShape shape)
        {         
            if (shape == null)         
                throw new ApplicationException();

            _shape = shape;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Translates the shape.
        /// </summary>
        /// <param name="offsetX">Offset x.</param>
        /// <param name="offsetY">Offset y.</param>
        virtual public void Translate(float offsetX, float offsetY)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(offsetX, offsetY);
                _shape.Geometric.Transform(matrix);
            }

            if (TranslateOccurred != null && (offsetX != 0 || offsetY != 0))
                TranslateOccurred(this, offsetX, offsetY);

            if (MovementOccurred != null && (offsetX != 0 || offsetY != 0))
                MovementOccurred(this);
        }
		
        /// <summary>
        /// Scales the shape.
        /// </summary>
        /// <param name="scaleX">Scale x.</param>
        /// <param name="scaleY">Scale y.</param>
		public void Scale(float scaleX, float scaleY)
        {
            Scale(scaleX, scaleY, HitPositions.BottomRight);
        }

        /// <summary>
        /// Scales the shape.
        /// </summary>
        /// <param name="scaleX">Scale x.</param>
        /// <param name="scaleY">Scale y.</param>
        /// <param name="reference">Reference hit position.</param>
        public void Scale(float scaleX, float scaleY, HitPositions reference)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            if (reference == HitPositions.None || reference == HitPositions.Center)
                return;

            PointF grabberPoint = _shape.GetGrabberPoint(reference);

            Scale(scaleX, scaleY, grabberPoint);
        }

        /// <summary>
        /// Scales the shape.
        /// </summary>
        /// <param name="scaleX">Scale x.</param>
        /// <param name="scaleY">Scale y.</param>
        /// <param name="point">Reference point.</param>
        virtual public void Scale(float scaleX, float scaleY, PointF point)
        {
            if (point.IsEmpty)
                return;

            if (scaleX <= 0.1 || float.IsNaN(scaleX) || float.IsInfinity(scaleX))
                return;
            if (scaleY <= 0.1 || float.IsNaN(scaleY) || float.IsInfinity(scaleY))
                return;
           
            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(-point.X, -point.Y, MatrixOrder.Append);
                matrix.Scale(scaleX, scaleY, MatrixOrder.Append);
                matrix.Translate(point.X, point.Y, MatrixOrder.Append);

                _shape.Geometric.Transform(matrix);
            }

            if (ScaleOccurred != null /*&& (scaleX != 1 || scaleY != 1)*/)
                ScaleOccurred(this, scaleX, scaleY, point);

            if (MovementOccurred != null)
                MovementOccurred(this);
        }

        /// <summary>
        /// Rotates the shape.
        /// </summary>
        /// <param name="degree">Rotation degree.</param>
		public void Rotate(float degree)
        {
            PointF refPoint = new PointF(
                _shape.Location.X + _shape.Dimension.Width / 2f,
                _shape.Location.Y + _shape.Dimension.Height / 2f);

            Rotate(degree, refPoint);
        }

        /// <summary>
        /// Rotates the shape.
        /// </summary>
        /// <param name="degree">Rotation degree.</param>
        /// <param name="point">Reference point.</param>
		virtual public void Rotate(float degree, PointF point)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            //_shape.Rotation = degree;

            using (Matrix matrix = new Matrix())
            {
                matrix.RotateAt(degree, point);
                _shape.Geometric.Transform(matrix);
            }

            if (RotateOccurred != null && degree != 0)
                RotateOccurred(this, degree, point);

            if (MovementOccurred != null && degree != 0)
                MovementOccurred(this);
        }

        /// <summary>
        /// Deforms the shape.
        /// </summary>
        /// <param name="indexPoint">Index point to deform.</param>
        /// <param name="newPoint">New point.</param>
        virtual public void Deform(int indexPoint, PointF newPoint)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            if (indexPoint == -1 || _shape.Geometric.PathData.Points.Length == 0)
                return;

            PointF[] points = new PointF[_shape.Geometric.PathData.Points.Length];
            _shape.Geometric.PathData.Points.CopyTo(points, 0);

            PointF memory = points[indexPoint];
            points[indexPoint] = newPoint;

            GraphicsPath geometric = new GraphicsPath(points, _shape.Geometric.PathTypes);
            _shape.Geometric.Reset();
            _shape.Geometric.AddPath(geometric, false);

            if (DeformOccurred != null && memory != newPoint)
                DeformOccurred(this, indexPoint, newPoint);

            if (MovementOccurred != null && memory != newPoint)
                MovementOccurred(this);
        }

        /// <summary>
        /// Copies a shape point, updating the shape itself.
        /// </summary>
        /// <param name="indexPoint">Reference index point.</param>
        /// <param name="before">Before or after reference index point.</param>
        /// <param name="newPoint">New point.</param>
        /// <param name="pointType">New point type.</param>
        virtual public void CopyPoint(int indexPoint, bool before, PointF newPoint, byte pointType)
        {
            //GraphicsPathIterator iterator = new GraphicsPathIterator(this.Shape.Geometric);
            //PointF[] points = new PointF[this.Shape.Geometric.PointCount + 1];
            //byte[] types = new byte[this.Shape.Geometric.PointCount + 1];

            //int startIndex1 = -1;
            //int endIndex1 = -1;
            //int startIndex2 = -1;
            //int endIndex2 = -1;

            //if (before)
            //{
            //    startIndex1 = 0;
            //    endIndex1 = indexPoint - 1;
            //    startIndex2 = indexPoint;
            //    endIndex2 = this.Shape.Geometric.PointCount;
            //}
            //else
            //{
            //    startIndex1 = 0;
            //    endIndex1 = indexPoint;
            //    startIndex2 = indexPoint + 1;
            //    endIndex2 = this.Shape.Geometric.PointCount;
            //}

            //for (int i = 0; i < this.Shape.Geometric.PointCount; i++)
            //{
            //    if (i == indexPoint)
            //        continue;

            //    PointF point = this.Shape.Geometric.PathPoints[i];
            //    byte type = this.Shape.Geometric.PathTypes[i];

            //    points[i] = point;
            //    types[i] = type;
            //}

            //points[indexPoint] = newPoint;
            //types[indexPoint] = pointType;
            //types[3] = 1;
            //types[4] = 129;

            //GraphicsPath graphicsPath = new GraphicsPath(points, types);

            //this.Shape.Geometric.Reset();
            //this.Shape.Geometric.AddPath(graphicsPath, false);
        }

        /// <summary>
        /// Flips the shape horizontally.
        /// </summary>
        /// <param name="x">Flip axis x.</param>
        virtual public void MirrorHorizontal(float x)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            float newX = 2 * x - _shape.Location.X - _shape.Dimension.Width;
            PointF newLocation = new PointF(newX, _shape.Location.Y);

            using (Matrix matrix = new Matrix(-1, 0, 0, 1, 0, 0))
            {
                _shape.Geometric.Transform(matrix);
            }

            float offsetX = newLocation.X - _shape.Location.X;
            float offsetY = newLocation.Y - _shape.Location.Y;

            Translate(offsetX, offsetY);

            if (MirrorHorizontalOccurred != null)
                MirrorHorizontalOccurred(this, x);

            if (MovementOccurred != null)
                MovementOccurred(this);
        }

        /// <summary>
        /// Flips the shape horizontally relative to center shape.
        /// </summary>
        public void MirrorHorizontal()
        {
            MirrorHorizontal(_shape.Center.X);
        }

        /// <summary>
        /// Flips the shape vertically.
        /// </summary>
        /// <param name="y">Flip axis y.</param>
        virtual public void MirrorVertical(float y)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            float newY = 2 * y - _shape.Location.Y - _shape.Dimension.Height;
            PointF newLocation = new PointF(_shape.Location.X, newY);

            using (Matrix matrix = new Matrix(1, 0, 0, -1, 0, 0))
            {
                _shape.Geometric.Transform(matrix);
            }

            float offsetX = newLocation.X - _shape.Location.X;
            float offsetY = newLocation.Y - _shape.Location.Y;

            Translate(offsetX, offsetY);

            if (MirrorVerticalOccurred != null)
                MirrorVerticalOccurred(this, y);

            if (MovementOccurred != null)
                MovementOccurred(this);
        }
        
        /// <summary>
        /// Flips the shape vertically relative to center shape.
        /// </summary>
        public void MirrorVertical()
        {
            MirrorVertical(_shape.Center.Y);
        }

        #endregion

        #region Properties

        IShape _shape = null;
        /// <summary>
        /// Gets the shape to manage.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public IShape Shape
        {
            get { return _shape; }
        }

        #endregion
    }
}

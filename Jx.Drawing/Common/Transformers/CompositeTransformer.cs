using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Manages the composite shape tranformations (just like move, resize, rotate etc.).
    /// </summary>
    public class CompositeTransformer : Transformer
    {
        CompositeShape _shape = null;

        #region Events and Delegates

        /// <summary>
        /// Fires when a movement occurs.
        /// </summary>
        override public event MovementHandler MovementOccurred;

        /// <summary>
        /// Fires when a translate movement occurs.
        /// </summary>
        override public event TranslateHandler TranslateOccurred;

        /// <summary>
        /// Fires when a scale movement occurs.
        /// </summary>
        override public event ScaleHandler ScaleOccurred;

        /// <summary>
        /// Fires when a rotate movement occurs.
        /// </summary>
        override public event RotateHandler RotateOccurred;

        /// <summary>
        /// Fires when a deform movement occurs.
        /// </summary>
        override public event DeformHandler DeformOccurred;

        /// <summary>
        /// Fires when a mirror horizontal movement occurs.
        /// </summary>
        override public event MirrorHorizontalHandler MirrorHorizontalOccurred;

        /// <summary>
        /// Fires when a mirror vertical movement occurs.
        /// </summary>
        override public event MirrorVerticalHandler MirrorVerticalOccurred;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shape">Composite shape to manage.</param>
        public CompositeTransformer(CompositeShape shape) : base (shape)
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
        override public void Translate(float offsetX, float offsetY)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            if (!_shape.MovementContentBlocked)
                foreach (IShape shape in _shape.Shapes)
                    shape.Transformer.Translate(offsetX, offsetY);

            base.Translate(offsetX, offsetY);

            if (TranslateOccurred != null && (offsetX != 0 || offsetY != 0))
            {
                if (MovementOccurred != null)
                    MovementOccurred(this);

                TranslateOccurred(this, offsetX, offsetY);
            }
        }
		
        /// <summary>
        /// Scales the shape.
        /// </summary>
        /// <param name="scaleX">Scale x.</param>
        /// <param name="scaleY">Scale y.</param>
        /// <param name="point">Reference point.</param>
        override public void Scale(float scaleX, float scaleY, PointF point)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            if (!_shape.MovementContentBlocked)
                foreach (IShape shape in _shape.Shapes)
                    shape.Transformer.Scale(scaleX, scaleY, point);

            base.Scale(scaleX, scaleY, point);

            if (ScaleOccurred != null && (scaleX != 1 || scaleY != 1))
            {
                if (MovementOccurred != null)
                    MovementOccurred(this);

                ScaleOccurred(this, scaleX, scaleY, point);
            }
        }

        /// <summary>
        /// Rotates the shape.
        /// </summary>
        /// <param name="degree">Rotation degree.</param>
        /// <param name="point">Reference point.</param>
		override public void Rotate(float degree, PointF point)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            if (!_shape.MovementContentBlocked)
                foreach (IShape shape in _shape.Shapes)
                    shape.Transformer.Rotate(degree, point);

            base.Rotate(degree, point);

            if (RotateOccurred != null && degree != 0)
            {
                if (MovementOccurred != null)
                    MovementOccurred(this);

                RotateOccurred(this, degree, point);
            }
        }

        /// <summary>
        /// Deforms the shape.
        /// </summary>
        /// <param name="indexPoint">Index point to deform.</param>
        /// <param name="newPoint">New point.</param>
        override public void Deform(int indexPoint, PointF newPoint)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            int totalPoint = 0;
            int indexShape = 0;
            IShape shape = null;

            for (indexShape = 0; indexShape < _shape.Shapes.Count; indexShape++)
            {
                shape = _shape.Shapes[indexShape];
                if (indexPoint < shape.Geometric.PointCount + totalPoint)
                    break;

                totalPoint += shape.Geometric.PointCount;
            }

            if (shape == null || indexPoint == -1)
                return;

            shape.Transformer.Deform(indexPoint - totalPoint, newPoint);

            PointF memory = _shape.Geometric.PathPoints[indexPoint];
            base.Deform(indexPoint, newPoint);

            if (DeformOccurred != null  && memory != newPoint)
            {
                if (MovementOccurred != null)
                    MovementOccurred(this);

                DeformOccurred(this, indexPoint, newPoint);
            }
        }

        /// <summary>
        /// Flips the shape horizontally.
        /// </summary>
        /// <param name="x">Flip axis x.</param>
        override public void MirrorHorizontal(float x)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            foreach (IShape shape in _shape.Shapes)
                shape.Transformer.MirrorHorizontal(x);

            _shape.MovementContentBlocked = true;
            base.MirrorHorizontal(x);
            _shape.MovementContentBlocked = false;

            if (MirrorHorizontalOccurred != null)
            {
                if (MovementOccurred != null)
                    MovementOccurred(this);

                MirrorHorizontalOccurred(this, x);
            }
        }

        /// <summary>
        /// Flips the shape vertically.
        /// </summary>
        /// <param name="y">Flip axis y.</param>
        override public void MirrorVertical(float y)
        {
            if (_shape.Parent == null && (!_shape.Selected || _shape.Locked))
                return;

            foreach (IShape shape in _shape.Shapes)
                shape.Transformer.MirrorVertical(y);

            _shape.MovementContentBlocked = true;
            base.MirrorVertical(y);
            _shape.MovementContentBlocked = false;

            if (MirrorVerticalOccurred != null)
            {
                if (MovementOccurred != null)
                    MovementOccurred(this);

                MirrorVerticalOccurred(this, y);
            }
        }
        
        #endregion
    }
}

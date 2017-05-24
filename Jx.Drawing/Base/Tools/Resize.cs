using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Jx.Drawing.Common;

namespace Jx.Drawing.Base
{
    /// <summary>
    /// Resizes a shape.
    /// </summary>
    public class Resize : Select
    {
        HitPositions _hitPosition = HitPositions.None;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Resize()
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

            foreach (IShape shape in document.Shapes)
            {
                _hitPosition = shape.HitTest(e.Location);
                if (_hitPosition != HitPositions.None)
                {
                    this.Ghost.Shape = shape;
                   
                    UpdateSize(Ghost, shape.Location);

                    Ghost.MouseDown(document, e);
                    Ghost.Location = shape.Location;
                }
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

            if (_hitPosition == HitPositions.Center || _hitPosition == HitPositions.None)
                return;

            if (Select.LastSelectedShape == null)
                return;

            document.Shapes.Remove(Select.LastSelectedShape);
            Select.LastSelectedShape = Ghost.Shape.Clone() as IShape;
            document.Shapes.Add(Select.LastSelectedShape);
            Select.LastSelectedShape.Selected = true;
            Select.LastSelectedShape.Locked = false;
            Select.LastSelectedShape.Location = Ghost.Shape.Location;
            Select.LastSelectedShape.Dimension = Ghost.Shape.Dimension;

            Ghost.MouseUp(document, e);
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            if (!MousePressed)
            {
                base.MouseMove(document, e);
                return;
            }

            UpdateCursor(document, document.Shapes, e.Location);

            if (_hitPosition == HitPositions.Center || _hitPosition == HitPositions.None)
                return;

            UpdateLocation(Ghost, e.Location);
            UpdateSize(Ghost, e.Location);

            float left, right, top, bottom;

			if (Ghost.MirrorPoint.X > e.X)
			{
				left = e.X;
				right = Ghost.MirrorPoint.X;
			}
			else
			{
				left = Ghost.MirrorPoint.X;
				right = e.X;
			}
			if (Ghost.MirrorPoint.Y > e.Y)
			{
				top = e.Y;
				bottom = Ghost.MirrorPoint.Y;
			}
			else
			{
				top = Ghost.MirrorPoint.Y;
				bottom = e.Y;
			}

            SizeF size = new SizeF(right - left, bottom - top);
            PointF point = new PointF(left, top);

            size = AdjustSize(size, document.GridManager);
            point = AdjustLocation(point, document.GridManager);

            Ghost.Location = point;
            Ghost.Dimension = size;

            Ghost.MouseMove(document, e);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            if (Ghost != null)
                Ghost.Paint(document, e);
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
            bool updated = true;

            foreach (IShape shape in shapes)
            {
                if (_hitPosition != HitPositions.None && MousePressed)
                {
                    document.ActiveCursor = Cursors.Cross;
                    return true;
                }

                switch (shape.HitTest(point))
                {
                    case HitPositions.TopLeft:
                        document.ActiveCursor = Cursors.SizeNWSE;
                        return true;
                    case HitPositions.Top:
                        document.ActiveCursor = Cursors.SizeNS;
                        return true;
                    case HitPositions.TopRight:
                        document.ActiveCursor = Cursors.SizeNESW;
                        return true;
                    case HitPositions.Right:
                        document.ActiveCursor = Cursors.SizeWE;
                        return true;
                    case HitPositions.BottomRight:
                        document.ActiveCursor = Cursors.SizeNWSE;
                        return true;
                    case HitPositions.Bottom:
                        document.ActiveCursor = Cursors.SizeNS;
                        return true;
                    case HitPositions.BottomLeft:
                        document.ActiveCursor = Cursors.SizeNESW;
                        return true;
                    case HitPositions.Left:
                        document.ActiveCursor = Cursors.SizeWE;
                        return true;
                    case HitPositions.Center:
                    case HitPositions.None:
                    default:
                        document.ActiveCursor = Cursors.Default;
                        updated = false;
                        break;
                }
            }

            return updated;
        }

        #endregion

        #region Private Functions

        private PointF AdjustLocation(PointF point, GridManager gridManager)
        {
            //switch (_hitPosition)
            //{
            //    case HitPositions.TopLeft:
            //        point = Rounder.GetRoundedPoint(point, resolution);
            //        break;
            //    case HitPositions.Top:
            //        point = new PointF(Ghost.Location.X, Rounder.GetRoundedValue(point.Y, resolution.Height));
            //        break;
            //    case HitPositions.TopRight:
            //        point = new PointF(point.X, Rounder.GetRoundedValue(point.Y, resolution.Height));
            //        break;
            //    case HitPositions.Right:
            //        point = new PointF(point.X, Ghost.Location.Y);
            //        break;
            //    case HitPositions.BottomRight:
            //        break;
            //    case HitPositions.Bottom:
            //        point = new PointF(Ghost.Location.X, point.Y);
            //        break;
            //    case HitPositions.BottomLeft:
            //        break;
            //    case HitPositions.Left:
            //        point = new PointF(Rounder.GetRoundedValue(point.X, resolution.Width), Ghost.Location.Y);
            //        break;
            //}

//            return point;

            if (_hitPosition == HitPositions.Top || _hitPosition == HitPositions.Bottom)
            {
                point = new PointF(Ghost.Location.X, point.Y);
            }

            if (_hitPosition == HitPositions.Right || _hitPosition == HitPositions.Left)
            {
                point = new PointF(point.X, Ghost.Location.Y);
            }

            return gridManager.GetRoundedPoint(point);
        }

        private SizeF AdjustSize(SizeF size, GridManager gridManager)
        {
            //switch (_hitPosition)
            //{
            //    case HitPositions.TopLeft:
            //        break;
            //    case HitPositions.Top:
            //        size = new SizeF(Ghost.Dimension.Width, Rounder.GetRoundedValue(size.Height, resolution.Height));
            //        break;
            //    case HitPositions.TopRight:
            //        break;
            //    case HitPositions.Right:
            //        size = new SizeF(Rounder.GetRoundedValue(size.Width, resolution.Width), Ghost.Dimension.Height);
            //        break;
            //    case HitPositions.BottomRight:
            //        size = Rounder.GetRoundedSize(size, resolution);
            //        break;
            //    case HitPositions.Bottom:
            //        size = new SizeF(Ghost.Dimension.Width, Rounder.GetRoundedValue(size.Height, resolution.Height));
            //        break;
            //    case HitPositions.BottomLeft:
            //        size = new SizeF(size.Width, Rounder.GetRoundedValue(size.Height, resolution.Height));
            //        break;
            //    case HitPositions.Left:
            //        size = new SizeF(Rounder.GetRoundedValue(size.Width, resolution.Width), Ghost.Dimension.Height);
            //        break;
            //}

//            return size;

            if (_hitPosition == HitPositions.Top || _hitPosition == HitPositions.Bottom)
                size = new SizeF(Ghost.Dimension.Width, size.Height);

            if (_hitPosition == HitPositions.Right || _hitPosition == HitPositions.Left)
                size = new SizeF(size.Width, Ghost.Dimension.Height);

            return gridManager.GetRoundedSize(size);
        }

        private SizeF UpdateSize(IShape shape, PointF currentLocation)
        {
            float offsetSizeX = shape.Dimension.Width;
            float offsetSizeY = shape.Dimension.Height;

            switch (_hitPosition)
            {
                case HitPositions.TopLeft:
                {
                    if (currentLocation.X < shape.Location.X)
                        offsetSizeX = currentLocation.X - shape.Location.X;
                    else
                        offsetSizeX = -(shape.Location.X - currentLocation.X);

                    if (currentLocation.Y < shape.Location.Y)
                        offsetSizeY = currentLocation.Y - shape.Location.Y;
                    else
                        offsetSizeY = -(shape.Location.Y - currentLocation.Y);

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X + Ghost.Dimension.Width, Ghost.Location.Y + Ghost.Dimension.Height);

                    break;
                }
                case HitPositions.Top:
                {
                    offsetSizeX = 0;

                    if (currentLocation.Y < shape.Location.Y)
                        offsetSizeY = currentLocation.Y - shape.Location.Y;
                    else
                        offsetSizeY = -(shape.Location.Y - currentLocation.Y);

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X, Ghost.Location.Y + Ghost.Dimension.Height);

                    break;
                }
                case HitPositions.TopRight:
                {
                    if (currentLocation.X > shape.Location.X + shape.Dimension.Width)
                        offsetSizeX = -(currentLocation.X - shape.Location.X - shape.Dimension.Width);
                    else
                        offsetSizeX = shape.Location.X + shape.Dimension.Width - currentLocation.X;

                    if (currentLocation.Y < shape.Location.Y)
                        offsetSizeY = currentLocation.Y - shape.Location.Y;
                    else
                        offsetSizeY = -(shape.Location.Y - currentLocation.Y);

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X, Ghost.Location.Y + Ghost.Dimension.Height);

                    break;
                }
                case HitPositions.Right:
                {
                    if (currentLocation.X > shape.Location.X + shape.Dimension.Width)
                        offsetSizeX = -(currentLocation.X - shape.Location.X - shape.Dimension.Width);
                    else
                        offsetSizeX = shape.Location.X + shape.Dimension.Width - currentLocation.X;

                    offsetSizeY = 0;

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X, Ghost.Center.Y);

                    break;
                }
                case HitPositions.BottomRight:
                {
                    if (currentLocation.X > shape.Location.X + shape.Dimension.Width)
                        offsetSizeX = -(currentLocation.X - shape.Location.X - shape.Dimension.Width);
                    else
                        offsetSizeX = shape.Location.X + shape.Dimension.Width - currentLocation.X;

                    if (currentLocation.Y < shape.Location.Y - shape.Dimension.Height)
                        offsetSizeY = -(currentLocation.Y - shape.Location.Y - shape.Dimension.Height);
                    else
                        offsetSizeY = shape.Location.Y + shape.Dimension.Height - currentLocation.Y;

                    Ghost.MirrorPoint = currentLocation;

                    break;
                }
                case HitPositions.Bottom:
                {
                    offsetSizeX = 0;

                    if (currentLocation.Y < shape.Location.Y - shape.Dimension.Height)
                        offsetSizeY = -(currentLocation.Y - shape.Location.Y - shape.Dimension.Height);
                    else
                        offsetSizeY = shape.Location.Y + shape.Dimension.Height - currentLocation.Y;

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X, Ghost.Location.Y);

                    break;
                }
                case HitPositions.BottomLeft:
                {
                    if (currentLocation.X < shape.Location.X)
                        offsetSizeX = currentLocation.X - shape.Location.X;
                    else
                        offsetSizeX = -(shape.Location.X - currentLocation.X);

                    if (currentLocation.Y < shape.Location.Y - shape.Dimension.Height)
                        offsetSizeY = -(currentLocation.Y - shape.Location.Y - shape.Dimension.Height);
                    else
                        offsetSizeY = shape.Location.Y + shape.Dimension.Height - currentLocation.Y;

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X + Ghost.Dimension.Width, Ghost.Location.Y);

                    break;
                }
                case HitPositions.Left:
                {
                    if (currentLocation.X < shape.Location.X)
                        offsetSizeX = currentLocation.X - shape.Location.X;
                    else
                        offsetSizeX = -(shape.Location.X - currentLocation.X);

                    offsetSizeY = 0;

                    Ghost.MirrorPoint = new PointF(Ghost.Location.X + Ghost.Dimension.Width, Ghost.Center.Y);

                    break;
                }
            }

            return new SizeF(shape.Dimension.Width - offsetSizeX, shape.Dimension.Height - offsetSizeY);
        }

        private PointF UpdateLocation(IShape shape, PointF currentLocation)
        {
            PointF newLocation = currentLocation;

            switch (_hitPosition)
            {
                case HitPositions.TopLeft:
                {
                    newLocation = currentLocation;
                    break;
                }
                case HitPositions.Top:
                {
                    newLocation.X = shape.Location.X;
                    newLocation.Y = currentLocation.Y;
                    break;
                }
                case HitPositions.TopRight:
                {
                    newLocation.X = shape.Location.X;
                    newLocation.Y = currentLocation.Y;
                    break;
                }
                case HitPositions.Right:
                {
                    newLocation.X = shape.Location.X;
                    newLocation.Y = shape.Location.Y;
                    break;
                }
                case HitPositions.BottomRight:
                {
                    newLocation.X = shape.Location.X;
                    newLocation.Y = shape.Location.Y;
                    break;
                }
                case HitPositions.Bottom:
                {
                    newLocation.X = shape.Location.X;
                    newLocation.Y = shape.Location.Y;
                    break;
                }
                case HitPositions.BottomLeft:
                {
                    newLocation.X = currentLocation.X;
                    newLocation.Y = shape.Location.Y;
                    break;
                }
                case HitPositions.Left:
                {
                    newLocation.X = currentLocation.X;
                    newLocation.Y = shape.Location.Y;
                    break;
                }
            }

            return newLocation;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Manages grid functionalities.
    /// </summary>
    public class GridManager
    {
        #region Events and Delegates

        /// <summary>
        /// Resolution is changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="oldValue">Resolution before changing.</param>
        /// <param name="newValue">Resolution after changing.</param>
        public delegate void ResolutionChangedHandler(GridManager sender, SizeF oldValue, SizeF newValue);

        /// <summary>
        /// Fires when resolution is changed.
        /// </summary>
        virtual public event ResolutionChangedHandler ResolutionChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GridManager()
        {
        }

        #endregion

        #region Properties

        SizeF _resolution = new SizeF(10, 10);
        /// <summary>
        /// Gets or sets the grid panel.
        /// </summary>
        public SizeF Resolution
        {
            get { return _resolution; }

            set
            {
                SizeF oldValue = _resolution;

                _resolution = value;

                if (ResolutionChanged != null && (oldValue.Width != _resolution.Width || oldValue.Height != _resolution.Height))
                    ResolutionChanged(this, oldValue, _resolution);
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Updates shapes location and size relative to grid resolution.
        /// </summary>
        /// <param name="shapes">Shapes to update.</param>
        public void SnapToGrid(ShapeCollection shapes)
        {
            foreach (IShape shape in shapes)
            {
                bool selected = shape.Selected;
                bool locked = shape.Locked;

                shape.Selected = true;
                shape.Locked = false;

                shape.Location = GetRoundedPoint(shape.Location);
                shape.Dimension = GetRoundedSize(shape.Dimension);

                shape.Selected = selected;
                shape.Locked = locked;
            }
        }

        /// <summary>
        /// Gets the rounded value relative to grid resolution.
        /// </summary>
        /// <param name="value">Value to round.</param>
        /// <param name="resolution">Reference resolution.</param>
        /// <returns>Rounded value.</returns>
        public float GetRoundedValue(float value, float resolution)
        {
            float roundedValue = (float)Math.Round(value + resolution/2);

            if (resolution != 0)
                roundedValue = roundedValue - roundedValue%resolution;

            return roundedValue;
        }

        /// <summary>
        /// Gets the rounded point relative to grid resolution.
        /// </summary>
        /// <param name="point">Point to round.</param>
        /// <returns>Rounded point.</returns>
        public PointF GetRoundedPoint(PointF point)
        {
            PointF roundedPoint = new PointF((float)Math.Round(point.X + _resolution.Width/2), (float)Math.Round(point.Y + _resolution.Height/2));

            if (_resolution.Height != 0)
                roundedPoint = new PointF(roundedPoint.X, roundedPoint.Y - roundedPoint.Y%_resolution.Height);

            if (_resolution.Width != 0)
                roundedPoint = new PointF(roundedPoint.X - roundedPoint.X%_resolution.Width, roundedPoint.Y);

            return roundedPoint;
        }

        /// <summary>
        /// Gets the rounded size relative to grid resolution.
        /// </summary>
        /// <param name="size">Size to round.</param>
        /// <returns>Rounded size.</returns>
        public SizeF GetRoundedSize(SizeF size)
        {
            SizeF roundedSize = new SizeF((float)Math.Round(size.Width + _resolution.Width/2), (float)Math.Round(size.Height + _resolution.Height/2));

            if (_resolution.Height != 0)
                roundedSize = new SizeF(roundedSize.Width, roundedSize.Height - roundedSize.Height%_resolution.Height);

            if (_resolution.Width != 0)
                roundedSize = new SizeF(roundedSize.Width - roundedSize.Width%_resolution.Width, roundedSize.Height);

            return roundedSize;
        }

        #endregion
    }
}

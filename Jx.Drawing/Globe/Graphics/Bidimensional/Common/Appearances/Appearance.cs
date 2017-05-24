using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

using Jx.Xml.Serialization;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Mananges the appearance of any shape.
    /// </summary>
    [XmlClassSerializable("appearance")]
    public abstract class Appearance : ICloneable
    {
        #region Added properties to serialize

        [XmlFieldSerializable("penWidth")]
        float PenWidth
        {
            get { return _activePen.Width; }
            set { _activePen.Width = value; }

        }

        [XmlFieldSerializable("PenColorString")]
        string PenColorString
        {
            get { return Jx.Core.Converters.ColorConverter.StringFromColor(_activePen.Color, ';'); }
            set { _activePen.Color = Jx.Core.Converters.ColorConverter.ColorFromString(value, ';'); }
        }

        [XmlFieldSerializable("borderColorString")]
        string BorderColorString
        {
            get { return Jx.Core.Converters.ColorConverter.StringFromColor(_borderColor, ';'); }
            set { _borderColor = Jx.Core.Converters.ColorConverter.ColorFromString(value, ';'); }
        }

        [XmlFieldSerializable("markerColorString")]
        string MarkerColorString
        {
            get { return Jx.Core.Converters.ColorConverter.StringFromColor(_markerColor, ';'); }
            set { _markerColor = Jx.Core.Converters.ColorConverter.ColorFromString(value, ';'); }
        }

        #endregion

        #region Events and Delegates

        /// <summary>
        /// Fires when appearance is changed.
        /// </summary>
        virtual public event AppearanceHandler AppearanceChanged;

        /// <summary>
        /// Fires when marker dimension is changed.
        /// </summary>
        virtual public event MarkerDimensionHandler MarkerDimensionChanged;

        /// <summary>
        /// Fires when marker color is changed.
        /// </summary>
        virtual public event MarkerColorHandler MarkerColorChanged;

        /// <summary>
        /// Fires when border color is changed.
        /// </summary>
        virtual public event BorderColorHandler BorderColorChanged;

        /// <summary>
        /// Fires when border width is changed.
        /// </summary>
        virtual public event BorderWidthHandler BorderWidthChanged;

        /// <summary>
        /// Fires when grabber dimension is changed.
        /// </summary>
        virtual public event GrabberDimensionHandler GrabberDimensionChanged;

        /// <summary>
        /// Fires when active pen is changed.
        /// </summary>
        virtual public event ActivePenHandler ActivePenChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Appearance()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="appearance">Appearance to copy.</param>
        public Appearance(Appearance appearance)
        {
            _markerDimension = appearance._markerDimension;
            _markerColor = appearance._markerColor;
            _borderColor = appearance._borderColor;
            _borderWidth = appearance._borderWidth;
            _grabberDimension = appearance._grabberDimension;
            _activePen = appearance._activePen.Clone() as Pen;
        }

        #endregion

        #region Properties

        IShape _shape = null;
        /// <summary>
        /// Gets or sets the _shape to visualize.
        /// </summary>
        [Browsable(false)]
        public IShape Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        int _markerDimension = 4;
        /// <summary>
        /// Gets or sets the marker dimension.
        /// </summary>
        [XmlFieldSerializable("markerDimension")]
        public int MarkerDimension
        {
            get { return _markerDimension; }
            
            set
            {
                int memory = _markerDimension;
                _markerDimension = value;
                if (_markerDimension < 4)
                    _markerDimension = 4;

                if (MarkerDimensionChanged != null && memory != _markerDimension)
                    MarkerDimensionChanged(this, memory, _markerDimension);

                if (AppearanceChanged != null && memory != _markerDimension)
                    AppearanceChanged(this);
            }
        }

        Color _markerColor = Color.Black;
        /// <summary>
        /// Gets or sets the marker color.
        /// </summary>
        public Color MarkerColor
        {
            get { return _markerColor; }
            set
            {
                Color memory = _markerColor;
                _markerColor = value;

                if (MarkerColorChanged != null && memory != _markerColor)
                    MarkerColorChanged(this, memory, _markerColor);

                if (AppearanceChanged != null && memory != _markerColor)
                    AppearanceChanged(this);
            }
        }

        Pen _activePen = new Pen(Color.Black);
        /// <summary>
        /// Gets or sets the active drawing pen.
        /// </summary>
        [Browsable(false)]
        public Pen ActivePen
        {
            get { return _activePen; }
            set
            {
                if (_activePen == null)
                    throw new ApplicationException();

                if (ActivePenChanged != null && _activePen != value)
                    ActivePenChanged(this, _activePen, value);

                if (AppearanceChanged != null && _activePen != value)
                    AppearanceChanged(this);

                _activePen = value;
            }
        }

        Color _borderColor = Color.Black;
        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                Color memory = _borderColor;
                _borderColor = value;

                if (_activePen != null)
                    _activePen.Color = value;

                if (BorderColorChanged != null && memory != _borderColor)
                    BorderColorChanged(this, memory, _borderColor);

                if (AppearanceChanged != null && memory != _borderColor)
                    AppearanceChanged(this);
            }
        }

        float _borderWidth = 1f;
        /// <summary>
        /// Gets or sets the border width.
        /// </summary>
        [XmlFieldSerializable("borderWidth")]
        public float BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                float memory = _borderWidth;
                _borderWidth = value;

                if (_activePen != null)
                    _activePen.Width = value;

                if (BorderWidthChanged != null && memory != _borderWidth)
                    BorderWidthChanged(this, memory, _borderWidth);

                if (AppearanceChanged != null && memory != _borderWidth)
                    AppearanceChanged(this);
            }
        }

        int _grabberDimension = 6;
        /// <summary>
        /// Gets or sets the grabber dimension into selected state.
        /// </summary>
        [XmlFieldSerializable("grabberDimension")]
        public int GrabberDimension
        {
            get { return _grabberDimension; }
            set
            {
                int memory = _grabberDimension;
                _grabberDimension = value;
                if (_grabberDimension < 3)
                    _grabberDimension = 3;

                if (GrabberDimensionChanged != null && memory != _grabberDimension)
                    GrabberDimensionChanged(this, memory, _grabberDimension);

                if (AppearanceChanged != null && memory != _grabberDimension)
                    AppearanceChanged(this);
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Clones the Appearance.
        /// </summary>
        /// <returns>New Appearance.</returns>
        abstract public object Clone();

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        virtual public void Paint(IDocument document, PaintEventArgs e)
        {
            if (!_shape.Visible || !IsValidGeometric(_shape.Geometric))
                return;

            float width = _shape.Dimension.Width;
            float height = _shape.Dimension.Height;

            if (width == 1)
                e.Graphics.DrawLine(_activePen, Point.Round(new PointF(_shape.Location.X, _shape.Location.Y)), Point.Round(new PointF(_shape.Location.X, _shape.Location.Y + _shape.Dimension.Height)));
            else if (height == 1)
                e.Graphics.DrawLine(_activePen, Point.Round(new PointF(_shape.Location.X, _shape.Location.Y)), Point.Round(new PointF(_shape.Location.X + _shape.Dimension.Width, _shape.Location.Y)));
            else
                e.Graphics.DrawPath(_activePen, _shape.Geometric);

            DrawSelection(document, e);
            DrawMarkers(document, e);
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Draws the frame selection if Selected property is true.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        virtual protected void DrawSelection(IDocument document, PaintEventArgs e)
        {
            if (!_shape.Selected || !IsValidGeometric(_shape.Geometric))
                return;

            Rectangle outside = Rectangle.Round(_shape.Geometric.GetBounds());
            Rectangle inside = outside;

            outside.Inflate(_grabberDimension / 2, _grabberDimension / 2);
            inside.Inflate(-_grabberDimension / 2, -_grabberDimension / 2);

            ControlPaint.DrawSelectionFrame(e.Graphics, true, outside, inside, document.DrawingControl.BackColor);

            Color color = Color.Black;
            if (Jx.Graphics.Bidimensional.Common.Select.LastSelectedShape == _shape)
                color = ControlPaint.LightLight(color);

            using (SolidBrush solidBrush = new SolidBrush(color))
            {
                Rectangle[] grabbers = _shape.GetGrabbers();

                foreach (Rectangle grabber in grabbers)
                    e.Graphics.FillRectangle(solidBrush, grabber);
            }
        }

        /// <summary>
        /// Draws markers of geometric.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        virtual protected void DrawMarkers(IDocument document, PaintEventArgs e)
        {
            if (!_shape.Marked || !IsValidGeometric(_shape.Geometric))
                return;

            foreach (PointF point in _shape.Geometric.PathPoints)
            {
                RectangleF rect = new RectangleF(point.X - MarkerDimension/2, point.Y - MarkerDimension/2, MarkerDimension, MarkerDimension);
                using (Brush brush = new SolidBrush(MarkerColor))
                {
                    e.Graphics.FillRectangle(brush, Rectangle.Round(rect));
                }
            }
        }

        /// <summary>
        /// Checks if the geometric is valid (width and height not null).
        /// </summary>
        /// <param name="geometric">Path to check.</param>
        /// <returns></returns>
        protected bool IsValidGeometric(GraphicsPath geometric)
        {
            if (geometric.GetBounds().Size.Width == 0 || geometric.GetBounds().Size.Height == 0)
                return false;

            return true;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Globe.Xml.Serialization;

namespace Globe.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Abstract shape class.
    /// </summary>
    [XmlClassSerializable("shape")]
    public abstract class Shape : IShape
    {
        #region Added properties to serialize

        PointF[] _geometricPoints = null;
        [XmlFieldSerializable("geometricPoints")]
        PointF[] GeometricPoints
        {
            get { return _geometric.PathPoints; }

            set
            {
                _geometricPoints = value;

                if (_geometricPoints != null && _geometricTypes != null)
                    _geometric = new GraphicsPath(_geometricPoints, _geometricTypes);
            }
        }

        Byte[] _geometricTypes = null;
        [XmlFieldSerializable("geometricTypes")]
        Byte[] GeometricTypes
        {
            get { return _geometric.PathTypes; }

            set
            {
                _geometricTypes = value;

                if (_geometricPoints != null && _geometricTypes != null)
                    _geometric = new GraphicsPath(_geometricPoints, _geometricTypes);
            }
        }

        [XmlFieldSerializable("locationX")]
        float LocationX
        {
            get { return Location.X; }
            set { Location = new PointF(value, Location.Y); }
        }

        [XmlFieldSerializable("locationY")]
        float LocationY
        {
            get { return Location.Y; }
            set { Location = new PointF(Location.X, value); }
        }

        [XmlFieldSerializable("width")]
        float Width
        {
            get { return Dimension.Width; }
            set { Dimension = new SizeF(value, Dimension.Height); }
        }

        [XmlFieldSerializable("height")]
        float Height
        {
            get { return Dimension.Height; }
            set { Dimension = new SizeF(Dimension.Width, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Shape()
        {
            this._transformer = new Transformer(this);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="shape">Shape to copy.</param>
        protected Shape(Shape shape)
        {
            this._geometric = shape.Geometric.Clone() as GraphicsPath;            
            this._transformer = new Transformer(this);
            this._appearance = shape.Appearance.Clone() as Appearance;

            this._visible = shape.Visible;
            this._locked = shape.Locked;
            this._selected = shape.Selected;

            this._marked = shape.Marked;
            this._menu = shape.Menu;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="geometric">Reference GraphicsPath.</param>
        protected Shape(GraphicsPath geometric)
        {
            this._transformer = new Transformer(this);
            this._geometric = geometric.Clone() as GraphicsPath;
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the shape.
        /// </summary>
        /// <returns>New shape.</returns>
        public abstract object Clone();

        #endregion

        #region IActions Interface

        /// <summary>
        /// Mouse down function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual public void MouseDown(IDocument document, MouseEventArgs e)
        {
            if (ShapeMouseDown != null)
                ShapeMouseDown(this, document, e);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual public void MouseUp(IDocument document, MouseEventArgs e)
        {
            if (ShapeMouseUp != null)
                ShapeMouseUp(this, document, e);
        }

        /// <summary>
        /// Mouse click function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual public void MouseClick(IDocument document, MouseEventArgs e)
        {
            if (ShapeMouseClick != null)
                ShapeMouseClick(this, document, e);
        }

        /// <summary>
        /// Mouse double click function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual public void MouseDoubleClick(IDocument document, MouseEventArgs e)
        {
            if (ShapeMouseDoubleClick != null)
                ShapeMouseDoubleClick(this, document, e);
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual public void MouseMove(IDocument document, MouseEventArgs e)
        {
            if (ShapeMouseMove != null)
                ShapeMouseMove(this, document, e);
        }

        /// <summary>
        /// Mouse wheel function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        virtual public void MouseWheel(IDocument document, MouseEventArgs e)
        {
            if (ShapeMouseWheel != null)
                ShapeMouseWheel(this, document, e);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        virtual public void Paint(IDocument document, PaintEventArgs e)
        {
            // This is bad, but for serialization is necessary set the shape
            _appearance.Shape = this;
            
            _appearance.Paint(document, e);

            if (ShapePaint != null)
                ShapePaint(this, document, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when change a property. Used to Undo-Redo mechanism.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.ShapeChangingHandler ShapeChanged;

        /// <summary>
        /// Fires when mouse down on shape.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.MouseDownOnShape ShapeMouseDown;

        /// <summary>
        /// Fires when mouse up on shape.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.MouseUpOnShape ShapeMouseUp;

        /// <summary>
        /// Fires when mouse click on shape.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.MouseClickOnShape ShapeMouseClick;

        /// <summary>
        /// Fires when mouse double click on shape.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.MouseDoubleClickOnShape ShapeMouseDoubleClick;

        /// <summary>
        /// Fires when mouse move on shape.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.MouseMoveOnShape ShapeMouseMove;

        /// <summary>
        /// Fires when mouse wheel on shape.
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.MouseWheel ShapeMouseWheel;

        /// <summary>
        /// Fires when paint shape
        /// </summary>
        virtual public event Globe.Graphics.Bidimensional.Common.PaintOnShape ShapePaint;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        [System.ComponentModel.TypeConverter(typeof(PointFTypeConverter))]
        public PointF Location
        {
            get { return _geometric.GetBounds().Location; }

            set
            {
                if (value.IsEmpty)
                    return;

                if (float.IsNaN(value.X) || float.IsNaN(value.Y) ||
                    float.IsInfinity(value.Y) || float.IsInfinity(value.Y))
                    return;

                float offsetX = value.X - this.Location.X;
                float offsetY = value.Y - this.Location.Y;
                _transformer.Translate(offsetX, offsetY);
            }
        }

        /// <summary>
        /// Gets or sets the current size.
        /// </summary>
        [System.ComponentModel.TypeConverter(typeof(SizeFTypeConverter))]
		public SizeF Dimension
        {
            get { return _geometric.GetBounds().Size; }

            set
            {
                if (value.IsEmpty)
                    return;

                // Never this.Dimension.Width and this.Dimension.Height are zero
                float scaleX = value.Width / this.Dimension.Width;
                float scaleY = value.Height / this.Dimension.Height;

                _transformer.Scale(scaleX, scaleY);
            }
        }

        /// <summary>
        /// Gets or sets the current center.
        /// </summary>
        [System.ComponentModel.TypeConverter(typeof(PointFTypeConverter))]
		public PointF Center
        {
            get
            {
                float x = this.Location.X + this.Dimension.Width / 2f;
                float y = this.Location.Y + this.Dimension.Height / 2f;

                return new PointF(x, y);
            }

            set
            {
                float offsetX = value.X - this.Location.X - this.Dimension.Width / 2f;
                float offsetY = value.Y - this.Location.Y - this.Dimension.Height / 2f;;

                _transformer.Translate(offsetX, offsetY);
            }
        }

        internal float _rotation = 0f;
        /// <summary>
        /// Gets or sets the current rotation.
        /// </summary>
        [XmlFieldSerializable("rotation")]
		public float Rotation
        {
            get { return _rotation; }
            
            set
            {
                _rotation += value;
                _transformer.Rotate(value);
            }
        }

        bool _visible = true;
        /// <summary>
        /// Get or sets the visibility.
        /// </summary>
        [XmlFieldSerializable("visible")]
		virtual public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        bool _locked = false;
        /// <summary>
        /// Gets or sets the locking during movements.
        /// </summary>
        virtual public bool Locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        bool _selected = false;
        /// <summary>
        /// Gets oe sets the selecting.
        /// </summary>
		virtual public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        GraphicsPath _geometric = new GraphicsPath();
        /// <summary>
        /// Gets the geometric form of the shape.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public GraphicsPath Geometric
        {
            get { return _geometric; }
        }

        Transformer _transformer = null;
        /// <summary>
        /// Gets or sets the transformer.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Transformer Transformer
        {
            get { return _transformer; }
            set { _transformer = value; }
        }

        Globe.Graphics.Bidimensional.Common.Appearance _appearance = new PolygonAppearance();
        /// <summary>
        /// Gets or sets the appearance.
        /// </summary>
        [XmlFieldSerializable("Appearance"), System.ComponentModel.TypeConverter(typeof(AppearanceTypeConverter))]
        public Globe.Graphics.Bidimensional.Common.Appearance Appearance
        {
            get { return _appearance; }
            set { _appearance = value; }
        }

        bool _marked = false;
        /// <summary>
        /// Gets or sets the makers visibility.
        /// </summary>
        [XmlFieldSerializable("marked")]
        public bool Marked
        {
            get { return _marked; }
            set
            {
                bool memory = _marked;
                _marked = value;

                if (ShapeChanged != null && memory != _marked)
                    ShapeChanged(this, _marked);
            }
        }

        IShape _parent = null;
        /// <summary>
        /// Gets the parent shape, if this is a composite shape.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public IShape Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        ContextMenuStrip _menu = null;
        /// <summary>
        /// Gets or sets the context menu.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public ContextMenuStrip Menu
        {
            get { return _menu; }
            set
            {
                _menu = value;

                if (ShapeChanged != null)
                    ShapeChanged(this, _menu);
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Gets the shape hit position.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns>Hit position.</returns>
		virtual public HitPositions HitTest(Point point)
        {
            HitPositions hitPosition = HitPositions.None;

            Rectangle[] grabbers = GetGrabbers();
         
            if (grabbers[0].Contains(point))
            {
                hitPosition = HitPositions.TopLeft;
            }
            else if (grabbers[1].Contains(point))
            {
                hitPosition = HitPositions.Top;
            }
            else if (grabbers[2].Contains(point))
            {
                hitPosition = HitPositions.TopRight;
            }
            else if (grabbers[3].Contains(point))
            {
                hitPosition = HitPositions.Right;
            }
            else if (grabbers[4].Contains(point))
            {
                hitPosition = HitPositions.BottomRight;
            }
            else if (grabbers[5].Contains(point))
            {
                hitPosition = HitPositions.Bottom;
            }
            else if (grabbers[6].Contains(point))
            {
                hitPosition = HitPositions.BottomLeft;
            }
            else if (grabbers[7].Contains(point))
            {
                hitPosition = HitPositions.Left;
            }
            else if (Contains(point))
            {
                hitPosition = HitPositions.Center;
            }

            return hitPosition;
        }

        /// <summary>
        /// Controls if the point is into this.
        /// </summary>
        /// <param name="point">Point to control.</param>
        /// <returns></returns>
		public bool Contains(Point point)
        {
            return _geometric.GetBounds().Contains(point);
        }

        /// <summary>
        /// Controls if the shape is into this.
        /// </summary>
        /// <param name="shape">Shape to control.</param>
        /// <returns></returns>
		public bool Contains(IShape shape)
        {
            return _geometric.GetBounds().Contains(shape.Geometric.GetBounds());
        }

        /// <summary>
        /// Gets markers of geometric form.
        /// </summary>
        /// <returns>Markers.</returns>
        public RectangleF[] GetMarkers()
        {
            if (_geometric.PointCount == 0)
                return null;

            RectangleF[] rects = new RectangleF[_geometric.PointCount];

            for (int i = 0; i < _geometric.PointCount; i++)
                rects[i] = new RectangleF(_geometric.PathPoints[i].X - _appearance.MarkerDimension/2, _geometric.PathPoints[i].Y - _appearance.MarkerDimension/2, _appearance.MarkerDimension, _appearance.MarkerDimension);

            return rects;
        }

        /// <summary>
        /// Gets grabbers (to resize for example).
        /// </summary>
        /// <returns>Grabbers.</returns>
        virtual public System.Drawing.Rectangle[] GetGrabbers()
        {
            Rectangle rect = Rectangle.Round(_geometric.GetBounds());
            Rectangle[] grabbers = new Rectangle[8];

            // Top Left
            grabbers[0].X = rect.X - _appearance.GrabberDimension / 2;
            grabbers[0].Y = rect.Y - _appearance.GrabberDimension / 2;
            //grabbers[0].X = rect.X;
            //grabbers[0].Y = rect.Y;
            grabbers[0].Width = _appearance.GrabberDimension;
            grabbers[0].Height = _appearance.GrabberDimension;

            // Top
            grabbers[1].X = rect.X + rect.Width / 2 - _appearance.GrabberDimension / 2;
            grabbers[1].Y = rect.Y - _appearance.GrabberDimension / 2;
            grabbers[1].Width = _appearance.GrabberDimension;
            grabbers[1].Height = _appearance.GrabberDimension;

            // Top Right
            grabbers[2].X = rect.X + rect.Width - _appearance.GrabberDimension / 2;
            grabbers[2].Y = rect.Y - _appearance.GrabberDimension / 2;
            //grabbers[2].X = rect.X + rect.Width - _grabberDimension;
            //grabbers[2].Y = rect.Y;
            grabbers[2].Width = _appearance.GrabberDimension;
            grabbers[2].Height = _appearance.GrabberDimension;

            // Right
            grabbers[3].X = rect.X + rect.Width - _appearance.GrabberDimension / 2;
//            grabbers[3].X = rect.X + rect.Width - _grabberDimension;
            grabbers[3].Y = rect.Y + rect.Height / 2 - _appearance.GrabberDimension / 2;
            grabbers[3].Width = _appearance.GrabberDimension;
            grabbers[3].Height = _appearance.GrabberDimension;

            // Bottom Right
            grabbers[4].X = rect.X + rect.Width - _appearance.GrabberDimension / 2;
            grabbers[4].Y = rect.Y + rect.Height - _appearance.GrabberDimension / 2;
            //grabbers[4].X = rect.X + rect.Width - _grabberDimension;
            //grabbers[4].Y = rect.Y + rect.Height - _grabberDimension;
            grabbers[4].Width = _appearance.GrabberDimension;
            grabbers[4].Height = _appearance.GrabberDimension;

            // Bottom
            grabbers[5].X = rect.X + rect.Width / 2 - _appearance.GrabberDimension / 2;
            grabbers[5].Y = rect.Y + rect.Height - _appearance.GrabberDimension / 2;
//            grabbers[5].Y = rect.Y + rect.Height - _grabberDimension;
            grabbers[5].Width = _appearance.GrabberDimension;
            grabbers[5].Height = _appearance.GrabberDimension;

            // Bottom Left
            grabbers[6].X = rect.X - _appearance.GrabberDimension / 2;
            grabbers[6].Y = rect.Y + rect.Height - _appearance.GrabberDimension / 2;
            //grabbers[6].X = rect.X;
            //grabbers[6].Y = rect.Y + rect.Height - _grabberDimension;
            grabbers[6].Width = _appearance.GrabberDimension;
            grabbers[6].Height = _appearance.GrabberDimension;

            // Left
            grabbers[7].X = rect.X - _appearance.GrabberDimension / 2;
//            grabbers[7].X = rect.X;
            grabbers[7].Y = rect.Y + rect.Height / 2 - _appearance.GrabberDimension / 2;
            grabbers[7].Width = _appearance.GrabberDimension;
            grabbers[7].Height = _appearance.GrabberDimension;

            return grabbers;
        }

        /// <summary>
        /// Returns the marker index point.
        /// </summary>
        /// <param name="point">Point to control.</param>
        /// <returns>Index point.</returns>
        public int GetMarkerIndex(PointF point)
        {
            RectangleF[] markers = GetMarkers();
            if (markers == null)
                return - 1;

            for (int i = 0; i < markers.Length; i++)
            {
                if (markers[i].Contains(point))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the center point of the clicked grabber.
        /// </summary>
        /// <param name="hitPosition">hit position.</param>
        /// <returns>Point.</returns>
        virtual public PointF GetGrabberPoint(HitPositions hitPosition)
        {
            if (hitPosition == HitPositions.Right ||
                hitPosition == HitPositions.BottomRight ||
                hitPosition == HitPositions.Bottom)
                return new PointF(this.Location.X, this.Location.Y);
            else if (
                hitPosition == HitPositions.BottomLeft ||
                hitPosition == HitPositions.Left)
                return new PointF(this.Location.X + this.Dimension.Width, this.Location.Y);
            else if (
                hitPosition == HitPositions.TopLeft ||
                hitPosition == HitPositions.Top ||
                hitPosition == HitPositions.Left)
                return new PointF(this.Location.X + this.Dimension.Width, this.Location.Y + this.Dimension.Height);
            else if (hitPosition == HitPositions.TopRight)
                return new PointF(this.Location.X, this.Location.Y + this.Dimension.Height);

            return new PointF(0, 0);
        }

        #endregion

        #endregion
    }
}

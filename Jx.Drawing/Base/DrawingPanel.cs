using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Jx.Drawing.Serialization.XML;
using Jx.Drawing.Utilities;

using Jx.Drawing.Common;

namespace Jx.Drawing.Base
{
    /// <summary>
    /// Panel in which drawing shapes.
    /// </summary>
    public class DrawingPanel : Panel, IDocument
    {
        bool _zoomCenter = true;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DrawingPanel()
        {
            SetStyle(
                ControlStyles.Selectable |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint, true);

            _shapes.ShapeChanged += new Jx.Drawing.Common.ShapeChangingHandler(_shapes_ShapeChanged);
            _shapes.ShapeMovementOccurred += new MovementHandler(_shapes_ShapeMovementOccurred);
            _shapes.ShapeAppearanceChanged += new AppearanceHandler(_shapes_ShapeAppearanceChanged);

            _gridManager.ResolutionChanged +=  new GridManager.ResolutionChangedHandler(_gridManager_ResolutionChanged);

            this.Size = new Size(1000, 1000);
        }

        #endregion

        #region Properties

        Tool _tool = new Pointer();
        /// <summary>
        /// Gets or sets current tool.
        /// </summary>
        [Browsable(false)]
        public Tool ActiveTool
        {
            get { return _tool; }
            set
            {
                if (value == null)
                    throw new ApplicationException();

                _tool = value;
            }
        }

        float _zoom = 1;
        /// <summary>
        /// Gets or sets zoom percent.
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            
            set
            {
                if (value <= 0)
                    throw new ApplicationException();

                _zoom = value;

                PointF center = PointF.Empty;

                if (!_zoomCenter)
                    center = this.PointToClient(System.Windows.Forms.Control.MousePosition);
                else
                    center = new PointF((this.Location.X + this.Size.Width) / 2, (this.Location.Y + this.Size.Height) / 2);
//                this.Size = new Size((int)_zoom * this.Size.Width, (int)_zoom * this.Size.Height);

                foreach (IShape shape in _shapes)
                {
                    bool selected = shape.Selected;
                    bool locked = shape.Locked;

                    shape.Selected = true;
                    shape.Locked = false;

                    shape.Transformer.Scale(_zoom, _zoom, center);

                    shape.Selected = selected;
                    shape.Locked = locked;
                }

                Invalidate();
            }
        }

        bool _enableWheelZoom = true;
        /// <summary>
        /// Gets or sets if the wheel changes the zoom factor.
        /// </summary>
        public bool EnableWheelZoom
        {
            get { return _enableWheelZoom; }
            set { _enableWheelZoom = value; }
        }

        #endregion

        #region IDocument Interface

        /// <summary>
        /// Gets the DrawingControl (this).
        /// </summary>
        [Browsable(false)]
        public Control DrawingControl
        {
            get { return this; }
        }

        ShapeCollection _shapes = new ShapeCollection();
        /// <summary>
        /// Gets or sets the shapes to draw.
        /// </summary>
        [Browsable(false)]
        public ShapeCollection Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }

        /// <summary>
        /// Gets or sets the active cursor.
        /// </summary>
        [Browsable(false)]
        public Cursor ActiveCursor
        {
            get { return this.Cursor; }
            set { this.Cursor = value; }
        }

        GridManager _gridManager = new GridManager();
        /// <summary>
        /// Gets (set protected) GridManager to handle grid properties.
        /// </summary>
        [System.ComponentModel.TypeConverter(typeof(GridManagerTypeConverter))]
        public GridManager GridManager
        {
            get { return _gridManager; }
            protected set { _gridManager = value; }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Undo action.
        /// </summary>
        virtual public void Undo()
        {
            if (History<ShapeCollection>.IsAtStart())
                return;

            _shapes.Clear();
            _shapes.AddRange(History<ShapeCollection>.Undo());
            Invalidate();
        }

        /// <summary>
        /// Redo action.
        /// </summary>
        virtual public void Redo()
        {
            if (History<ShapeCollection>.IsAtEnd())
                return;

            _shapes.Clear();
            _shapes.AddRange(History<ShapeCollection>.Redo());
            Invalidate();
        }

        /// <summary>
        /// Cut action.
        /// </summary>
        virtual public void Cut()
        {
            History<ShapeCollection>.Memorize(_shapes);
            Clipboard<ShapeCollection>.Clip = Jx.Drawing.Common.Select.GetSelectedShapes(_shapes);
            
            Delete();
            Invalidate();
        }

        /// <summary>
        /// Copy action.
        /// </summary>
        virtual public void Copy()
        {
            Clipboard<ShapeCollection>.Clip = Jx.Drawing.Common.Select.GetSelectedShapes(_shapes);
        }

        /// <summary>
        /// Paste action.
        /// </summary>
        virtual public void Paste()
        {
            History<ShapeCollection>.Memorize(_shapes);

            foreach (IShape shape in Clipboard<ShapeCollection>.Clip)
            {
                shape.Location = new PointF(shape.Location.X + 10, shape.Location.Y + 10);
                _shapes.Add(shape.Clone() as IShape);
            }

            Invalidate();
        }

        /// <summary>
        /// Delete action.
        /// </summary>
        virtual public void Delete()
        {
            History<ShapeCollection>.Memorize(_shapes);

            for (int i = 0; i < _shapes.Count; i++)
            {
                if (_shapes[i].Selected)
                {
                    _shapes.Remove(_shapes[i]);
                    i = -1;
                }
            }

            Invalidate();
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Mouse down.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            _tool.MouseDown(this, e);

            base.OnMouseDown(e);
            Invalidate();
        }

        /// <summary>
        /// Mouse up.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _tool.MouseUp(this, e);

            base.OnMouseUp(e);
            Invalidate();
        }

        /// <summary>
        /// Mouse move.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _tool.MouseMove(this, e);

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Mouse wheel.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!_enableWheelZoom)
                return;

            _zoomCenter = false;

            if (Math.Sign(e.Delta) == -1)
                Zoom = 0.9f;
            else if (Math.Sign(e.Delta) == 1)
                Zoom = 1.1f;

            _zoomCenter = true;

            base.OnMouseWheel(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Jx.Drawing.Common.Select.LastSelectedShape == null)
                return base.ProcessDialogKey(keyData);

            ShapeCollection selectedShapes = new ShapeCollection();

            Keys key = keyData & Keys.KeyCode;

            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
                selectedShapes.AddRange(Jx.Drawing.Common.Select.GetSelectedShapes(_shapes));
            else
                selectedShapes.Add(Jx.Drawing.Common.Select.LastSelectedShape);

            float offsetX = _gridManager.Resolution.Width;
            if (_gridManager.Resolution.Width == 0)
                offsetX = 1;

            float offsetY = _gridManager.Resolution.Height;
            if (_gridManager.Resolution.Height == 0)
                offsetY = 1;

            switch (key)
            {
                case Keys.Up:
                    foreach (IShape shape in selectedShapes)
                        shape.Transformer.Translate(0, -offsetY);

                    Invalidate();
                    break;

                case Keys.Down:
                    foreach (IShape shape in selectedShapes)
                        shape.Transformer.Translate(0, offsetY);

                    Invalidate();
                    break;

                case Keys.Left:
                    foreach (IShape shape in selectedShapes)
                        shape.Transformer.Translate(-offsetX, 0);

                    Invalidate();
                    break;

                case Keys.Right:
                    foreach (IShape shape in selectedShapes)
                        shape.Transformer.Translate(offsetX, 0);

                    Invalidate();
                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Paint.
        /// </summary>
        /// <param name="e">PaintEventArgs.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Painting(_shapes, e);

            _tool.Paint(this, e);
        }

        /// <summary>
        /// Draws shapes and other.
        /// </summary>
        /// <param name="shapes">Shapes to draw.</param>
        /// <param name="e">PaintEventArgs.</param>
        protected void Painting(ShapeCollection shapes, PaintEventArgs e)
        {
            if (_gridManager.Resolution != Size.Empty)
                ControlPaint.DrawGrid(e.Graphics, e.ClipRectangle, Size.Round(_gridManager.Resolution), BackColor);

            foreach (IShape shape in shapes)
                shape.Paint(this, e);
        }

        #endregion

        #region Private Functions

        void _shapes_ShapeChanged(IShape shape, object changing)
        {
            History<ShapeCollection>.Memorize(_shapes);

            RectangleF rectangle = new RectangleF(
                new PointF(
                shape.Location.X - shape.Appearance.GrabberDimension,
                shape.Location.Y - shape.Appearance.GrabberDimension),
                new SizeF(
                shape.Dimension.Width + shape.Appearance.GrabberDimension,
                shape.Dimension.Height + shape.Appearance.GrabberDimension));

            Invalidate(System.Drawing.Rectangle.Round(rectangle));
        }

        void _shapes_ShapeMovementOccurred(Transformer transformer)
        {
            History<ShapeCollection>.Memorize(_shapes);

            RectangleF rectangle = new RectangleF(
                new PointF(
                transformer.Shape.Location.X - transformer.Shape.Appearance.GrabberDimension,
                transformer.Shape.Location.Y - transformer.Shape.Appearance.GrabberDimension),
                new SizeF(
                transformer.Shape.Dimension.Width + transformer.Shape.Appearance.GrabberDimension,
                transformer.Shape.Dimension.Height + transformer.Shape.Appearance.GrabberDimension));

            Invalidate(System.Drawing.Rectangle.Round(rectangle));
        }

        void _shapes_ShapeAppearanceChanged(Jx.Drawing.Common.Appearance appearance)
        {
            History<ShapeCollection>.Memorize(_shapes);

            RectangleF rectangle = new RectangleF(
                new PointF(
                appearance.Shape.Location.X - appearance.Shape.Appearance.GrabberDimension,
                appearance.Shape.Location.Y - appearance.Shape.Appearance.GrabberDimension),
                new SizeF(
                appearance.Shape.Dimension.Width + appearance.Shape.Appearance.GrabberDimension,
                appearance.Shape.Dimension.Height + appearance.Shape.Appearance.GrabberDimension));

            Invalidate(System.Drawing.Rectangle.Round(rectangle));
        }

        void _gridManager_ResolutionChanged(GridManager sender, SizeF oldValue, SizeF newValue)
        {
            GridManager.SnapToGrid(_shapes);
            Invalidate();
        }

        #endregion
    }
}

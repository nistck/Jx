using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Jx.Drawing.Serialization.XML;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Shape used by some tools (for example DrawShape and Resize) to manage the movements of a shape.
    /// </summary>
    public class Ghost : Shape
    {
        IShape _memoryShape = null;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Ghost()
        {
            this.Transformer.MirrorHorizontalOccurred += new MirrorHorizontalHandler(Transformer_MirrorHorizontalOccurred);
            this.Transformer.MirrorVerticalOccurred +=new MirrorVerticalHandler(Transformer_MirrorVerticalOccurred);

            this.Appearance = new GhostAppearance();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shape">Shape to manage.</param>
        public Ghost(IShape shape)
        {
            _referenceShape = shape;
            _memoryShape = shape.Clone() as IShape;
            _shape = _memoryShape.Clone() as IShape;

            Geometric.Reset();
            Geometric.AddPath(shape.Geometric, false);

            Selected = false;
            Visible = false;

            this.Transformer.MirrorHorizontalOccurred += new MirrorHorizontalHandler(Transformer_MirrorHorizontalOccurred);
            this.Transformer.MirrorVerticalOccurred += new MirrorVerticalHandler(Transformer_MirrorVerticalOccurred);

            this.Appearance = new GhostAppearance();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="ghost">Shape to copy.</param>
        public Ghost(Ghost ghost) : base(ghost)
        {
            _referenceShape = ghost.Shape;
            _memoryShape = ghost.Shape.Clone() as IShape;
            _shape = _memoryShape.Clone() as IShape;

            Geometric.Reset();
            Geometric.AddPath(ghost.Shape.Geometric, false);

            ghost.Selected = false;
            ghost.Visible = false;

            this.Transformer.MirrorHorizontalOccurred += new MirrorHorizontalHandler(Transformer_MirrorHorizontalOccurred);
            this.Transformer.MirrorVerticalOccurred += new MirrorVerticalHandler(Transformer_MirrorVerticalOccurred);

            this.Appearance = new GhostAppearance();
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the ghost.
        /// </summary>
        /// <returns>New shape.</returns>
        public override object Clone()
        {
            return new Ghost(this);
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Mouse down function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseDown(IDocument document, System.Windows.Forms.MouseEventArgs e)
        {
            _shape = _memoryShape.Clone() as IShape;

            Geometric.Reset();
            Geometric.AddPath(_shape.Geometric, false);

            Visible = false;
            Selected = false;

            InitializeVersors(_shape.HitTest(e.Location));

            base.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(document, e);

            Visible = false;
            _mirrorPoint = Point.Empty;

            _horizontalVersor = HorizontalVersors.LeftRight;
            _verticalVersor = VerticalVersors.TopBottom;
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            base.MouseMove(document, e);

            Visible = true;
            Selected = true;

            UpdateVersors(_mirrorPoint, e.Location);

            _shape.Location = Location;
            _shape.Dimension = Dimension;

            document.DrawingControl.Invalidate();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or sets the selecting and the managed shape selecting.
        /// </summary>
        public override bool Selected
        {
            get { return base.Selected; }

            set
            {
                base.Selected = value;

                if (_shape != null)
                    _shape.Selected = value;
            }
        }

        /// <summary>
        /// Get or sets the visibility and the managed shape visibility.
        /// </summary>
        public override bool Visible
        {
            get { return base.Visible; }

            set
            {
                base.Visible = value;

                if (_shape != null)
                    _shape.Visible = value;
            }
        }

        #endregion

        #endregion

        #region Properties

        IShape _shape = null;
        /// <summary>
        /// Gets or sets the shape to manage.
        /// </summary>
        virtual public IShape Shape
        {
            get { return _shape; }

            set
            {
                if (value == null)
                    throw new ApplicationException();

                _referenceShape = value;
                _memoryShape = value.Clone() as IShape;
                _shape = _memoryShape.Clone() as IShape;

                Geometric.Reset();
                Geometric.AddPath(value.Geometric, false);

                Selected = false;
                Visible = false;
            }
        }

        IShape _referenceShape = null;
        /// <summary>
        /// Gets the real shape. Any changes in this change will reflect to real shape.
        /// </summary>
        public IShape ReferenceShape
        {
            get { return _referenceShape; }
        }

        PointF _mirrorPoint = PointF.Empty;
        /// <summary>
        /// Gets or sets the mirror point to manage shape flipping.
        /// </summary>
        public PointF MirrorPoint
        {
            get { return _mirrorPoint; }
            set
            {
                if (_mirrorPoint == PointF.Empty)
                    _mirrorPoint = value;
            }
        }

        HorizontalVersors _horizontalVersor = HorizontalVersors.LeftRight;
        /// <summary>
        /// Gets or sets the horizontal versor.
        /// </summary>
        public HorizontalVersors HorizontalVersor
        {
            get { return _horizontalVersor; }
            set { _horizontalVersor  = value; }
        }

        VerticalVersors _verticalVersor = VerticalVersors.TopBottom;
        /// <summary>
        /// Gets or sets the vertical versor.
        /// </summary>
        public VerticalVersors VerticalVersor
        {
            get { return _verticalVersor; }
            set { _verticalVersor  = value; }
        }

        bool _horizontalMirror = true;
        /// <summary>
        /// Gets or sets the ability to flip horizontally.
        /// </summary>
        protected bool HorizontalMirror
        {
            get { return _horizontalMirror; }
            set { _horizontalMirror = value; }
        }

        bool _verticalMirror = true;
        /// <summary>
        /// Gets or sets the ability to flip vertically.
        /// </summary>
        protected bool VerticalMirror
        {
            get { return _verticalMirror; }
            set { _verticalMirror = value; }
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Updates versors.
        /// </summary>
        /// <param name="mirrorPoint">Mirror point.</param>
        /// <param name="currentPoint">Mouse location.</param>
        virtual protected void UpdateVersors(PointF mirrorPoint, Point currentPoint)
        {
            if (_mirrorPoint == PointF.Empty)
                return;

            HorizontalVersors horizontalVersor = GetHorizontalVersor(currentPoint);
            VerticalVersors verticalVersor = GetVerticalVersor(currentPoint);

            if (_horizontalVersor != horizontalVersor && horizontalVersor != HorizontalVersors.Undefined && _horizontalMirror)
            {
                Transformer.MirrorHorizontal(mirrorPoint.X);
                _horizontalVersor = horizontalVersor;
            }

            if (_verticalVersor != verticalVersor && verticalVersor != VerticalVersors.Undefined && _verticalMirror)
            {
                Transformer.MirrorVertical(mirrorPoint.Y);
                _verticalVersor = verticalVersor;
            }
        }

        /// <summary>
        /// Gets horizontal versor relative to mirror point
        /// </summary>
        /// <param name="point">Current point</param>
        /// <returns></returns>
        protected HorizontalVersors GetHorizontalVersor(Point point)
        {
            if (point.X < _mirrorPoint.X)
                return HorizontalVersors.RightLeft;
            else if (point.X > _mirrorPoint.X)
                return HorizontalVersors.LeftRight;

            return HorizontalVersors.Undefined;
        }

        /// <summary>
        /// Gets vertical versor relative to mirror point
        /// </summary>
        /// <param name="point">Current point</param>
        /// <returns></returns>
        protected VerticalVersors GetVerticalVersor(Point point)
        {
            if (point.Y < _mirrorPoint.Y)
                return VerticalVersors.BottomTop;
            else if (point.Y > _mirrorPoint.Y)
                return VerticalVersors.TopBottom;

            return VerticalVersors.Undefined;
        }

        /// <summary>
        /// Initializes versors relative to ghosted shape
        /// </summary>
        /// <param name="hitPosition">Ghosted shape hit position</param>
        protected void InitializeVersors(HitPositions hitPosition)
        {
            _horizontalMirror = true;
            _verticalMirror = true;

            switch (hitPosition)
            {
                case HitPositions.TopLeft:
                {
                    _horizontalVersor = HorizontalVersors.RightLeft;
                    _verticalVersor = VerticalVersors.BottomTop;

                    break;
                }
                case HitPositions.Top:
                {
                    _horizontalVersor = HorizontalVersors.LeftRight;
                    _verticalVersor = VerticalVersors.BottomTop;

                    _horizontalMirror = false;

                    break;
                }
                case HitPositions.TopRight:
                {
                    _horizontalVersor = HorizontalVersors.LeftRight;
                    _verticalVersor = VerticalVersors.BottomTop;

                    break;
                }
                case HitPositions.Right:
                {
                    _horizontalVersor = HorizontalVersors.LeftRight;
                    _verticalVersor = VerticalVersors.TopBottom;

                    _verticalMirror = false;

                    break;
                }
                case HitPositions.BottomRight:
                {
                    _horizontalVersor = HorizontalVersors.LeftRight;
                    _verticalVersor = VerticalVersors.TopBottom;

                    break;
                }
                case HitPositions.Bottom:
                {
                    _horizontalVersor = HorizontalVersors.LeftRight;
                    _verticalVersor = VerticalVersors.TopBottom;

                    _horizontalMirror = false;

                    break;
                }
                case HitPositions.BottomLeft:
                {
                    _horizontalVersor = HorizontalVersors.RightLeft;
                    _verticalVersor = VerticalVersors.TopBottom;

                    break;
                }
                case HitPositions.Left:
                {
                    _horizontalVersor = HorizontalVersors.RightLeft;
                    _verticalVersor = VerticalVersors.TopBottom;

                    _verticalMirror = false;

                    break;
                }
            }
        }

        #endregion

        #region Private Functions

        void Transformer_MirrorHorizontalOccurred(Transformer transformer, float x)
        {
            if (_shape != null)
                _shape.Transformer.MirrorHorizontal(x);
        }

        void Transformer_MirrorVerticalOccurred(Transformer transformer, float y)
        {
            if (_shape != null)
                _shape.Transformer.MirrorVertical(y);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using Jx.Serialization.XML;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Mananges the appearance of polygon shapes.
    /// </summary>
    [XmlClassSerializable("polygonAppearance")]
    public class PolygonAppearance : Appearance
    {
        #region Added properties to serialize

        [XmlFieldSerializable("backgroundColor1String")]
        string BackGroundColor1String
        {
            get { return Jx.Drawing.Converters.ColorConverter.StringFromColor(_backgroundColor1, ';'); }
            set { _backgroundColor1 = Jx.Drawing.Converters.ColorConverter.ColorFromString(value, ';'); }
        }

        [XmlFieldSerializable("backgroundColor2String")]
        string BackGroundColor2String
        {
            get { return Jx.Drawing.Converters.ColorConverter.StringFromColor(_backgroundColor2, ';'); }
            set { _backgroundColor2 = Jx.Drawing.Converters.ColorConverter.ColorFromString(value, ';'); }
        }

        #endregion

        #region Events and Delegates

        /// <summary>
        /// Fires when appearance is changed.
        /// </summary>
        public override event AppearanceHandler AppearanceChanged;

        /// <summary>
        /// Fires when background color 1 is changed.
        /// </summary>
        virtual public event BackgroundColor1Handler BackgroundColor1Changed;

        /// <summary>
        /// Fires when background color 2 is changed.
        /// </summary>
        virtual public event BackgroundColor2Handler BackgroundColor2Changed;

        /// <summary>
        /// Fires when gradient angle is changed.
        /// </summary>
        virtual public event GradientAngleHandler GradientAngleChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PolygonAppearance()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="polygonAppearance">Arrearance to copy.</param>
        public PolygonAppearance(PolygonAppearance polygonAppearance) : base (polygonAppearance)
        {
            _backgroundColor1 = polygonAppearance._backgroundColor1;
            _backgroundColor2 = polygonAppearance._backgroundColor2;
            _gradientAngle = polygonAppearance._gradientAngle;
        }

        #endregion

        #region Properties

        Color _backgroundColor1 = Color.Transparent;
        /// <summary>
        /// Gets or sets the first background color.
        /// </summary>
		public Color BackgroundColor1
        {
            get { return _backgroundColor1; }
            set
            {
                Color memory = _backgroundColor1;
                _backgroundColor1 = value;

                if (BackgroundColor1Changed != null && memory != _backgroundColor1)
                    BackgroundColor1Changed(this, memory, _backgroundColor1);

                if (AppearanceChanged != null && memory != _backgroundColor1)
                    AppearanceChanged(this);
            }
        }

        Color _backgroundColor2 = Color.Transparent;
        /// <summary>
        /// Gets or sets the second background color.
        /// </summary>
		public Color BackgroundColor2
        {
            get { return _backgroundColor2; }
            set
            {
                Color memory = _backgroundColor2;
                _backgroundColor2 = value;

                if (BackgroundColor2Changed != null && memory != _backgroundColor2)
                    BackgroundColor2Changed(this, memory, _backgroundColor2);

                if (AppearanceChanged != null && memory != _backgroundColor2)
                    AppearanceChanged(this);
            }
        }

        float _gradientAngle = 0f;
        /// <summary>
        /// Gets or sets the gradient angle for the background colors.
        /// </summary>
        [XmlFieldSerializable("gradientAngle")]
        public float GradientAngle
        {
            get { return _gradientAngle; }
            set
            {
                float memory = _gradientAngle;
                _gradientAngle = value;

                if (GradientAngleChanged != null && memory != _gradientAngle)
                    GradientAngleChanged(this, memory, _gradientAngle);

                if (AppearanceChanged != null && memory != _gradientAngle)
                    AppearanceChanged(this);
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Clones the Appearance.
        /// </summary>
        /// <returns>New Appearance.</returns>
        override public object Clone()
        {
            return new PolygonAppearance(this);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            base.Paint(document, e);

            DrawBackground(document, e);
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        virtual protected void DrawBackground(IDocument document, PaintEventArgs e)
        {
            if (!IsValidGeometric(Shape.Geometric))
                return;

            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(Shape.Geometric.GetBounds(), _backgroundColor1, _backgroundColor2, _gradientAngle, true))
            {
                e.Graphics.FillPath(gradientBrush, Shape.Geometric);
            }
        }
        
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Jx.Drawing.Common;
using Jx.Serialization.XML;

namespace Jx.Drawing.Base
{
    /// <summary>
    /// Text shape.
    /// </summary>
    [XmlClassSerializable("text")]
    public class Text : Shape
    {
        #region Added properties to serialize

        float _fontSize = 0;
        [XmlFieldSerializable("fontSize")]
        float FontSize
        {
            get { return _font.Size; }
            set
            {
                _fontSize = value;
                UpdateAfterLoad();
            }
        }

        FontStyle _fontStyle = FontStyle.Regular;
        [XmlFieldSerializable("fontStyle")]
        int FontStyleEnum
        {
            get { return (int)_font.Style; }
            set
            {
                _fontStyle = (FontStyle)value;
                UpdateAfterLoad();
            }
        }

        GraphicsUnit _fontGraphicUnit = GraphicsUnit.Pixel;
        [XmlFieldSerializable("fontGraphicsUnit")]
        int FontGraphicUnitEnum
        {
            get { return (int)_font.Unit; }
            set
            {
                _fontGraphicUnit = (GraphicsUnit)value;
                UpdateAfterLoad();
            }
        }

        string _fontFamily = string.Empty;
        [XmlFieldSerializable("fontFamily")]
        string FontFamilyString
        {
            get { return _font.FontFamily.GetName(0); }
            set
            {
                _fontFamily = value;
                UpdateAfterLoad();
            }
        }

        /// <summary>
        /// Create the font after all fields of the font are ready.
        /// </summary>
        private void UpdateAfterLoad()
        {
            if (_fontSize > 0 && _fontFamily != string.Empty)
                _font = new Font(new FontFamily(_fontFamily), _fontSize, _fontStyle, _fontGraphicUnit);

            UpdateText();
        }

        #endregion

        #region Data Members

        float _degree = 0.0f;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Text()
        {
            Geometric.AddString(_displayedText, _font.FontFamily, (int)_font.Style, _font.Size, Geometric.GetBounds(), _stringFormat);
            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="text">Text to copy.</param>
        public Text(Text text)
            : base(text)
        {
            _displayedText = text.DisplayedText;
            _font = text.Font.Clone() as Font;
            _stringFormat = text.StringFormat.Clone() as StringFormat;
            _degree = text._degree;

            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text to view.</param>
        /// <param name="font">Text font.</param>
        /// <param name="stringFormat">String format text.</param>
        public Text(string text, Font font, StringFormat stringFormat)
        {
            if (text != string.Empty)
                _displayedText = text;

            _font = font;
            _stringFormat = stringFormat;

            Geometric.AddString(text, _font.FontFamily, (int)_font.Style, _font.Size, Geometric.GetBounds(), _stringFormat);

            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the shape.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Text(this);
        }

        #endregion

        #endregion

        #region Properties

        string _displayedText = "Text";
        /// <summary>
        /// Gets or sets (protected) the displayed text.
        /// </summary>
        [XmlFieldSerializable("displayedText")]
        public string DisplayedText
        {
            get { return _displayedText; }
            
            set
            {
                _displayedText = value;
                UpdateText();
            }
        }

        Font _font = new Font(FontFamily.GenericSansSerif, 12);
        /// <summary>
        /// Gets or sets the text font.
        /// </summary>
        public Font Font
        {
            get { return _font; }
            
            set
            {
                _font = value;
                UpdateText();
            }
        }

        StringFormat _stringFormat = new StringFormat(StringFormatFlags.NoWrap);
        /// <summary>
        /// Gets or sets the string format text.
        /// </summary>
        public StringFormat StringFormat
        {
            get { return _stringFormat; }
            
            set
            {
                _stringFormat = value;
                UpdateText();
            }
        }

        #endregion

        #region Protected Functions

        virtual protected void UpdateText()
        {
            SizeF oldDimension = Dimension;
            PointF oldLocation = Location;
            float oldRotation = Rotation;

            Geometric.Reset();
            Geometric.AddString(_displayedText, _font.FontFamily, (int)_font.Style, _font.Size, Geometric.GetBounds(), _stringFormat);

            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
            matrix.Rotate(_degree);
            Geometric.Transform(matrix);

            if (_degree != oldRotation)
                base.Rotation = _degree;

            base.Dimension = oldDimension;
            base.Location = oldLocation;
        }

        #endregion

        #region Private Functions

        void Transformer_RotateOccurred(Transformer transformer, float degree, PointF point)
        {
            _degree += degree;
        }

        #endregion
    }
}

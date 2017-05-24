//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;
//using Globe.Graphics.Bidimensional.Common;
//using Globe.Xml.Serialization;

//namespace Globe.Graphics.Bidimensional.Base
//{
//    /// 
//    /// Text shape.
//    /// 
//    [XmlClassSerializable("text")]
//    public class Text : Shape
//    {
//        #region Added properties to serialize

//        float _degree;

//        float _fontSize = 0;
//        [XmlFieldSerializable("fontSize")]
//        float FontSize
//        {
//            get { return _font.Size; }
//            set
//            {
//                _fontSize = value;
//                UpdateAfterLoad();
//            }
//        }

//        FontStyle _fontStyle = FontStyle.Regular;
//        [XmlFieldSerializable("fontStyle")]
//        int FontStyleEnum
//        {
//            get { return (int)_font.Style; }
//            set
//            {
//                _fontStyle = (FontStyle)value;
//                UpdateAfterLoad();
//            }
//        }

//        GraphicsUnit _fontGraphicUnit = GraphicsUnit.Pixel;
//        [XmlFieldSerializable("fontGraphicsUnit")]
//        int FontGraphicUnitEnum
//        {
//            get { return (int)_font.Unit; }
//            set
//            {
//                _fontGraphicUnit = (GraphicsUnit)value;
//                UpdateAfterLoad();
//            }
//        }

//        string _fontFamily = string.Empty;
//        [XmlFieldSerializable("fontFamily")]
//        string FontFamilyString
//        {
//            get { return _font.FontFamily.GetName(0); }
//            set
//            {
//                _fontFamily = value;
//                UpdateAfterLoad();
//            }
//        }

//        /// 
//        /// Create the font after all fields of the font are ready
//        /// 
//        private void UpdateAfterLoad()
//        {
//            if (_fontSize > 0 && _fontFamily != string.Empty)
//                _font = new Font(new FontFamily(_fontFamily), _fontSize, _fontStyle, _fontGraphicUnit);

//            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
//        }

//        void Transformer_RotateOccurred(Transformer transformer, float degree, PointF point)
//        {
//            _degree += degree;
//        }

//        #endregion

//        #region Constructors

//        /// 
//        /// Default constructor.
//        /// 
//        public Text()
//        {
//            Geometric.AddString(_text, _font.FontFamily, (int)_font.Style, _font.Size, Geometric.GetBounds(), _stringFormat);
//            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
//        }

//        /// 
//        /// Copy constructor.
//        /// 
//        /// Text to copy.
//        public Text(Text text)
//            : base(text)
//        {
//            this.DisplayedText = text.DisplayedText;
//            this.Font = text.Font.Clone() as Font;
//            this.StringFormat = text.StringFormat.Clone() as StringFormat;
//            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
//        }

//        /// 
//        /// Constructor.
//        /// 
//        /// Text to view.
//        /// Text font.
//        /// String format text.
//        public Text(string text, Font font, StringFormat stringFormat)
//        {
//            if (text != string.Empty)
//                _text = text;

//            _font = font;
//            _stringFormat = stringFormat;

//            Geometric.AddString(text, _font.FontFamily, (int)_font.Style, _font.Size, Geometric.GetBounds(), _stringFormat);
//            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
//        }

//        #endregion

//        #region IShape Interface

//        #region ICloneable Interface

//        /// 
//        /// Clones the shape.
//        /// 
//        /// 
//        public override object Clone()
//        {
//            return new Text(this);
//        }

//        #endregion

//        #endregion

//        #region Properties

//        string _text = "Text1";
//        /// 
//        /// Gets or sets (protected) the displayed text.
//        /// 
//        [XmlFieldSerializable("displayedText")]
//        public string DisplayedText
//        {
//            get { return _text; }
//            set
//            {
//                SizeF dimension = Dimension;
//                PointF location = Location;
//                float rotation = Rotation;

//                if (value != _text)
//                {
//                    Geometric.Reset();
//                    _text = value;

//                    Geometric.AddString(_text, _font.FontFamily, (int)_font.Style, _font.Size, Geometric.GetBounds(), _stringFormat);
//                }

//                System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
//                matrix.Rotate(_degree);
//                Geometric.Transform(matrix);

//                if (_degree != rotation)
//                    base.Rotation = _degree;

//                base.Dimension = dimension;
//                base.Location = location;
//            }
//        }

//        Font _font = new Font(FontFamily.GenericSansSerif, 12);
//        /// 
//        /// Gets or sets the text font.
//        /// 
//        public Font Font
//        {
//            get { return _font; }
//            set { _font = value; }
//        }

//        StringFormat _stringFormat = new StringFormat(StringFormatFlags.NoWrap);
//        /// 
//        /// Gets or sets the string format text.
//        /// 
//        public StringFormat StringFormat
//        {
//            get { return _stringFormat; }
//            set { _stringFormat = value; }
//        }

//        #endregion
//    }
//}

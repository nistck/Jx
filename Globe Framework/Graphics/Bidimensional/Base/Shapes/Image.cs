using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Globe.Graphics.Bidimensional.Common;
using Globe.Xml.Serialization;

namespace Globe.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Image shape.
    /// </summary>
    [XmlClassSerializable("image")]
    public class Image : Shape
    {
        #region Added properties to serialize

        [XmlFieldSerializable("imageBytes")]
        byte[] ImageBytes
        {
            get { return Globe.Core.Converters.BitmapConverter.BytesFromBitmap(_bitmap); }
            set { _bitmap = Globe.Core.Converters.BitmapConverter.BitmapFromBytes(value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Image()
        {
            Geometric.AddLine(new Point(0, 0), new Point(1, 1));
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="image">Image to copy.</param>
        public Image(Image image) : base(image)
        {
            _bitmap = image._bitmap.Clone() as Bitmap;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bitmap">Reference bitmap.</param>
        public Image(Bitmap bitmap)
        {
            Geometric.AddLine(new Point(0, 0), new Point(1, 1));

            _bitmap = bitmap.Clone() as Bitmap;
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
            return new Image(this);
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Paint function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">PaintEventArgs</param>
        public override void Paint(IDocument document, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(_bitmap, Location);
        }

        #endregion

        #endregion

        #region Properties

        Bitmap _bitmap = null;
        /// <summary>
        /// Gets or sets the bitmap image.
        /// </summary>
        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set { _bitmap = value; }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Drawing;

namespace Jx.Core.Converters
{
    /// <summary>
    /// Manages some conversions of a bitmap.
    /// </summary>
    public class BitmapConverter
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BitmapConverter()
        {
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Gets a bitmap from byte[].
        /// </summary>
        /// <param name="bytes">Byte[] to convert.</param>
        /// <returns>Bitmap.</returns>
        public static Bitmap BitmapFromBytes(byte[] bytes)
        {
            Bitmap bitmap = null;
            if (bytes != null)
                bitmap = new Bitmap(new MemoryStream(bytes));

            return bitmap;
        }

        /// <summary>
        /// Gets byte[] from a bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to convert.</param>
        /// <returns>Byte[].</returns>
        public static byte[] BytesFromBitmap(Bitmap bitmap)
        {
            try
            {
                System.ComponentModel.TypeConverter bitmapConverter =
                    System.ComponentModel.TypeDescriptor.GetConverter(bitmap.GetType());
                return (byte[])bitmapConverter.ConvertTo(bitmap, typeof(byte[]));
            }
            catch
            {
                throw new ApplicationException();
            }
        }

        #endregion
    }
}

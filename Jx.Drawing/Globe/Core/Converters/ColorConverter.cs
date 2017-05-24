using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Jx.Core.Converters
{
    /// <summary>
    /// Manages some conversions of a color.
    /// </summary>
    public class ColorConverter
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorConverter()
        {
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Gets a color from string.
        /// </summary>
        /// <param name="argb">String to convert.</param>
        /// <param name="separator">Char separator for a, r, g, b.</param>
        /// <returns>Color.</returns>
        public static Color ColorFromString(string argb, char separator)
        {
            string[] components = argb.Split(new char[] {separator});

            Color color = Color.White;

            try
            {
                color = Color.FromArgb(
                    int.Parse(components[0]),
                    int.Parse(components[1]),
                    int.Parse(components[2]),
                    int.Parse(components[3]));
            }
            catch
            {
                throw new ApplicationException();
            }

            return color;
        }

        /// <summary>
        /// Gets a string from a color.
        /// </summary>
        /// <param name="color">Color to convert.</param>
        /// <param name="separator">Char separator for a, r, g, b.</param>
        /// <returns>String that rapresents the color (format: a'separator'r'separator'g'separator'b).</returns>
        public static string StringFromColor(Color color, char separator)
        {
            string argb = color.A.ToString() + separator + color.R.ToString() + separator + color.G.ToString() + separator + color.B.ToString();

            return argb;
        }

        #endregion
    }
}

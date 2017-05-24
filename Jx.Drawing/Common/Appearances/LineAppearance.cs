using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Jx.Drawing.Serialization.XML;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Mananges the appearance of line shapes.
    /// </summary>
    [XmlClassSerializable("lineAppearance")]
    public class LineAppearance : Appearance
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LineAppearance()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="lineAppearance">Appearance to copy.</param>
        public LineAppearance(LineAppearance lineAppearance) : base (lineAppearance)
        {
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Clones the Appearance.
        /// </summary>
        /// <returns>New Appearance.</returns>
        override public object Clone()
        {
            return new LineAppearance(this);
        }

        #endregion
    }
}

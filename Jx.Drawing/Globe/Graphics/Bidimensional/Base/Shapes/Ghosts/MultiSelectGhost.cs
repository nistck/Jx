using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Jx.Graphics.Bidimensional.Common;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Ghost used by multi select tool.
    /// </summary>
    [Serializable]
    public class MultiSelectGhost : Ghost
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MultiSelectGhost() : base(new Rectangle())
        {
            _brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.LightBlue));
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Paint function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">PaintEventArgs</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            if (!Visible)
                return;

            e.Graphics.FillRectangle(_brush, System.Drawing.Rectangle.Round(new System.Drawing.RectangleF(Location, Dimension)));
            e.Graphics.DrawRectangle(System.Drawing.Pens.Black, System.Drawing.Rectangle.Round(Geometric.GetBounds()));
        }

        #endregion

        #region Properties

        System.Drawing.SolidBrush _brush = null;
        /// <summary>
        /// Gets or sets the brush to draw.
        /// </summary>
        protected System.Drawing.SolidBrush DrawingBrush
        {
            get { return _brush; }
            set { _brush = value; }
        }

        #endregion
    }
}

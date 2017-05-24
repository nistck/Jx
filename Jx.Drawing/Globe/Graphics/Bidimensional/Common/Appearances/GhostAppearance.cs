using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Jx.Xml.Serialization;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Mananges the appearance of ghosts.
    /// </summary>
    [XmlClassSerializable("ghostAppearance")]
    public class GhostAppearance : PolygonAppearance
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GhostAppearance()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="ghostAppearance">Appearance to copy.</param>
        public GhostAppearance(GhostAppearance ghostAppearance) : base (ghostAppearance)
        {
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Clones the Appearance.
        /// </summary>
        /// <returns>New Appearance.</returns>
        public override object Clone()
        {
            return new GhostAppearance(this);
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Draws the frame selection if Selected property is true.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        protected override void DrawSelection(IDocument document, PaintEventArgs e)
        {
            if (!Shape.Visible || !IsValidGeometric(Shape.Geometric))
                return;

            System.Drawing.Rectangle outside = System.Drawing.Rectangle.Round(Shape.Geometric.GetBounds());
            System.Drawing.Rectangle inside = outside;

            outside.Inflate(GrabberDimension / 2, GrabberDimension / 2);
            inside.Inflate(-GrabberDimension / 2, -GrabberDimension / 2);

            ControlPaint.DrawSelectionFrame(e.Graphics, true, outside, inside, document.DrawingControl.BackColor);
        }

        /// <summary>
        /// Updates the back.
        /// </summary>
        /// <param name="backColor">Back color.</param>
        virtual protected void UpdateActivePen(Color backColor)
        {
            ActivePen = new Pen(Color.Black, 2);
        }

        #endregion
    }
}

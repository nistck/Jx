using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Windows.Forms;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Interface to capture DrawingPanel events.
    /// </summary>
    public interface IActions
    {
        /// <summary>
        /// Mouse down function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        void MouseDown(IDocument document, MouseEventArgs e);

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        void MouseUp(IDocument document, MouseEventArgs e);

        /// <summary>
        /// Mouse click function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        void MouseClick(IDocument document, MouseEventArgs e);

        /// <summary>
        /// Mouse double click function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        void MouseDoubleClick(IDocument document, MouseEventArgs e);

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        void MouseMove(IDocument document, MouseEventArgs e);

        /// <summary>
        /// Mouse wheel function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        void MouseWheel(IDocument document, MouseEventArgs e);

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
		void Paint(IDocument document, PaintEventArgs e);
   }
}

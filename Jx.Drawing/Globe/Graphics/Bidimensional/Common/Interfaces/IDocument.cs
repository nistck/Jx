using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Interface to connect DrawingPanel (that derives from this) to tools
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Gets drawing panel.
        /// </summary>
        Control DrawingControl { get; }

        /// <summary>
        /// Gets shapes.
        /// </summary>
        ShapeCollection Shapes { get; }

        /// <summary>
        /// Gets or sets current cursor.
        /// </summary>
        Cursor ActiveCursor { get; set; }

        /// <summary>
        /// Gets the current tool.
        /// </summary>
        Tool ActiveTool { get; }

        /// <summary>
        /// Gets GridManager to handle grid properties.
        /// </summary>
        GridManager GridManager { get; }
    }
}

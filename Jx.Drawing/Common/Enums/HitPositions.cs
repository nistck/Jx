using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Mouse positions on a clicked shape
    /// </summary>
    public enum HitPositions
    {
        /// <summary>
        /// Out shape.
        /// </summary>
		None,

        /// <summary>
        /// Center shape.
        /// </summary>
		Center,

        /// <summary>
        /// For future use.
        /// </summary>
        All,

        /// <summary>
        /// Top-Left border.
        /// </summary>
		TopLeft,

        /// <summary>
        /// Top border.
        /// </summary>
        Top,

        /// <summary>
        /// Top-Right border.
        /// </summary>
        TopRight,

        /// <summary>
        /// Right border.
        /// </summary>
        Right,

        /// <summary>
        /// Bottom-Right border.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Bottom border.
        /// </summary>
        Bottom,

        /// <summary>
        /// Bottom-Left border.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Left border.
        /// </summary>
        Left
    }
}

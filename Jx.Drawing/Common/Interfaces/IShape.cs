using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Interface to manage all shapes. You must work with this for convenience.
    /// </summary>
    public interface IShape : ICloneable, IActions
    {
        #region Events

        /// <summary>
        /// Fires when change a property. Used to Undo-Redo mechanism.
        /// </summary>
        event Jx.Drawing.Common.ShapeChangingHandler ShapeChanged;

        /// <summary>
        /// Fires when mouse down on shape.
        /// </summary>
        event Jx.Drawing.Common.MouseDownOnShape ShapeMouseDown;

        /// <summary>
        /// Fires when mouse up on shape.
        /// </summary>
        event Jx.Drawing.Common.MouseUpOnShape ShapeMouseUp;

        /// <summary>
        /// Fires when mouse click on shape.
        /// </summary>
        event Jx.Drawing.Common.MouseClickOnShape ShapeMouseClick;

        /// <summary>
        /// Fires when mouse double click on shape.
        /// </summary>
        event Jx.Drawing.Common.MouseDoubleClickOnShape ShapeMouseDoubleClick;

        /// <summary>
        /// Fires when mouse move on shape.
        /// </summary>
        event Jx.Drawing.Common.MouseMoveOnShape ShapeMouseMove;

        /// <summary>
        /// Fires when mouse wheel on shape.
        /// </summary>
        event Jx.Drawing.Common.MouseWheel ShapeMouseWheel;

        /// <summary>
        /// Fires when paint shape.
        /// </summary>
        event Jx.Drawing.Common.PaintOnShape ShapePaint;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        PointF Location { get; set; }

        /// <summary>
        /// Gets or sets the current size.
        /// </summary>
        SizeF Dimension { get; set; }

        /// <summary>
        /// Gets or sets the current center.
        /// </summary>
        PointF Center { get; set; }

        /// <summary>
        /// Gets or sets the current rotation.
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// Get or sets the visibility.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the locking during movements.
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// Gets oe sets the selecting.
        /// </summary>
        bool Selected { get; set; }

        /// <summary>
        /// Gets the geometric form of the shape.
        /// </summary>
		GraphicsPath Geometric { get; }

        /// <summary>
        /// Gets or sets the transformer.
        /// </summary>
        Transformer Transformer { get; set; }

        /// <summary>
        /// Gets or sets the appearance.
        /// </summary>
        Appearance Appearance { get; set; }

        /// <summary>
        /// Gets or sets the makers visibility.
        /// </summary>
        bool Marked { get; set; }

        /// <summary>
        /// Gets the parent shape, if this is a composite shape.
        /// </summary>
        IShape Parent { get; }

        /// <summary>
        /// Gets or sets the context menu.
        /// </summary>
        ContextMenuStrip Menu { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Gets the shape hit position.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns>Hit position.</returns>
        HitPositions HitTest(Point point);

        /// <summary>
        /// Controls if the point is into this.
        /// </summary>
        /// <param name="point">Point to control.</param>
        /// <returns></returns>
		bool Contains(Point point);

        /// <summary>
        /// Controls if the shape is into this.
        /// </summary>
        /// <param name="shape">Shape to control.</param>
        /// <returns></returns>
        bool Contains(IShape shape);

        /// <summary>
        /// Gets markers of geometric form.
        /// </summary>
        /// <returns>Markers.</returns>
        RectangleF[] GetMarkers();

        /// <summary>
        /// Gets grabbers (to resize for example).
        /// </summary>
        /// <returns>Grabbers.</returns>
        Rectangle[] GetGrabbers();

        /// <summary>
        /// Returns the marker index point.
        /// </summary>
        /// <param name="point">Point to control.</param>
        /// <returns>Index point.</returns>
        int GetMarkerIndex(PointF point);

        /// <summary>
        /// Returns the center point of the clicked grabber.
        /// </summary>
        /// <param name="hitPosition">hit position.</param>
        /// <returns>Point of hit position.</returns>
        PointF GetGrabberPoint(HitPositions hitPosition);

        #endregion
    }
}

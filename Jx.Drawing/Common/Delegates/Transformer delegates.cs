using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Drawing;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Notifies any movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    public delegate void MovementHandler(Transformer transformer);

    /// <summary>
    /// Notifies translate movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    /// <param name="offsetX">OffsetX.</param>
    /// <param name="offsetY">OffsetY.</param>
    public delegate void TranslateHandler(Transformer transformer, float offsetX, float offsetY);

    /// <summary>
    /// Notifies scale movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    /// <param name="scaleX">ScaleX.</param>
    /// <param name="scaleY">ScaleY.</param>
    /// <param name="point">Reference point.</param>
    public delegate void ScaleHandler(Transformer transformer, float scaleX, float scaleY, PointF point);

    /// <summary>
    /// Notifies rotate movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    /// <param name="degree">Degree.</param>
    /// <param name="point">Reference point.</param>
    public delegate void RotateHandler(Transformer transformer, float degree, PointF point);

    /// <summary>
    /// Notifies deform movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    /// <param name="indexPoint">Index point.</param>
    /// <param name="newPoint">New point.</param>
    public delegate void DeformHandler(Transformer transformer, int indexPoint, PointF newPoint);

    /// <summary>
    /// Notifies mirror horizontal movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    /// <param name="x">X.</param>
    public delegate void MirrorHorizontalHandler(Transformer transformer, float x);

    /// <summary>
    /// Notifies mirror vertical movement.
    /// </summary>
    /// <param name="transformer">Sender.</param>
    /// <param name="y">Y.</param>
    public delegate void MirrorVerticalHandler(Transformer transformer, float y);
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Drawing;

namespace Globe.Graphics.Bidimensional.Common
{
    #region Appearance

    /// <summary>
    /// Notifies a change in marker dimension.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void MarkerDimensionHandler(Appearance appearance, int oldValue, int newValue);

    /// <summary>
    /// Notifies a change in marker color.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void MarkerColorHandler(Appearance appearance, Color oldValue, Color newValue);

    /// <summary>
    /// Notifies a change in border color.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void BorderColorHandler(Appearance appearance, Color oldValue, Color newValue);

    /// <summary>
    /// Notifies a change in border width.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void BorderWidthHandler(Appearance appearance, float oldValue, float newValue);

    /// <summary>
    /// Notifies a change in grabber dimension.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void GrabberDimensionHandler(Appearance appearance, int oldValue, int newValue);

    /// <summary>
    /// Notifies a change in grabber dimension.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void ActivePenHandler(Appearance appearance, Pen oldValue, Pen newValue);

    #endregion

    #region Polygon Appearance

    /// <summary>
    /// Notifies any change in the appearance of the shape.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    public delegate void AppearanceHandler(Appearance appearance);

    /// <summary>
    /// Notifies a change in background color 1.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void BackgroundColor1Handler(Appearance appearance, Color oldValue, Color newValue);

    /// <summary>
    /// Notifies a change in background color 2.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void BackgroundColor2Handler(Appearance appearance, Color oldValue, Color newValue);

    /// <summary>
    /// Notifies a change in gradient angle.
    /// </summary>
    /// <param name="appearance">Sender.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    public delegate void GradientAngleHandler(Appearance appearance, float oldValue, float newValue);

    #endregion
}

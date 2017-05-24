using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// A shape property is changed.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="changing">Property.</param>
    public delegate void ShapeChangingHandler(IShape shape, object changing);

    /// <summary>
    /// The mouse is clicked down on shape.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">MouseEventArgs.</param>
    public delegate void MouseDownOnShape(IShape shape, IDocument document, MouseEventArgs e);

    /// <summary>
    /// The mouse is clicked up on shape.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">MouseEventArgs.</param>
    public delegate void MouseUpOnShape(IShape shape, IDocument document, MouseEventArgs e);

    /// <summary>
    /// The mouse is moved on shape.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">MouseEventArgs.</param>
    public delegate void MouseMoveOnShape(IShape shape, IDocument document, MouseEventArgs e);

    /// <summary>
    /// The mouse is clicked on shape.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">MouseEventArgs.</param>
    public delegate void MouseClickOnShape(IShape shape, IDocument document, MouseEventArgs e);

    /// <summary>
    /// The mouse is double clicked on shape.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">MouseEventArgs.</param>
    public delegate void MouseDoubleClickOnShape(IShape shape, IDocument document, MouseEventArgs e);

    /// <summary>
    /// The mouse is wheel on shape.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">MouseEventArgs.</param>
    public delegate void MouseWheel(IShape shape, IDocument document, MouseEventArgs e);

    /// <summary>
    /// The shape Paint function is called.
    /// </summary>
    /// <param name="shape">Shape.</param>
    /// <param name="document">Document.</param>
    /// <param name="e">PaintEventArgs.</param>
    public delegate void PaintOnShape(IShape shape, IDocument document, PaintEventArgs e);	
}

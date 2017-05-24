using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Abstract tool class
    /// </summary>
    public abstract class Tool : IActions
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tool()
        {
        }

        #endregion

        #region Properties

        bool _mousePressed = false;
        /// <summary>
        /// Gets (sets protected) the mouse button state
        /// </summary>
        public bool MousePressed
        {
            get { return _mousePressed; }
            protected set { _mousePressed = value; }
        }

        Point _mouseDownPoint = Point.Empty;
        /// <summary>
        /// Gets or sets the mouse down point
        /// </summary>
        public Point MouseDownPoint
        {
            get { return _mouseDownPoint; }
            set { _mouseDownPoint = value; }
        }

        Point _mouseUpPoint = Point.Empty;
        /// <summary>
        /// Gets or sets the mouse up point
        /// </summary>
        public Point MouseUpPoint
        {
            get { return _mouseUpPoint; }
            set { _mouseUpPoint = value; }
        }

        Ghost _ghost = new Ghost();
        /// <summary>
        /// Gets or sets the ghost to move shapes
        /// </summary>
        protected Ghost Ghost
        {
            get { return _ghost; }
            set { _ghost = value; }
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Mouse down function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
        virtual public void MouseDown(IDocument document, MouseEventArgs e)
        {
            _mousePressed = true;
            _mouseDownPoint = e.Location;

            foreach (IShape shape in document.Shapes)
                shape.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
		virtual public void MouseUp(IDocument document, MouseEventArgs e)
        {
            _mousePressed = false;
            _mouseUpPoint = e.Location;

            foreach (IShape shape in document.Shapes)
                shape.MouseUp(document, e);
        }

        /// <summary>
        /// Mouse click function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
        virtual public void MouseClick(IDocument document, MouseEventArgs e)
        {
            foreach (IShape shape in document.Shapes)
                shape.MouseClick(document, e);
        }

        /// <summary>
        /// Mouse double click function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
        virtual public void MouseDoubleClick(IDocument document, MouseEventArgs e)
        {
            foreach (IShape shape in document.Shapes)
                shape.MouseDoubleClick(document, e);
        }

        /// <summary>
        /// Mouse move function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
		virtual public void MouseMove(IDocument document, MouseEventArgs e)
        {
            UpdateCursor(document, document.Shapes, e.Location);

            foreach (IShape shape in document.Shapes)
                shape.MouseMove(document, e);
        }

        /// <summary>
        /// Mouse wheel function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">MouseEventArgs</param>
		virtual public void MouseWheel(IDocument document, MouseEventArgs e)
        {
            foreach (IShape shape in document.Shapes)
                shape.MouseWheel(document, e);
        }

        /// <summary>
        /// Paint function
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="e">PaintEventArgs</param>
		virtual public void Paint(IDocument document, PaintEventArgs e)
        {
            foreach (IShape shape in document.Shapes)
                shape.Paint(document, e);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Updates the cursor during the tool actions
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel</param>
        /// <param name="shapes">Shapes to manage</param>
        /// <param name="point">Mouse point</param>
        /// <returns></returns>
        virtual public bool UpdateCursor(IDocument document, ShapeCollection shapes, Point point)
        {
            return false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Selects shapes.
    /// </summary>
    public class Select : Tool
    {
        #region Events and Delegates

        /// <summary>
        /// Selected shapes.
        /// </summary>
        /// <param name="tool">Tool that performs the selection action.</param>
        /// <param name="shapes">Selected shapes.</param>
        public delegate void OnSelectedShapes(Tool tool, ShapeCollection shapes);

        /// <summary>
        /// Fires when shapes are selected.
        /// </summary>
        public event OnSelectedShapes SelectedShapes;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Select()
        {
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Mouse down function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseDown(IDocument document, MouseEventArgs e)
        {
            base.MouseDown(document, e);

            if (SelectShape(document.Shapes, e.Location) == HitPositions.None)
                Select.UnselectAll(document.Shapes);

            if (SelectedShapes != null)
                SelectedShapes(this, Select.GetSelectedShapes(document.Shapes));

            document.GridManager.SnapToGrid(Select.GetSelectedShapes(document.Shapes));
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            base.MouseUp(document, e);

            document.ActiveCursor = Cursors.Default;
        }

        #endregion

        #region Properties

        static IShape _lastSelectedShape = null;
        /// <summary>
        /// Gets or sets the last selected shapes.
        /// </summary>
        static public IShape LastSelectedShape
        {
            get { return _lastSelectedShape; }
            set { _lastSelectedShape = value; }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Selects all shapes.
        /// </summary>
        /// <param name="shapes">Shapes to select.</param>
        public static void SelectAll(ShapeCollection shapes)
        {
            foreach (IShape shape in shapes)
                shape.Selected = true;
        }

        /// <summary>
        /// Unselects all shapes.
        /// </summary>
        /// <param name="shapes">Shapes to unselect.</param>
        public static void UnselectAll(ShapeCollection shapes)
        {
            foreach (IShape shape in shapes)
                shape.Selected = false;
        }

        /// <summary>
        /// Gets selected shapes in the collection.
        /// </summary>
        /// <param name="shapes">Shape collection.</param>
        /// <returns>Selected shapes.</returns>
        public static ShapeCollection GetSelectedShapes(ShapeCollection shapes)
        {
            ShapeCollection selectedShapes = new ShapeCollection();

            foreach (IShape shape in shapes)
            {
                if (shape.Selected)
                    selectedShapes.Add(shape);
            }

            return selectedShapes;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Selects the shape contains point.
        /// </summary>
        /// <param name="shapes">Shape collection.</param>
        /// <param name="point">Point to check.</param>
        /// <returns>Hit position of the selected shape.</returns>
        protected HitPositions SelectShape(ShapeCollection shapes, Point point)
        {
            if (Control.ModifierKeys != Keys.Control)
                Select.UnselectAll(shapes);

            HitPositions hitPosition = HitPositions.None;

            for (int i = shapes.Count - 1; i >= 0; i--)
            {
                IShape shape = shapes[i];

                hitPosition = shape.HitTest(point);
                if (hitPosition != HitPositions.None)
                {
                    shapes.BringToFront(shape);
                    shape.Selected = true;
                    _lastSelectedShape = shape;

                    return hitPosition;
                }
            }

            return HitPositions.None;
        }

        #endregion
    }
}

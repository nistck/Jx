using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Jx.Graphics.Bidimensional.Common;

namespace Jx.Graphics.Bidimensional.Base
{
    /// <summary>
    /// Selects more shapes.
    /// </summary>
    public class MultiSelect : Draw
    {
        #region Events and Delegates

        /// <summary>
        /// Function to notify shapes selection.
        /// </summary>
        /// <param name="tool">Tool that notifies.</param>
        /// <param name="shapes">Shapes.</param>
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
        public MultiSelect()
        {
            Ghost = new MultiSelectGhost();
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
            if (Control.ModifierKeys != Keys.Control && e.Button != MouseButtons.Right)
                Select.UnselectAll(document.Shapes);

            base.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
			SelectIntersectedShapes(document.Shapes);

            if (SelectedShapes != null)
                SelectedShapes(this, Select.GetSelectedShapes(document.Shapes));

            base.MouseUp(document, e);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            Ghost.Paint(document, e);
        }

        #endregion

        #region Private Functions

        private void SelectIntersectedShapes(ShapeCollection shapes)
		{
			foreach (IShape shape in shapes)
			{
                if (Ghost.Geometric.GetBounds().IntersectsWith(                    
                    shape.Geometric.GetBounds()))
                    shape.Selected = true;
            }
        }

        #endregion
    }
}

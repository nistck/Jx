using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Jx.Drawing.Common;

namespace Jx.Drawing.Base
{
    /// <summary>
    /// Manages shapes groups.
    /// </summary>
    public static class GroupEngine
    {
        #region Group

        /// <summary>
        /// Group in a single shape the selected shapes.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void Group(IDocument document)
        {
            ShapeCollection selectedShapes = Select.GetSelectedShapes(document.Shapes);

            CompositeShape group = new CompositeShape();
            group.Selected = true;

            foreach (IShape shape in selectedShapes)
            {
                shape.Selected = false;
                group.Shapes.Add(shape);
                document.Shapes.Remove(shape);
            }

            document.Shapes.Add(group);
        }

        /// <summary>
        /// Ungroup in a single shape all selected shapes.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void Ungroup(IDocument document)
        {
            ShapeCollection selectedShapes = Select.GetSelectedShapes(document.Shapes);

            foreach (IShape shape in selectedShapes)
            {
                CompositeShape group = shape as CompositeShape;
                if (group == null)
                    continue;

                document.Shapes.Remove(group);

                foreach (IShape grouppedShape in group.Shapes)
                {
                    grouppedShape.Selected = true;
                    document.Shapes.Add(grouppedShape);
                }

                while (group.Shapes.Count != 0)
                    group.Shapes.RemoveAt(0);
            }
        }

        #endregion

        #region Layout

        /// <summary>
        /// Aligns left all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void AlignLefts(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Location = new PointF(Select.LastSelectedShape.Location.X, shape.Location.Y);
        }

        /// <summary>
        /// Aligns right all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void AlignRights(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Location = new PointF(Select.LastSelectedShape.Location.X + Select.LastSelectedShape.Dimension.Width - shape.Dimension.Width, shape.Location.Y);
        }

        /// <summary>
        /// Aligns top all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void AlignTops(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Location = new PointF(shape.Location.X, Select.LastSelectedShape.Location.Y);
        }

        /// <summary>
        /// Aligns bottom all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void AlignBottoms(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Location = new PointF(shape.Location.X, Select.LastSelectedShape.Location.Y + Select.LastSelectedShape.Dimension.Height - shape.Dimension.Height);
        }

        /// <summary>
        /// Makes same width of all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void MakeSameWidth(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Dimension = new SizeF(Select.LastSelectedShape.Dimension.Width, shape.Dimension.Height);
        }

        /// <summary>
        /// Makes same height of all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void MakeSameHeight(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Dimension = new SizeF(shape.Dimension.Width, Select.LastSelectedShape.Dimension.Height);
        }

        /// <summary>
        /// Makes same size of all selected shapes relative to last selected shape location.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        public static void MakeSameSize(IDocument document)
        {
            if (Select.LastSelectedShape == null)
                return;

            foreach (IShape shape in Select.GetSelectedShapes(document.Shapes))
                if (shape != Select.LastSelectedShape)
                    shape.Dimension = Select.LastSelectedShape.Dimension;
        }

        #endregion
    }
}

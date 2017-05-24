using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

using Jx.Drawing.Common;

namespace Jx.Drawing.Base
{
    /// <summary>
    /// Ghost collection to manage more than one Ghost.
    /// </summary>
    [Serializable]
    public class GhostCollection : Ghost
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GhostCollection()
        {
            this.Transformer.TranslateOccurred += new TranslateHandler(Transformer_TranslateOccurred);
            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="shapes">ShapeCollection to copy.</param>
        public GhostCollection(ShapeCollection shapes)
        {
            foreach (IShape shape in shapes)
            {
                Ghost ghost = new Ghost(shape);
                _ghosts.Add(ghost);
            }

            this.Transformer.TranslateOccurred += new TranslateHandler(Transformer_TranslateOccurred);
            this.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the GhostCollection.
        /// </summary>
        /// <returns>New GhostCollection.</returns>
        public override object Clone()
        {
            GhostCollection clonedCollection = new GhostCollection();
            foreach (IShape shape in _ghosts)
                clonedCollection.Ghosts.Add(shape.Clone() as IShape);

            return clonedCollection;
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
            foreach (IShape shape in _ghosts)
                shape.MouseDown(document, e);
        }

        /// <summary>
        /// Mouse up function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseUp(IDocument document, MouseEventArgs e)
        {
            foreach (IShape shape in _ghosts)
            {
                if (ShapeMouseUp != null)
                    ShapeMouseUp(shape, document, e);

                shape.MouseUp(document, e);
            }

            if (ShapeMouseUp != null)
                ShapeMouseUp(this, document, e);
        }

        /// <summary>
        /// Mouse move function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">MouseEventArgs.</param>
        public override void MouseMove(IDocument document, MouseEventArgs e)
        {
            Selected = true;

            foreach (IShape shape in _ghosts)
                shape.MouseMove(document, e);
        }

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            foreach (IShape shape in _ghosts)
                shape.Paint(document, e);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when mouse up on shape
        /// </summary>
        public override event MouseUpOnShape ShapeMouseUp;

        #endregion

        #endregion

        #region Properties

        ShapeCollection _ghosts = new ShapeCollection();
        /// <summary>
        /// Gets or sets ghosts.
        /// </summary>
        virtual public ShapeCollection Ghosts
        {
            get { return _ghosts; }

            set
            {
                _ghosts.Clear();

                foreach (IShape shape in value)
                {
                    Ghost ghost = new Ghost(shape);
                    _ghosts.Add(ghost);
                }
            }
        }

        #endregion

        #region Private Functions

        void Transformer_TranslateOccurred(Transformer transformer, float offsetX, float offsetY)
        {
            foreach (IShape shape in _ghosts)
                shape.Transformer.Translate(offsetX, offsetY);
        }

        void Transformer_RotateOccurred(Transformer transformer, float degree, System.Drawing.PointF point)
        {
            foreach (IShape shape in _ghosts)
                shape.Transformer.Rotate(degree, point);
        }

        #endregion
    }
}

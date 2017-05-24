using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Jx.Xml.Serialization;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Manages all shapes in the drawing panel and their particular movements (translate, scale etc.).
    /// </summary>
    [XmlClassSerializable("shapes")]
    public class ShapeCollectionEx : ShapeCollection
    {
        #region Events and Delegates

        /// <summary>
        /// Notifies translate movement.
        /// </summary>
        virtual public event Jx.Graphics.Bidimensional.Common.TranslateHandler ShapeTranslateOccurred;

        /// <summary>
        /// Notifies scale movement.
        /// </summary>
        virtual public event Jx.Graphics.Bidimensional.Common.ScaleHandler ShapeScaleOccurred;

        /// <summary>
        /// Notifies rotate movement.
        /// </summary>
        virtual public event Jx.Graphics.Bidimensional.Common.RotateHandler ShapeRotateOccurred;

        /// <summary>
        /// Notifies deform movement.
        /// </summary>
        virtual public event Jx.Graphics.Bidimensional.Common.DeformHandler ShapeDeformOccurred;

        /// <summary>
        /// Notifies mirror horizontal movement.
        /// </summary>
        virtual public event Jx.Graphics.Bidimensional.Common.MirrorHorizontalHandler ShapeMirrorHorizontalOccurred;

        /// <summary>
        /// Notifies mirror vertical movement.
        /// </summary>
        virtual public event Jx.Graphics.Bidimensional.Common.MirrorVerticalHandler ShapeMirrorVerticalOccurred;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShapeCollectionEx()
        {
        }

        #endregion

        #region ICloneable Interface

        /// <summary>
        /// Clones the collection and its content.
        /// </summary>
        /// <returns>Cloned object.</returns>
        override public object Clone()
        {
            ShapeCollectionEx clonedCollection = new ShapeCollectionEx();
            foreach (IShape shape in this)
                clonedCollection.Add(shape.Clone() as IShape);

            return clonedCollection;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Called when a shape is inserted.
        /// </summary>
        /// <param name="index">Inserting index.</param>
        /// <param name="item">Shape inserted.</param>
        protected override void InsertItem(int index, IShape item)
        {
            //if (item.Parent != null)
            //    throw new ApplicationException("Must first removes item from Shape Container!");

            base.InsertItem(index, item);

            item.Transformer.TranslateOccurred += new TranslateHandler(Transformer_TranslateOccurred);
            item.Transformer.ScaleOccurred += new ScaleHandler(Transformer_ScaleOccurred);
            item.Transformer.RotateOccurred += new RotateHandler(Transformer_RotateOccurred);
            item.Transformer.DeformOccurred += new DeformHandler(Transformer_DeformOccurred);
            item.Transformer.MirrorHorizontalOccurred += new MirrorHorizontalHandler(Transformer_MirrorHorizontalOccurred);
            item.Transformer.MirrorVerticalOccurred += new MirrorVerticalHandler(Transformer_MirrorVerticalOccurred);
        }

        /// <summary>
        /// Called when a shape is removed.
        /// </summary>
        /// <param name="index">Removing shape.</param>
        protected override void RemoveItem(int index)
        {
            IShape item = this[index];

            base.RemoveItem(index);

            item.Transformer.TranslateOccurred -= new TranslateHandler(Transformer_TranslateOccurred);
            item.Transformer.ScaleOccurred -= new ScaleHandler(Transformer_ScaleOccurred);
            item.Transformer.RotateOccurred -= new RotateHandler(Transformer_RotateOccurred);
            item.Transformer.DeformOccurred -= new DeformHandler(Transformer_DeformOccurred);
            item.Transformer.MirrorHorizontalOccurred -= new MirrorHorizontalHandler(Transformer_MirrorHorizontalOccurred);
            item.Transformer.MirrorVerticalOccurred -= new MirrorVerticalHandler(Transformer_MirrorVerticalOccurred);
        }

        #endregion

        #region Private Functions

        void Transformer_TranslateOccurred(Transformer transformer, float offsetX, float offsetY)
        {
            if (ShapeTranslateOccurred != null)
                ShapeTranslateOccurred(transformer, offsetX, offsetY);
        }

        void Transformer_ScaleOccurred(Transformer transformer, float scaleX, float scaleY, System.Drawing.PointF point)
        {
            if (ShapeScaleOccurred != null)
                ShapeScaleOccurred(transformer, scaleX, scaleY, point);
        }

        void Transformer_RotateOccurred(Transformer transformer, float degree, System.Drawing.PointF point)
        {
            if (ShapeRotateOccurred != null)
                ShapeRotateOccurred(transformer, degree, point);
        }

        void Transformer_DeformOccurred(Transformer transformer, int indexPoint, System.Drawing.PointF newPoint)
        {
            if (ShapeDeformOccurred != null)
                ShapeDeformOccurred(transformer, indexPoint, newPoint);
        }

        void Transformer_MirrorHorizontalOccurred(Transformer transformer, float x)
        {
            if (ShapeMirrorHorizontalOccurred != null)
                ShapeMirrorHorizontalOccurred(transformer, x);
        }

        void Transformer_MirrorVerticalOccurred(Transformer transformer, float y)
        {
            if (ShapeMirrorVerticalOccurred != null)
                ShapeMirrorVerticalOccurred(transformer, y);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Jx.Drawing.Serialization.XML;

namespace Jx.Drawing.Common
{
    /// <summary>
    /// Manages all shapes in the drawing panel.
    /// </summary>
    [XmlClassSerializable("shapes")]
    public class ShapeCollection : Collection<IShape>, ICollection, ICloneable
    {
        #region Events and Delegates

        /// <summary>
        /// Inserted shape.
        /// </summary>
        /// <param name="shape">Shape inserted.</param>
        /// <param name="index">Index of the collection.</param>
        public delegate void OnInsertedItem(IShape shape, int index);

        /// <summary>
        /// Fires when a shape is inserted.
        /// </summary>
        virtual public event OnInsertedItem InsertedItem;

        /// <summary>
        /// Removed shape.
        /// </summary>
        /// <param name="shape">Shape removed.</param>
        /// <param name="index">Index of the collection.</param>
        public delegate void OnRemovedItem(IShape shape, int index);

        /// <summary>
        /// Fires when a shape is removed.
        /// </summary>
        virtual public event OnRemovedItem RemovedItem;

        /// <summary>
        /// Fires when a shape property is changed.
        /// </summary>
        virtual public event Jx.Drawing.Common.ShapeChangingHandler ShapeChanged;

        /// <summary>
        /// Notifies anyone movement.
        /// </summary>
        virtual public event Jx.Drawing.Common.MovementHandler ShapeMovementOccurred;

        /// <summary>
        /// Notifies anyone appearance changing.
        /// </summary>
        virtual public event Jx.Drawing.Common.AppearanceHandler ShapeAppearanceChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShapeCollection()
        {
        }

        #endregion

        #region ICloneable Interface

        /// <summary>
        /// Clones the collection and its content.
        /// </summary>
        /// <returns>Cloned object.</returns>
        virtual public object Clone()
        {
            ShapeCollection clonedCollection = new ShapeCollection();
            foreach (IShape shape in this)
                clonedCollection.Add(shape.Clone() as IShape);

            return clonedCollection;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Add a ShapeCollection in the current collection.
        /// </summary>
        /// <param name="shapes">ShapeCollection to add.</param>
        public void AddRange(ShapeCollection shapes)
        {
            foreach (IShape shape in shapes)
                Add(shape);
        }

        /// <summary>
        /// Brings to front the shape.
        /// </summary>
        /// <param name="shape">Shape to move.</param>
        public void BringToFront(IShape shape)
        {
            if (Remove(shape))
                Add(shape);
        }

        /// <summary>
        /// Sends to back the shape.
        /// </summary>
        /// <param name="shape">Shape to move.</param>
        public void SendToBack(IShape shape)
        {
            if (Remove(shape))
                Insert(0, shape);
        }

        /// <summary>
        /// Transforms all internal shapes into an array of objects.
        /// </summary>
        /// <param name="shapes">Collection to transform.</param>
        /// <returns>Array of object.</returns>
        public static object[] ToObjects(ShapeCollection shapes)
        {
            if (shapes.Count == 0)
                return null;

            object[] objectShapes = new object[shapes.Count];
            for (int i = 0; i < shapes.Count; i++)
                objectShapes[i] = shapes[i];

            return objectShapes;
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

            if (InsertedItem != null)
                InsertedItem(item, index);

            item.ShapeChanged += new Jx.Drawing.Common.ShapeChangingHandler(item_ShapeChanged);
            item.Transformer.MovementOccurred += new MovementHandler(Transformer_MovementOccurred);
            item.Appearance.AppearanceChanged += new AppearanceHandler(Appearance_AppearanceOccurred);
        }

        /// <summary>
        /// Called when a shape is removed.
        /// </summary>
        /// <param name="index">Removing shape.</param>
        protected override void RemoveItem(int index)
        {
            IShape item = this[index];

            base.RemoveItem(index);

            if (RemovedItem != null)
                RemovedItem(item, index);

            item.ShapeChanged -= new Jx.Drawing.Common.ShapeChangingHandler(item_ShapeChanged);
            item.Transformer.MovementOccurred -= new MovementHandler(Transformer_MovementOccurred);
            item.Appearance.AppearanceChanged -= new AppearanceHandler(Appearance_AppearanceOccurred);
        }

        #endregion

        #region Private Functions

        void item_ShapeChanged(IShape shape, object changing)
        {
            if (ShapeChanged != null)
                ShapeChanged(shape, changing);
        }

        void Transformer_MovementOccurred(Transformer transformer)
        {
            if (ShapeMovementOccurred != null)
                ShapeMovementOccurred(transformer);
        }

        void Appearance_AppearanceOccurred(Appearance appearance)
        {
            if (ShapeAppearanceChanged != null)
                ShapeAppearanceChanged(appearance);
        }

        #endregion
    }
}

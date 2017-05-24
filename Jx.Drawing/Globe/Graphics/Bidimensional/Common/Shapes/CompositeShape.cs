using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Jx.Xml.Serialization;

namespace Jx.Graphics.Bidimensional.Common
{
    /// <summary>
    /// Manages more shapes like one only.
    /// </summary>
    [XmlClassSerializable("compositeShape")]
    public class CompositeShape : Shape
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompositeShape()
        {
            Transformer = new CompositeTransformer(this);

            _shapes.InsertedItem += new ShapeCollection.OnInsertedItem(_shapes_InsertedItem);
            _shapes.RemovedItem += new ShapeCollection.OnRemovedItem(_shapes_RemovedItem);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="compositeShape">CompositeShape to copy.</param>
        public CompositeShape(CompositeShape compositeShape) : base(compositeShape)
        {
            Transformer = new CompositeTransformer(this);

            _shapes.InsertedItem += new ShapeCollection.OnInsertedItem(_shapes_InsertedItem);
            _shapes.RemovedItem += new ShapeCollection.OnRemovedItem(_shapes_RemovedItem);

            Geometric.Reset();

            foreach (IShape shape in compositeShape.Shapes)
                _shapes.Add(shape.Clone() as IShape);
        }

        #endregion

        #region IShape Interface

        #region ICloneable Interface

        /// <summary>
        /// Clones the shape.
        /// </summary>
        /// <returns>New shape.</returns>
        public override object Clone()
        {
            return new CompositeShape(this);
        }

        #endregion

        #region IActions Interface

        /// <summary>
        /// Paint function.
        /// </summary>
        /// <param name="document">Informations transferred from DrawingPanel.</param>
        /// <param name="e">PaintEventArgs.</param>
        public override void Paint(IDocument document, PaintEventArgs e)
        {
            foreach (IShape shape in _shapes)
            {
                // This is bad, but for serialization is necessary set the shape
                shape.Appearance.Shape = shape;
                shape.Appearance.Paint(document, e);
            }

            Appearance.Shape = this;
            Appearance.Paint(document, e);
        }

        #endregion

        #endregion

        #region Properties

        ShapeCollection _shapes = new ShapeCollection();
        /// <summary>
        /// Gets the shapes in the composite shape.
        /// </summary>
        [XmlFieldSerializable("shapes")]
        public ShapeCollection Shapes
        {
            get { return _shapes; }
            private set
            {
                if (value == null)
                    return;

                _shapes.AddRange(value);
            }
        }

        bool _movementContentBlocked = false;
        /// <summary>
        /// Gets or sets the blocking of shapes.
        /// </summary>
        [XmlFieldSerializable("movementContentBlocked")]
        public bool MovementContentBlocked
        {
            get { return _movementContentBlocked; }
            set { _movementContentBlocked = value; }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Called when a shape is inserted.
        /// </summary>
        /// <param name="shape">Inserted shape.</param>
        /// <param name="index">Inserting index.</param>
        void _shapes_InsertedItem(IShape shape, int index)
        {
            (shape as Shape).Parent = this;

            Geometric.AddPath(shape.Geometric, false);
        }

        /// <summary>
        /// Called when a shape is removed.
        /// </summary>
        /// <param name="shape">Removed shape.</param>
        /// <param name="index">Removing index.</param>
        void _shapes_RemovedItem(IShape shape, int index)
        {
            (shape as Shape).Parent = null;

            Geometric.Reset();

            foreach (IShape grouppedShape in _shapes)
                Geometric.AddPath(grouppedShape.Geometric, false);
        }

        #endregion
    }
}

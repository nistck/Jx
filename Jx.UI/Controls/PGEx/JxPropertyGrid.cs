using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using System.Drawing.Design;
using System.Globalization;
using System.Linq;
 

namespace Jx.UI.Controls.PGEx
{

    public partial class JxPropertyGrid : PropertyGrid
	{
        private const int WM_PARENTNOTIFY = 528;
        private const int WM_LBUTTONDOWN = 513;

        #region "Protected variables and objects"

        // Internal PropertyGrid Controls
        protected object internalPropertyGridView;
		protected object internalHotCommands;
		protected object internalDocComment;
		protected ToolStrip internalToolStrip;
		
		// Internal PropertyGrid Fields
		protected Label oDocCommentTitle;
		protected Label oDocCommentDescription;
		protected FieldInfo oPropertyGridEntries;

        protected FieldInfo internalFieldLabelRatio;

        // Properties variables
        protected bool bAutoSizeProperties;
        protected bool bDrawFlatToolbar;
        #endregion

        private DateTime lastTimeClick;
        private GridItem selectedObject;
        public event EventHandler GridItemDoubleClick;

        #region "Public Functions"
        public JxPropertyGrid()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			
			// Add any initialization after the InitializeComponent() call.
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
 
			// Attach internal controls
			internalPropertyGridView = base.GetType().BaseType.InvokeMember("gridView", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
			internalHotCommands = base.GetType().BaseType.InvokeMember("hotcommands", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
			internalToolStrip = (ToolStrip) base.GetType().BaseType.InvokeMember("toolStrip", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
			internalDocComment = base.GetType().BaseType.InvokeMember("doccomment", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);

            if (internalPropertyGridView != null)
                internalFieldLabelRatio = internalPropertyGridView.GetType().GetField("labelRatio"); 

			// Attach DocComment internal fields
			if (internalDocComment != null)
			{
				oDocCommentTitle = (Label)internalDocComment.GetType().InvokeMember("m_labelTitle", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, internalDocComment, null);
				oDocCommentDescription = (Label)internalDocComment.GetType().InvokeMember("m_labelDesc", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, internalDocComment, null);
			}
			
			// Attach PropertyGridView internal fields
			if (internalPropertyGridView != null)
			{
				oPropertyGridEntries = internalPropertyGridView.GetType().GetField("allGridEntries", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			}
			
			// Apply Toolstrip style
			if (internalToolStrip != null)
			{
				ApplyToolStripRenderMode(bDrawFlatToolbar);
			} 
        }

        [Description("列宽参考值")]
        public double LabelRatio
        {
            get {
                if (internalFieldLabelRatio == null)
                    return 2.0;

                object fv = internalFieldLabelRatio.GetValue(internalPropertyGridView);
                return (double)fv;
            }
            set {
                if (internalFieldLabelRatio == null)
                    return;

                internalFieldLabelRatio.SetValue(internalPropertyGridView, value);
            }
        }
		
		public void MoveSplitterTo(int x)
		{
            internalPropertyGridView.GetType().InvokeMember("MoveSplitterTo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, internalPropertyGridView, new object[] { x });
        }
		
		public override void Refresh()
		{
			base.Refresh();
			if (bAutoSizeProperties)
			{
				AutoSizeSplitter(32);
			}
		}
		
		public void SetComment(string title, string description)
		{
            MethodInfo method = internalDocComment.GetType().GetMethod("SetComment");
            method.Invoke(internalDocComment, new object[] { title, description });
			//internalDocComment.SetComment(title, description);
		}
		
		#endregion
		
		#region "Protected Functions"
		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize(e);
			if (bAutoSizeProperties)
			{
				AutoSizeSplitter(32);
			}
		}
		
		protected void AutoSizeSplitter(int RightMargin)
		{
			
			GridItemCollection oItemCollection =  (System.Windows.Forms.GridItemCollection) oPropertyGridEntries.GetValue(internalPropertyGridView);
			if (oItemCollection == null)
			{
				return;
			}
			System.Drawing.Graphics oGraphics = System.Drawing.Graphics.FromHwnd(this.Handle);
			int CurWidth = 0;
			int MaxWidth = 0;
			
			foreach (GridItem oItem in oItemCollection)
			{
				if (oItem.GridItemType == GridItemType.Property)
				{
					CurWidth =  (int) oGraphics.MeasureString(oItem.Label, this.Font).Width + RightMargin;
					if (CurWidth > MaxWidth)
					{
						MaxWidth = CurWidth;
					}
				}
			}
			
			MoveSplitterTo(MaxWidth);
		}
		protected void ApplyToolStripRenderMode(bool value)
		{
			if (value)
			{
				internalToolStrip.Renderer = new ToolStripSystemRenderer();
			}
			else
			{
                ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer(new JxCustomColorScheme());
                renderer.RoundedEdges = false;
				internalToolStrip.Renderer = renderer;
			}
		}
        #endregion

        #region "Properties"

        #region 只读!
        private bool _readOnly;
        private bool _selectionChangedInternally;
        private ReadOnlyCollection<object> _originalSelectedObjects;
        
        private readonly Dictionary<object, TypeDescriptionProvider> typeDescriptionProviderDic = new Dictionary<object, TypeDescriptionProvider>();

        /// <summary>
        /// Represents the special <see cref="T:System.ComponentModel.TypeConverter"/> that makes every property of the applicable object as read only.
        /// </summary>
        private class ReadOnlyConverter : TypeConverter
        {
            private TypeConverter _originalConverter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadOnlyConverter"/> class.
            /// </summary>
            /// <param name="originalConverter">The original converter.</param>
            public ReadOnlyConverter(TypeConverter originalConverter)
            {
                _originalConverter = originalConverter;
            }

            /// <summary>
            /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
            /// <returns>
            /// true if this converter can perform the conversion; otherwise, false.
            /// </returns>
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return _originalConverter.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            /// Returns whether this converter can convert the object to the specified type, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
            /// <returns>
            /// true if this converter can perform the conversion; otherwise, false.
            /// </returns>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return _originalConverter.CanConvertTo(context, destinationType);
            }

            /// <summary>
            /// Converts the given object to the type of this converter, using the specified context and culture information.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
            /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
            /// <returns>
            /// An <see cref="T:System.Object"/> that represents the converted value.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return _originalConverter.ConvertFrom(context, culture, value);
            }

            /// <summary>
            /// Converts the given value object to the specified type, using the specified context and culture information.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
            /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
            /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
            /// <returns>
            /// An <see cref="T:System.Object"/> that represents the converted value.
            /// </returns>
            /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
            /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return _originalConverter.ConvertTo(context, culture, value, destinationType);
            }

            /// <summary>
            /// Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter"/> is associated with, using the specified context, given a set of property values for the object.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary"/> of new property values.</param>
            /// <returns>
            /// An <see cref="T:System.Object"/> representing the given <see cref="T:System.Collections.IDictionary"/>, or null if the object cannot be created. This method always returns null.
            /// </returns>
            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
            {
                return _originalConverter.CreateInstance(context, propertyValues);
            }

            /// <summary>
            /// Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> to create a new value, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <returns>
            /// true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> to create a new value; otherwise, false.
            /// </returns>
            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
            {
                return _originalConverter.GetCreateInstanceSupported(context);
            }

            /// <summary>
            /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
            /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
            /// <remarks>
            /// This is the point where instead of the actual properties of the object a collection of special, always read only, <see cref="T:System.ComponentModel.PropertyDescriptor"/> is returned.
            /// </remarks>
            /// <returns>
            /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or null if there are no properties.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                PropertyDescriptorCollection originalProperties = _originalConverter.GetProperties(context, value, attributes);
                PropertyDescriptor[] readOnlyProperties = new PropertyDescriptor[originalProperties.Count];

                for (int i = 0; i < originalProperties.Count; i++)
                {
                    readOnlyProperties[i] = new ReadOnlyPropertyDescriptor(originalProperties[i]);
                }

                return new PropertyDescriptorCollection(readOnlyProperties);
            }

            /// <summary>
            /// Returns whether this object supports properties, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <returns>
            /// true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, false.
            /// </returns>
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return _originalConverter.GetPropertiesSupported(context);
            }

            /// <summary>
            /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
            /// <returns>
            /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
            /// </returns>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return _originalConverter.GetStandardValues(context);
            }

            /// <summary>
            /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive list of possible values, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <returns>
            /// true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exhaustive list of possible values; false if other values are possible.
            /// </returns>
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return _originalConverter.GetStandardValuesExclusive(context);
            }

            /// <summary>
            /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <returns>
            /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, false.
            /// </returns>
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return _originalConverter.GetStandardValuesSupported(context);
            }

            /// <summary>
            /// Returns whether the given value object is valid for this type and for the specified context.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="value">The <see cref="T:System.Object"/> to test for validity.</param>
            /// <returns>
            /// true if the specified value is valid for this object; otherwise, false.
            /// </returns>
            public override bool IsValid(ITypeDescriptorContext context, object value)
            {
                return _originalConverter.IsValid(context, value);
            }
        }

        private class ReadOnlyWrapper : ICustomTypeDescriptor
        {
            private object _wrappedObject;
            private ReadOnlyConverter _typeConverter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadOnlyWrapper"/> class.
            /// </summary>
            /// <param name="wrappedObject">The selected object.</param>
            public ReadOnlyWrapper(object wrappedObject)
            {
                _wrappedObject = wrappedObject;
                _typeConverter = new ReadOnlyConverter(TypeDescriptor.GetConverter(_wrappedObject));
            }

            /// <summary>
            /// Gets the wrapped object.
            /// </summary>
            /// <value>The wrapped object.</value>
            public object WrappedObject
            {
                get { return _wrappedObject; }
            }

            #region ICustomTypeDescriptor Members

            public AttributeCollection GetAttributes()
            {
                return TypeDescriptor.GetAttributes(_wrappedObject);
            }

            public string GetClassName()
            {
                return TypeDescriptor.GetClassName(_wrappedObject);
            }

            public string GetComponentName()
            {
                return TypeDescriptor.GetComponentName(_wrappedObject);
            }

            public TypeConverter GetConverter()
            {
                return _typeConverter;
            }

            public EventDescriptor GetDefaultEvent()
            {
                return TypeDescriptor.GetDefaultEvent(_wrappedObject);
            }

            public PropertyDescriptor GetDefaultProperty()
            {
                return TypeDescriptor.GetDefaultProperty(_wrappedObject);
            }

            public object GetEditor(Type editorBaseType)
            {
                return TypeDescriptor.GetEditor(_wrappedObject, editorBaseType);
            }

            public EventDescriptorCollection GetEvents(Attribute[] attributes)
            {
                return TypeDescriptor.GetEvents(_wrappedObject, attributes);
            }

            public EventDescriptorCollection GetEvents()
            {
                return TypeDescriptor.GetEvents(_wrappedObject);
            }

            public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                PropertyDescriptorCollection originalProperties = TypeDescriptor.GetProperties(_wrappedObject, attributes);
                PropertyDescriptor[] readOnlyProperties = new PropertyDescriptor[originalProperties.Count];

                for (int i = 0; i < originalProperties.Count; i++)
                {
                    readOnlyProperties[i] = new ReadOnlyPropertyDescriptor(originalProperties[i]);
                }

                return new PropertyDescriptorCollection(readOnlyProperties);
            }

            public PropertyDescriptorCollection GetProperties()
            {
                return TypeDescriptor.GetProperties(_wrappedObject);
            }

            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                return _wrappedObject;
            }

            #endregion
        }

        private class ReadOnlyPropertyDescriptor : PropertyDescriptor
        {
            private PropertyDescriptor _originalPropertyDescriptor;
            private ReadOnlyConverter _typeConverter;

            private static UITypeEditor _editor = new UITypeEditor();

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadOnlyPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="originalPropertyDescriptor">The original property descriptor.</param>
            public ReadOnlyPropertyDescriptor(PropertyDescriptor originalPropertyDescriptor)
                : base(originalPropertyDescriptor)
            {
                _originalPropertyDescriptor = originalPropertyDescriptor;
                _typeConverter = new ReadOnlyConverter(originalPropertyDescriptor.Converter);
            }

            /// <summary>
            /// When overridden in a derived class, returns whether resetting an object changes its value.
            /// </summary>
            /// <param name="component">The component to test for reset capability.</param>
            /// <returns>
            /// true if resetting the component changes its value; otherwise, false.
            /// </returns>
            public override bool CanResetValue(object component)
            {
                return _originalPropertyDescriptor.CanResetValue(component);
            }

            /// <summary>
            /// When overridden in a derived class, gets the type of the component this property is bound to.
            /// </summary>
            /// <value></value>
            /// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
            public override Type ComponentType
            {
                get { return _originalPropertyDescriptor.ComponentType; }
            }

            /// <summary>
            /// Gets the type converter for this property.
            /// </summary>
            /// <value></value>
            /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is used to convert the <see cref="T:System.Type"/> of this property.</returns>
            /// <PermissionSet>
            /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
            /// </PermissionSet>
            public override TypeConverter Converter
            {
                get { return _typeConverter; }
            }

            /// <summary>
            /// Gets an editor of the specified type.
            /// </summary>
            /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
            /// <returns>
            /// An instance of the requested editor type, or null if an editor cannot be found.
            /// </returns>
            public override object GetEditor(Type editorBaseType)
            {
                //return _originalPropertyDescriptor.GetEditor(editorBaseType);
                return _editor;
            }

            /// <summary>
            /// When overridden in a derived class, gets the current value of the property on a component.
            /// </summary>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            /// <returns>
            /// The value of a property for a given component.
            /// </returns>
            public override object GetValue(object component)
            {
                return _originalPropertyDescriptor.GetValue(component);
            }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether this property is read-only.
            /// </summary>
            /// <value></value>
            /// <returns>Always returns true.</returns>
            public override bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// When overridden in a derived class, gets the type of the property.
            /// </summary>
            /// <value></value>
            /// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
            public override Type PropertyType
            {
                get { return _originalPropertyDescriptor.PropertyType; }
            }

            /// <summary>
            /// When overridden in a derived class, resets the value for this property of the component to the default value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be reset to the default value.</param>
            public override void ResetValue(object component)
            {
                _originalPropertyDescriptor.ResetValue(component);
            }

            /// <summary>
            /// When overridden in a derived class, sets the value of the component to a different value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be set.</param>
            /// <param name="value">The new value.</param>
            public override void SetValue(object component, object value)
            {
                _originalPropertyDescriptor.SetValue(component, value);
            }

            /// <summary>
            /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
            /// </summary>
            /// <param name="component">The component with the property to be examined for persistence.</param>
            /// <returns>
            /// true if the property should be persisted; otherwise, false.
            /// </returns>
            public override bool ShouldSerializeValue(object component)
            {
                return _originalPropertyDescriptor.ShouldSerializeValue(component);
            }

            private PropertyInfo propertyInfo
            {
                get {
                    if (_originalPropertyDescriptor == null)
                        return null;

                    PropertyInfo result = _originalPropertyDescriptor.ComponentType.GetProperty(_originalPropertyDescriptor.Name);
                    return result;
                }
            }

            public override string DisplayName
            {
                get
                {
                    if (propertyInfo != null)
                    {
                        JxNameAttribute attrFound = propertyInfo.GetCustomAttribute<JxNameAttribute>();
                        if (attrFound != null && !string.IsNullOrEmpty(attrFound.Name))
                            return attrFound.Name;
                    }
                    return base.DisplayName;
                }
            }

            public override string Category
            {
                get
                {
                    if (propertyInfo != null)
                    {
                        JxNameAttribute attrFound = propertyInfo.DeclaringType.GetCustomAttribute<JxNameAttribute>();
                        if (attrFound != null && !string.IsNullOrEmpty(attrFound.Name))
                        {
                            string categoryInfo = string.Format("{0} ({1})", attrFound.Name, base.Category);
                            return categoryInfo;
                        }
                    }
                    return base.Category;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="P:SelectedObjects"/> is displayed as read only.
        /// </summary>
        /// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                SetReadOnly();
            }
        }

        /// <summary>
        /// Gets the original unwrapped selected object.
        /// </summary>
        /// <remarks>
        /// Use this property instead of <see cref="P:SelectedObject"/> to get the correct unwrapped object that is being displayed.
        /// If the <see cref="ReadOnly"/> property is set to <c>false</c> then the value returned by both this property and <see cref="P:SelectedObject"/> will be the same.
        /// </remarks>
        /// <value>The original selected object.</value>
        public object OriginalSelectedObject
        {
            get
            {
                object selectedObject = null;

                if (null != _originalSelectedObjects && _originalSelectedObjects.Count > 0)
                {
                    selectedObject = _originalSelectedObjects[0];
                }

                return selectedObject;
            }
        }

        /// <summary>
        /// Gets the original unwrapped selected objects.
        /// </summary>
        /// <remarks>
        /// Use this property instead of <see cref="P:SelectedObjects"/> to get the correct unwrapped objects that are being displayed.
        /// If the <see cref="ReadOnly"/> property is set to <c>false</c> then the values returned by both this property and <see cref="P:SelectedObjects"/> will be the same.
        /// </remarks>
        /// <value>The original selected objects.</value>
        public ReadOnlyCollection<object> OriginalSelectedObjects
        {
            get { return _originalSelectedObjects; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.PropertyGrid.SelectedObjectsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            if (!_selectionChangedInternally)
            {
                base.OnSelectedObjectsChanged(e);

                _originalSelectedObjects = new ReadOnlyCollection<object>(SelectedObjects);
                SetReadOnly();
            }
        }

        private void SetReadOnly()
        {
            if (null != _originalSelectedObjects)
            {
                _selectionChangedInternally = true;

                if (_readOnly)
                {
                    object[] wrappedSelectedObjects = new object[SelectedObjects.Length];
                    for (int i = 0; i < wrappedSelectedObjects.Length; i++)
                    {
                        wrappedSelectedObjects[i] = new ReadOnlyWrapper(SelectedObjects[i]);
                    }

                    SelectedObjects = wrappedSelectedObjects;
                }
                else
                {
                    SelectedObjects = _originalSelectedObjects.ToArray();
                }

                _selectionChangedInternally = false;
            }
        } 
        #endregion

        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Move automatically the splitter to better fit all the properties shown.")]
        public bool AutoSizeProperties
        {
			get
			{
				return bAutoSizeProperties;
			}
			set
			{
				bAutoSizeProperties = value;
				if (value)
				{
					AutoSizeSplitter(32);
				}
			}
		}
         
		[Category("Appearance"), DefaultValue(false), DescriptionAttribute("Draw a flat toolbar")]
        public new bool DrawFlatToolbar
        {
			get
			{
				return bDrawFlatToolbar;
			}
			set
			{
				bDrawFlatToolbar = value;
				ApplyToolStripRenderMode(bDrawFlatToolbar);
			}
		}

        [Category("Appearance"), DisplayName("Toolstrip"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Toolbar object"), Browsable(true)]
        public ToolStrip ToolStrip
        {
			get
			{
				return internalToolStrip;
			}
		}
		
		[Category("Appearance"), DisplayName("Help"), DescriptionAttribute("DocComment object. Represent the comments area of the PropertyGrid."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Control DocComment
		{
			get
			{
				return  (System.Windows.Forms.Control) internalDocComment;
			}
		}
		
		[Category("Appearance"), DisplayName("HelpTitle"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Help Title Label."), Browsable(true)]
        public Label DocCommentTitle
		{
			get
			{
				return oDocCommentTitle;
			}
		}
		
		[Category("Appearance"), DisplayName("HelpDescription"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Help Description Label."), Browsable(true)]
        public Label DocCommentDescription
		{
			get
			{
				return oDocCommentDescription;
			}
		}
		
		[Category("Appearance"), DisplayName("HelpImageBackground"), DescriptionAttribute("Help Image Background.")]
        public Image DocCommentImage
		{
			get
			{
				return ((Control)internalDocComment).BackgroundImage;
			}
			set
			{
                ((Control)internalDocComment).BackgroundImage = value;
			}
		}

        #endregion

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PARENTNOTIFY && m.WParam.ToInt32() == WM_LBUTTONDOWN)
            {
                DateTime now = DateTime.Now;
                if ((now - this.lastTimeClick).TotalMilliseconds < (double)SystemInformation.DoubleClickTime && base.SelectedGridItem == this.selectedObject && this.GridItemDoubleClick != null)
                {
                    this.GridItemDoubleClick(this, EventArgs.Empty);
                }
                this.lastTimeClick = DateTime.Now;
                this.selectedObject = base.SelectedGridItem;
            }
        }
 
    }
	
}


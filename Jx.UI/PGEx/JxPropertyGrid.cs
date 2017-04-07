using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Jx.UI.PGEx
{

    public partial class JxPropertyGrid : PropertyGrid
	{			
		#region "Protected variables and objects"
		// CustomPropertyCollection assigned to MyBase.SelectedObject
		protected JxCustomPropertyCollection oCustomPropertyCollection;
		protected bool bShowCustomProperties;
		
		// CustomPropertyCollectionSet assigned to MyBase.SelectedObjects
		protected JxCustomPropertyCollectionSet oCustomPropertyCollectionSet;
		protected bool bShowCustomPropertiesSet;

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
		
		#region "Public Functions"
		public JxPropertyGrid()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			
			// Add any initialization after the InitializeComponent() call.
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

			// Initialize collections
            oCustomPropertyCollection = new JxCustomPropertyCollection();
            oCustomPropertyCollectionSet = new JxCustomPropertyCollectionSet();
			
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

        [Description("ÁÐ¿í²Î¿¼Öµ")]
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
			if (bShowCustomPropertiesSet)
			{
				base.SelectedObjects =  (object[]) oCustomPropertyCollectionSet.ToArray();
			}
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

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                this.SetObjectAsReadOnly(this.SelectedObject, _readOnly);
            }
        }

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            this.SetObjectAsReadOnly(this.SelectedObject, this._readOnly);
            base.OnSelectedObjectsChanged(e);
        }

        private void SetObjectAsReadOnly(object selectedObject, bool isReadOnly)
        {
            if (this.SelectedObject != null)
            {
                TypeDescriptionProvider provider = TypeDescriptor.AddAttributes(this.SelectedObject, new Attribute[] { new ReadOnlyAttribute(_readOnly) });
                this.Refresh();                
            }
        }

        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Set the collection of the CustomProperty. Set ShowCustomProperties to True to enable it."), RefreshProperties(RefreshProperties.Repaint)]
        public JxCustomPropertyCollection Item
        {
			get
			{
				return oCustomPropertyCollection;
			}
		}

		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Set the CustomPropertyCollectionSet. Set ShowCustomPropertiesSet to True to enable it."), RefreshProperties(RefreshProperties.Repaint)]public JxCustomPropertyCollectionSet ItemSet
        {
			get
			{
				return oCustomPropertyCollectionSet;
			}
		}

        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Move automatically the splitter to better fit all the properties shown.")]public bool AutoSizeProperties
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

        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Use the custom properties collection as SelectedObject."), RefreshProperties(RefreshProperties.All)]public bool ShowCustomProperties
        {
			get
			{
				return bShowCustomProperties;
			}
			set
			{
				if (value == true)
				{
					bShowCustomPropertiesSet = false;
					base.SelectedObject = oCustomPropertyCollection;
				}
				bShowCustomProperties = value;
			}
		}

        [Category("Behavior"), DefaultValue(false), DescriptionAttribute("Use the custom properties collections as SelectedObjects."), RefreshProperties(RefreshProperties.All)]
        public bool ShowCustomPropertiesSet
        {
			get
			{
				return bShowCustomPropertiesSet;
			}
			set
			{
				if (value == true)
				{
					bShowCustomProperties = false;
					base.SelectedObjects =  (object[]) oCustomPropertyCollectionSet.ToArray();
				}
				bShowCustomPropertiesSet = value;
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
		
		[Category("Appearance"), DisplayName("Help"), DescriptionAttribute("DocComment object. Represent the comments area of the PropertyGrid."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]public Control DocComment
		{
			get
			{
				return  (System.Windows.Forms.Control) internalDocComment;
			}
		}
		
		[Category("Appearance"), DisplayName("HelpTitle"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Help Title Label."), Browsable(true)]public Label DocCommentTitle
		{
			get
			{
				return oDocCommentTitle;
			}
		}
		
		[Category("Appearance"), DisplayName("HelpDescription"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DescriptionAttribute("Help Description Label."), Browsable(true)]public Label DocCommentDescription
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
		
	}
	
}


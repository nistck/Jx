using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

// Adjustable Height Text Box
// (c)Adrian Hayes, 2008.
// adrian.hayes@gmail.com
// * Source Code and Executable Files can be freely used in commercial applications;
// * Source Code can be modified to create derivative works.
// * No claim of suitability, guarantee, or any warranty whatsoever is provided. 
//   The software is provided "as-is".
// * If you publish the Source Code or any portion thereof, please include a
//   reference link back to http://www.bearnakedcode.com.
namespace Jx.UI.Controls
{
    /// <summary>
    /// A TextBox control that allows you to set the height of the TextBox.
    /// </summary>
    public partial class JxTextBox : TextBox
    {
        // Original height before Dock property set
        int ciPreDockHeight;
        // Original Distance to Bottom - used when set to AnchorStyles.Bottom
        int ciOrigDistanceToBottom;
        // If the textbox is set to multi-line, do the default value
        bool IsMultiLine = false; 

        public JxTextBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the height of a textbox by adjusting the size of the font
        /// </summary>
        /// <param name="TextBoxHeight">Height of the textbox</param>
        /// <param name="OriginalFont">Font used</param>
        /// <returns>Returns a font object with the correct size to adjust the TextBox size.</returns>
        private static Font GetFontForTextBoxHeight(int TextBoxHeight, Font OriginalFont)
        {
            // What is the target size of the text box?
            float desiredheight = (float)TextBoxHeight;

            // Set the font from the existing TextBox font.
            // We use the fnt = new Font(...) method so we can ensure that
            //  we're setting the GraphicsUnit to Pixels.  This avoids all
            //  the DPI conversions between point & pixel.
            Font fnt = new Font(OriginalFont.FontFamily,
                                OriginalFont.Size,
                                OriginalFont.Style,
                                GraphicsUnit.Pixel);

            // TextBoxes never size below 8 pixels. This consists of the
            // 4 pixels above & 3 below of whitespace, and 1 pixel line of
            // greeked text.
            if (desiredheight < 8)
                desiredheight = 8;

            // Determine the Em sizes of the font and font line spacing
            // These values are constant for each font at the given font style.
            // and screen DPI.
            float FontEmSize = fnt.FontFamily.GetEmHeight(fnt.Style);
            float FontLineSpacing = fnt.FontFamily.GetLineSpacing(fnt.Style);

            // emSize is the target font size.  TextBoxes have a total of
            // 7 pixels above and below the FontHeight of the font.
            float emSize = (desiredheight - 7) * FontEmSize / FontLineSpacing;

            // Create the font, with the proper size to change the TextBox Height to the desired size.
            fnt = new Font(fnt.FontFamily, emSize, fnt.Style, GraphicsUnit.Pixel);

            return fnt;
        }

        /// <summary>
        /// Determines if we should perform the default base.XXX action.
        /// </summary>
        /// <returns>
        /// True if the parent is null, the control is disposing
        /// or the Multi-Line property is true.
        /// </returns>
        private bool DoDefault()
        {
            return (Parent == null || Disposing || IsMultiLine);
        }

        /// <summary>
        /// Sets the height of the TextBox - the Size.Height property is ignored on TextBox controls
        /// </summary>
        [System.ComponentModel.Category("Layout")]
        [System.ComponentModel.Description("Set the TextBox.Height.")]
        public int LineHeight
        {
            get { return this.Height; }
            set
            {
                // If the parent does not exist, we're set to multi-line
                // or we are disposing, do default
                if (DoDefault())
                    return;
                if (value != this.Height)
                {
                    this.Height = value;
                    ciOrigDistanceToBottom = Parent.ClientSize.Height + this.Top - value;
                    this.Font = GetFontForTextBoxHeight(value, this.Font);
                }
            }
        }

        // If multi-line is set to true, set IsMultiline.  The control
        // will perform the default base.XXX actions.
        public override bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                IsMultiLine = value;
                base.Multiline = value;
            }
        }

        // If the dock style changes to a height-adjusting value, get the original
        // size first
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                // If the parent does not exist, we're set to multi-line
                // or we are disposing, do default
                if (DoDefault())
                {
                    base.Dock = value;
                    return;
                }

                // if this docking change should affect the height
                if ((value & DockStyle.Left) == DockStyle.Left ||
                    (value & DockStyle.Right) == DockStyle.Right ||
                    (value & DockStyle.Fill) == DockStyle.Fill)
                {
                    // and if the base.dock is NOT ALREADY set to a height-adjusting
                    // DockStyle, then get the original height.
                    if ((base.Dock & DockStyle.Left) != DockStyle.Left &&
                        (base.Dock & DockStyle.Right) != DockStyle.Right &&
                        (base.Dock & DockStyle.Fill) != DockStyle.Fill)
                        ciPreDockHeight = Height;
                }
                base.Dock = value;
            }
        }

        // Intercept TextBox.OnDockChanged to adjust the height of textbox
        // back to its original pre-dock value, if necessary.
        protected override void OnDockChanged(EventArgs e)
        {
            // If the parent does not exist, we're set to multi-line
            // or we are disposing, do default
            if (DoDefault())
            {
                base.OnDockChanged(e);
                return;
            }

            // if this docking change is bottom or none, set the height back to 
            // the original pre-dock value.
            if ((this.Dock & DockStyle.Bottom) == DockStyle.Bottom ||
                (this.Dock & DockStyle.None) == DockStyle.None)
            {
                this.Font = GetFontForTextBoxHeight(ciPreDockHeight, this.Font);
            }
            base.OnDockChanged(e);
        }

        // Intercept OnParentChanged to set distance to bottom for Anchoring
        // and add an event subscription to Parent.ClientSizeChanged
        protected override void OnParentChanged(EventArgs e)
        {
            // If the parent does not exist, we're set to multi-line
            // or we are disposing, do default
            if (DoDefault())
            {
                base.OnDockChanged(e);
                return;
            }
            ciOrigDistanceToBottom = Parent.ClientSize.Height - this.Bottom;
            ciPreDockHeight = this.Height;
            Parent.ClientSizeChanged += new EventHandler(Parent_ClientSizeChanged);
            base.OnParentChanged(e);
        }

        // Event Handler for Parent.ClientSizeChanged
        // If the parent size changes, we may need to adjust the textbox size
        // if it is docked or anchored.
        void Parent_ClientSizeChanged(object sender, EventArgs e)
        {
            // If the parent does not exist, we're set to multi-line
            // or we are disposing, do default
            if (DoDefault())
            {
                base.OnDockChanged(e);
                return;
            }

            if ((this.Dock & DockStyle.Left) == DockStyle.Left ||
                (this.Dock & DockStyle.Right) == DockStyle.Right ||
                (this.Dock & DockStyle.Fill) == DockStyle.Fill ||
                (this.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
                this.Font = GetFontForTextBoxHeight(Parent.ClientSize.Height, this.Font);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            // If the parent does not exist, we're set to multi-line
            // or we are disposing, do default
            if (DoDefault())
            {
                base.OnSizeChanged(e);
                return;
            }

            int height = this.Height;
            // Is the control docked or anchored to bottom?
            switch (this.Dock)
            {
                case DockStyle.Fill:
                case DockStyle.Left:
                case DockStyle.Right:
                    height = Parent.ClientSize.Height;
                    this.Font = GetFontForTextBoxHeight(height, this.Font);
                    break;
                // Not docked in a way that should modify height.
                default:
                    // Check for Anchoring that should change the height.
                    // If so, set the height based on the original distance to bottom.
                    if ((this.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
                    {
                        height = Parent.ClientSize.Height - ciOrigDistanceToBottom;
                        this.Font = GetFontForTextBoxHeight(height, this.Font);
                    }
                    break;
            }

            base.OnSizeChanged(e);
        }
    }

}
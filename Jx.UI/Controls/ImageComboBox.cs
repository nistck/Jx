using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Jx.UI.Model;

namespace Jx.UI.Controls
{
    public class ImageComboBox : ComboBox
    {
        private Color itemBorderColor = Color.FromArgb(255, 255, 255);
        private Color itemFocusBorderColor = Color.FromArgb(229, 195, 101);

        public ImageComboBox()
            : base()
        {
            base.DrawMode = DrawMode.OwnerDrawFixed;
            base.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public new System.Windows.Forms.DrawMode DrawMode
        {
            get;
            set;
        }

        public new ComboBoxStyle DropDownStyle
        {
            get;
            set;
        } 
         
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = this.ItemHeight;
        }

        private Padding itemPadding = new Padding(0, 0, 0, 0);
        public Padding ItemPadding
        {
            get { return itemPadding; }
            set { itemPadding = value; }
        }
        
        public Color ItemBorderColor
        {
            get { return itemBorderColor; }
            set { itemBorderColor = value; }
        }

        public Color ItemFocusBorderColor
        {
            get { return itemFocusBorderColor; }
            set { itemFocusBorderColor = value; }
        }

        private Color textColor = Color.Black;
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > Items.Count - 1)
                return;
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics bufferGraphics = currentContext.Allocate(e.Graphics, e.Bounds);
            Graphics g = bufferGraphics.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.Clear(this.BackColor);     

            object item = Items[e.Index]; 

            //获得项文本内容,绘制文本
            string itemText = item == null ? "" : item.ToString();
            // 图标
            Image img = null;
            Size ImageSize = new Size(16, 16);

            IViewModel itemModel = item as IViewModel;
            if (itemModel != null)
            {
                img = itemModel.Icon;
                ImageSize = itemModel.IconSize;
            }

            int paddingLeft = ItemPadding.Left;
            int paddingRight = ItemPadding.Right;
            int paddingTop = ItemPadding.Top;
            int paddingBottom = ItemPadding.Bottom;

            //填充区域
            Rectangle borderRect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

            Rectangle imgRect = new Rectangle(0, 0, 1, 1);
            Rectangle textRect = new Rectangle(0, 0, 1, 1);

            if (img == null)
            {
                textRect = new Rectangle(
                    borderRect.X + paddingLeft, borderRect.Y + paddingTop,
                    borderRect.Width - paddingLeft - paddingRight, borderRect.Height - paddingTop - paddingBottom);
            }
            else
            {
                int imageY = borderRect.Y + (borderRect.Height - ImageSize.Height) / 2;
                //图片绘制的区域
                imgRect = new Rectangle(borderRect.X + paddingLeft, imageY, ImageSize.Width, ImageSize.Height);
                //文本内容显示区域 
                textRect = new Rectangle(
                    imgRect.Right + paddingRight + paddingLeft, borderRect.Y + paddingTop,
                    borderRect.Width - paddingLeft - paddingRight * 2, borderRect.Height - paddingTop - paddingBottom);
            } 

            //鼠标选中在这个项上
            if ((e.State & DrawItemState.Selected) != 0)
            {
                //渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.FromArgb(255, 251, 237),
                                                 Color.FromArgb(255, 236, 181), LinearGradientMode.Vertical);
                g.FillRectangle(brush, borderRect);

                //画边框 
                Pen pen = new Pen(ItemFocusBorderColor);
                g.DrawRectangle(pen, borderRect);
            }
            else if ((e.State & DrawItemState.Disabled) != 0)
            {
                g.FillRectangle(new SolidBrush(SystemColors.ControlLight), e.Bounds);
                g.DrawString(itemText, e.Font, Brushes.Black, textRect);
                e.DrawFocusRectangle();
            }
            else
            {
                SolidBrush brush = new SolidBrush(ItemBorderColor);
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            if (img != null)
                g.DrawImage(img, imgRect);

            //文本格式垂直居中
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;

            Color textColor = TextColor;
            if (itemModel != null)
                textColor = itemModel.TextColor;
            if (textColor == null)
                textColor = TextColor;
            SolidBrush textBrush = new SolidBrush(textColor);
            g.DrawString(itemText, Font, textBrush, textRect, strFormat);

            // 
            bufferGraphics.Render(e.Graphics);
            g.Dispose();
            bufferGraphics.Dispose();
        }
    }
}

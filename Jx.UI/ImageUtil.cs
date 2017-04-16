using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Jx.UI
{
    public class ImageCache
    {
        [DllImport("Shell32")]
        public static extern int ExtractIconEx(
            string sFile,
            int iIndex,
            out IntPtr piLargeVersion,
            out IntPtr piSmallVersion,
            int amountIcons);


        private Icon defaultIcon;
        private Image defaultImage;

        public ImageCache(ImageList il)
        {
            this.IL = il;
        }

        public Size ImageSize
        {
            get {
                return IL == null ? new Size(16, 16) : IL.ImageSize;
            }
        }

        public ImageList IL { get; private set; }

        private void initIcon()
        {
            if (defaultIcon != null)
                return;
            defaultIcon = ImageUtil.GetIcon(Application.ExecutablePath);
            defaultImage = defaultIcon.ToBitmap();
        }

        public Icon DefaultIcon
        {
            get
            {
                initIcon();
                return defaultIcon;
            }
        }

        public Image DefaultImage
        {
            get
            {
                initIcon();
                return defaultImage;
            }
        }

        public int Index(string key)
        {
            if (IL == null || key == null)
                return 0;

            if (!IL.Images.Keys.Contains(key))
                return 0;

            return IL.Images.Keys.IndexOf(key);
        }


        public Icon GetIcon(int index)
        {
            return this[index].ToIcon();
        }

        public Icon GetIcon(string key)
        {
            return this[key].ToIcon();
        }

        public Image this[int index]
        {
            get
            {
                if (IL == null)
                    return DefaultImage;

                if (index < 0 || index > IL.Images.Count - 1)
                    return DefaultImage;

                return IL.Images[index];
            }
        }

        public Image this[String key]
        {
            get
            {
                if (IL == null || key == null)
                    return DefaultImage;

                foreach (String _key in IL.Images.Keys)
                {
                    if (_key == null)
                        continue;

                    if (key.ToLower().Equals(_key.ToLower()))
                        return IL.Images[_key];
                }
                return DefaultImage;
            }
        }
    }

    public static class ImageUtil
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static Icon GetIcon(string path = null)
        {
            path = path ?? Application.ExecutablePath;
            return Icon.ExtractAssociatedIcon(path);
        }

        public static Icon ToIcon(this Image image)
        {
            if (image == null)
                return null;

            Bitmap bmp = new Bitmap(image);
            IntPtr ptr = bmp.GetHicon();
            Icon icon = Icon.FromHandle(ptr);
            DeleteObject(ptr);
            return icon;
        }

        public static Image ToImage(this Icon icon)
        {
            if (icon == null)
                return null;

            return icon.ToBitmap();
        }

        public static Image ResizeTo(this Image Img, Size size)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(Img, 0, 0, bmp.Width, bmp.Height);
            g.Dispose();

            return bmp;
        }

        public static Image ResizeByWidth(this Image Img, int NewWidth)
        {
            float PercentW = ((float)Img.Width / (float)NewWidth);

            Bitmap bmp = new Bitmap(NewWidth, (int)(Img.Height / PercentW));
            Graphics g = Graphics.FromImage(bmp);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(Img, 0, 0, bmp.Width, bmp.Height);
            g.Dispose();

            return bmp;
        }

        public static Image ResizeByHeight(this Image Img, int NewHeight)
        {
            float PercentH = ((float)Img.Height / (float)NewHeight);

            Bitmap bmp = new Bitmap((int)(Img.Width / PercentH), NewHeight);
            Graphics g = Graphics.FromImage(bmp);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(Img, 0, 0, bmp.Width, bmp.Height);
            g.Dispose();

            return bmp;
        }

        /// <summary>
        /// Method to resize, convert and save the image.
        /// </summary>
        /// <param name="image">Bitmap image.</param>
        /// <param name="maxWidth">resize width.</param>
        /// <param name="maxHeight">resize height.</param>
        /// <param name="quality">quality setting value.</param>
        /// <param name="filePath">file path.</param>      
        public static void Save(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
    }
}

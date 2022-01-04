using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCBArtHelper
{
    //Source for image quantization code is https://www.codeproject.com/articles/66341/a-simple-yet-quite-powerful-palette-quantizer-in-c
    public partial class ImageQuantization : Form
    {
        private IColorQuantizer quantizer;
        public Image img = null;

        public ImageQuantization()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 2;
            quantizer = new PaletteQuantizer();
        }

        private void btnQuantize_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = GetQuantizedImage(this.img,Convert.ToInt32(comboBox1.SelectedItem));
        }

        private Image GetQuantizedImage(Image image,int pallete)
        {
            // checks whether a source image is valid
            if (image == null)
            {
                const String message = "Cannot quantize a null image.";
                throw new ArgumentNullException(message);
            }

            // locks the source image data
            Bitmap bitmap = (Bitmap)image;
            Rectangle bounds = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);
            BitmapData sourceData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                Int32[] sourceBuffer = new Int32[image.Width];
                Int64 sourceOffset = sourceData.Scan0.ToInt64();

                for (Int32 row = 0; row < image.Height; row++)
                {
                    Marshal.Copy(new IntPtr(sourceOffset), sourceBuffer, 0, image.Width);

                    foreach (Color color in sourceBuffer.Select(argb => Color.FromArgb(argb)))
                    {
                        quantizer.AddColor(color);
                    }

                    // increases a source offset by a row
                    sourceOffset += sourceData.Stride;
                }
            }
            catch
            {
                bitmap.UnlockBits(sourceData);
                throw;
            }

            // calculates the palette
            Bitmap result2;
            switch (pallete)
            {
                case 2:
                    result2 = Resource1._2Pallete;
                    break;
                case 3:
                    result2 = Resource1._3Pallete;
                    break;
                case 4:
                    result2 = Resource1._4Pallete;
                    break;
                case 5:
                    result2 = Resource1._5Pallete;
                    break;
                case 6:
                    result2 = Resource1._6Pallete;
                    break;
                default:
                    result2 = Resource1._4Pallete;
                    break;
            }
             
            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);

            List<Color> palette = quantizer.GetPalette(pallete);
            ColorPalette imagePalette = result2.Palette;


            for (Int32 index = 0; index < palette.Count; index++)
            {
                imagePalette.Entries[index] = palette[index];
            }

            result.Palette = imagePalette;

            // locks the target image data
            BitmapData targetData = result.LockBits(bounds, ImageLockMode.WriteOnly, result.PixelFormat);

            try
            {
                Byte[] targetBuffer = new Byte[result.Width];
                Int32[] sourceBuffer = new Int32[image.Width];
                Int64 sourceOffset = sourceData.Scan0.ToInt64();
                Int64 targetOffset = targetData.Scan0.ToInt64();

                for (Int32 row = 0; row < image.Height; row++)
                {
                    Marshal.Copy(new IntPtr(sourceOffset), sourceBuffer, 0, image.Width);

                    for (Int32 index = 0; index < image.Width; index++)
                    {
                        Color color = Color.FromArgb(sourceBuffer[index]);
                        targetBuffer[index] = quantizer.GetPaletteIndex(color);
                    }

                    Marshal.Copy(targetBuffer, 0, new IntPtr(targetOffset), result.Width);

                    // increases the offsets by a row
                    sourceOffset += sourceData.Stride;
                    targetOffset += targetData.Stride;
                }
            }
            finally
            {
                // releases the locks on both images
                bitmap.UnlockBits(sourceData);
                result.UnlockBits(targetData);
            }

            return result;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            ofd.ShowDialog();
            pictureBox1.Image = new Bitmap(ofd.FileName);
            this.img = pictureBox1.Image;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();
            sfd.FileOk += Sfd_FileOk;
        }

        private void Sfd_FileOk(object sender, CancelEventArgs e)
        {
            this.img.Save(((SaveFileDialog)sender).FileName);
        }
    }
}

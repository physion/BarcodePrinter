using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BarcodeLib;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows;


namespace BarcodePrinter
{

    [Export(typeof(BarcodeLabel))]
    class BarcodeLabel
    {

        public BarcodeLabel(String contents, int width, int height)
        {
            this.Contents = contents;
            this.Width = width;
            this.Height = height;
        }

        public string Contents {get; private set;}

        public string Code { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public ImageSource Image {
            get
            {
                return RenderImage().ToBitmapSource();
            }
        }

        public Image RenderImage()
        {
            Barcode b = new Barcode(Contents, TYPE.CODE128);
            b.ForeColor = System.Drawing.Color.Black;
            b.BackColor = System.Drawing.Color.White;
            b.Width = Width;
            b.Height = Height;
            b.IncludeLabel = true;


            return b.Encode(TYPE.CODE128, Contents);
        }
        public void Print(string printer) { }
    }


    public static class BitmapSourceHelper
    {
        /// <summary>
        /// FxCop requires all Marshalled functions to be in a class called NativeMethods.
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Image source)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(source);

            var bitSrc = bitmap.ToBitmapSource();

            bitmap.Dispose();
            bitmap = null;

            return bitSrc;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }
    }
}

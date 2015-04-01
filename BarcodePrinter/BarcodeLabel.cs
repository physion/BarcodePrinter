using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows;
using Com.SharpZebra.Printing;
using Com.SharpZebra.Commands;
using Com.SharpZebra;
using Com.SharpZebra.Commands.Codes;


namespace BarcodePrinter
{

    [Export(typeof(BarcodeLabel))]
    class BarcodeLabel
    {

        public BarcodeLabel(String contents, String epl)
        {
            this.Contents = contents;
            this.Epl = epl;
            this.Width = 1.75 * QuantityTypes.Length.Inch;
            this.Height = 0.6 * QuantityTypes.Length.Inch;
        }

        public string Contents { get; private set; }
        public string Epl { get; private set; }

        public string BarcodeType { get; private set; }

        public QuantityTypes.IQuantity Width { get; private set; }
        public QuantityTypes.IQuantity Height { get; private set; }

        public ImageSource Image
        {
            get
            {
                return RenderImage().ToBitmapSource();
            }
        }

        public Image RenderImage()
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode(Contents, BarcodeLib.TYPE.CODE128);
            b.ForeColor = System.Drawing.Color.Black;
            b.BackColor = System.Drawing.Color.White;
            b.Width = (int)Math.Ceiling(Width.ConvertTo(QuantityTypes.Length.Inch) * 96); //96 DPI
            b.Height = (int)Math.Ceiling(Height.ConvertTo(QuantityTypes.Length.Inch) * 96); //96 DPI
            b.IncludeLabel = true;


            return b.Encode(BarcodeLib.TYPE.CODE128, Contents);
        }

        public static int dpi = 203;

        public void Print(string printer)
        {
            new ZebraPrinter(printer).Print(String.Format(this.Epl, Contents)); 
        }
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

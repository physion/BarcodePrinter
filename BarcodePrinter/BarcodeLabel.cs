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

        public BarcodeLabel(String contents, QuantityTypes.IQuantity width, QuantityTypes.IQuantity height)
        {
            this.Contents = contents;
            this.Width = width;
            this.Height = height;
        }

        public string Contents { get; private set; }

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


        public void Print(string printer)
        {
            //ps.Width = 2034; //dots
            //ps.Length = 2034; //dots 


            int heightDots = (int)Math.Ceiling(Height.ConvertTo(QuantityTypes.Length.Millimetre) * 8); //8 dots per mm
            int widthDots = heightDots;

            var commands = new ZebraCommands();
            commands.Add(ZebraCommands.BarCodeCommand(10,
              10,
              ElementRotation.NO_ROTATION,
              1, //Com.SharpZebra.BarcodeType.CODE128_AUTO,
              2, //narrow width dots
              widthDots,
              heightDots,
              true,
              Contents));

            new ZebraPrinter(printer).Print(commands.ToZebraInstruction());
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

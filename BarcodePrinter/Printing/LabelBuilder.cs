using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.SharpZebra;
using Com.SharpZebra.Commands;
using Com.SharpZebra.Printing;

namespace BarcodePrinter.Printing
{

    public class BarcodeCommand
    {
        public int X { get; set;}
        public int Y {get; set;}
        public int Height {get;set;}
        public Barcode BarcodeType {get;set;}
        public string Code { get; set; }


        public byte[] ToZPL()
        {
            return ZPLCommands.BarCodeWrite(X, Y, Height, ElementDrawRotation.NO_ROTATION, BarcodeType, true, Code);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace BarcodePrinter.Printing
{
    public class ApplicationPrinter
    {
        public IList<string> InstalledPrinters { 
            get {
                return ToSortedStringArray(PrinterSettings.InstalledPrinters);
            }
        }

        public string DefaultZebraPrinter {
            get {
                foreach (string printer in InstalledPrinters)
            {
                if (!string.IsNullOrEmpty(printer)
                    && printer.ToUpper().Contains("ZEBRA"))
                {
                    return printer;
                }
            }
            return null;
            }
        }

        private IList<string> ToSortedStringArray(PrinterSettings.StringCollection printers)
        {
            List<string> stringList = new List<string>();
            foreach (string printer in printers)
            {
                stringList.Add(printer);
            }
            stringList.Sort(StringComparer.Ordinal);
            return stringList;
        }
    }
}

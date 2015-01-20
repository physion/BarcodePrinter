using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace BarcodePrinter.Printing
{
    public class ApplicationPrinter
    {
        public IList<string> InstalledPrinters
        {
            get
            {
                return ToSortedStringArray(PrinterSettings.InstalledPrinters);
            }
        }

        public IList<string> InstalledZebraPrinters
        {
            get
            {
                return InstalledPrinters.Where(IsZebraPrinter).ToList();
            }
        }

        public string DefaultZebraPrinter
        {
            get { return InstalledZebraPrinters.FirstOrDefault(); }
        }

        private static bool IsZebraPrinter(string printer)
        {
            return !string.IsNullOrEmpty(printer)
                   && printer.ToUpper().Contains("ZEBRA");
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

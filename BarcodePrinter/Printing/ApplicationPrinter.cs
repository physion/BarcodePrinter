using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using Caliburn.Micro;

namespace BarcodePrinter.Printing
{
    public class ApplicationPrinter
    {
        ILog logger = LogManager.GetLog(typeof(ApplicationPrinter));

        public IList<string> InstalledPrinters
        {
            get
            {
                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    logger.Info("InstalledPrinters list is empty. Trying via System.Management");

                    var query = new ObjectQuery("SELECT * FROM Win32_Printer");
                    var searcher = new ManagementObjectSearcher(query);

                    List<string> result = new List<string>();
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        result.Add((string)mo["Name"]);
                    }

                    result.Sort(StringComparer.Ordinal);

                    return result;

                }


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
                   && (printer.ToUpper().Contains("ZEBRA") || printer.ToUpper().Contains("ZDESIGNER"));
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

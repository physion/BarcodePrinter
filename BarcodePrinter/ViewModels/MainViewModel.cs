using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using BarcodePrinter.Printing;
using System.Windows.Media;
using System.Windows;
using Caliburn.Micro;

namespace BarcodePrinter.ViewModels
{
    [Export(typeof(MainViewModel))]
    class MainViewModel : Caliburn.Micro.PropertyChangedBase
    {
        private const string WindowTitleDefault = "Ovation Barcode";

        private string _windowTitle = WindowTitleDefault;

        public BarcodeLabel Label { get; private set; }

        public String SelectedPrinter { get; set; }
        private ApplicationPrinter AppPrinter { get; set; }
        public IList<String> AvailablePrinters
        {
            get
            {
                return AppPrinter.InstalledPrinters;
            }
        }

        int _printProgress = 0;
        public int Progress {
            get
            {
                return _printProgress;
            }

            set
            {
                _printProgress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }

        [ImportingConstructor]
        public MainViewModel(BarcodeLabel label, ApplicationPrinter printer)
        {
            this.Label = label;
            this.AppPrinter = printer;
            this.SelectedPrinter = printer.DefaultZebraPrinter;
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

        public ImageSource LabelImage
        {
            get
            {
                return Label.Image;
            }
        }

        public async Task Print()
        {
            try
            {
                await Task.Run(() => Label.Print(SelectedPrinter));
            }
            catch (Exception e)
            {
                LogManager.GetLog(GetType()).Error(e);
                throw;
            }
        }
    }
}

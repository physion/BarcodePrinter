using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using BarcodePrinter.Printing;
using System.Windows.Media;
using System.Windows;
using Caliburn.Micro;
using NLog;
using LogManager = Caliburn.Micro.LogManager;

namespace BarcodePrinter.ViewModels
{
    [Export(typeof(MainViewModel))]
    class MainViewModel : Screen
    {
        private const string WindowTitleDefault = "Ovation Barcode";

        private string _windowTitle = WindowTitleDefault;
        private IEventAggregator eventAggregator;

        [ImportingConstructor]
        public MainViewModel(BarcodeLabel label, ApplicationPrinter printer, IEventAggregator eventAggregator)
        {
            this.Label = label;
            this.AppPrinter = printer;
            this.SelectedPrinter = printer.DefaultZebraPrinter;
            this.eventAggregator = eventAggregator;
            logger = LogManager.GetLog(GetType());

        }


        public BarcodeLabel Label { get; private set; }

        private string _selectedPrinter;

        public string SelectedPrinter
        {
            get { return _selectedPrinter; }
            set
            {
                _selectedPrinter = value;
                NotifyOfPropertyChange(() => SelectedPrinter);
                NotifyOfPropertyChange(() => CanPrint);
            }
        }

        private ApplicationPrinter AppPrinter { get; set; }
        public IList<string> AvailablePrinters
        {
            get { return AppPrinter.InstalledZebraPrinters; }
        }

        int _printProgress = 0;
        private ILog logger;

        public int Progress
        {
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

        public void Print()
        {
            try
            {
                //await Task.Run(() => Label.Print(SelectedPrinter));
                Label.Print(SelectedPrinter);

                eventAggregator.PublishOnUIThread(new PrintCompletion(this));

            }
            catch (Exception e)
            {
                logger.Error(e);
                MessageBox.Show("Unable to print label:\n" + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanPrint
        {
            get { return SelectedPrinter != null; }
        }


    }
}

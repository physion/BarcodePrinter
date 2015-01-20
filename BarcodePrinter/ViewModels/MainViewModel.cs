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
using Squirrel;

namespace BarcodePrinter.ViewModels
{
    [Export(typeof(MainViewModel))]
    class MainViewModel : Screen
    {
        private const string WindowTitleDefault = "Ovation Barcode";

        private string _windowTitle = WindowTitleDefault;
        private Task updateTask {get; set; }

        [ImportingConstructor]
        public MainViewModel(BarcodeLabel label, ApplicationPrinter printer)
        {
            this.Label = label;
            this.AppPrinter = printer;
            this.SelectedPrinter = printer.DefaultZebraPrinter;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            updateTask = Task.Run(() => CheckUpdates());
        }

        public override void CanClose(Action<bool> callback)
        {
            callback(updateTask.IsCompleted || updateTask.IsFaulted);
        }

        async Task CheckUpdates()
        {

            using (var mgr = new UpdateManager(@"https://s3.amazonaws.com/download.ovation.io/barcode_printer", "us-physion-barcode-printer", FrameworkVersion.Net45))
            {
                SquirrelAwareApp.HandleEvents(onInitialInstall: v => AssociateFileExtenstion(),
                    onAppUpdate: v => AssociateFileExtenstion(),
                    // ReSharper disable once AccessToDisposedClosure
                    onAppUninstall: v => mgr.RemoveShortcutForThisExe());

                await mgr.UpdateApp();
            }
        }

        void AssociateFileExtenstion()
        {

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
            get
            {
                return AppPrinter.InstalledZebraPrinters;
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
                MessageBox.Show("Unable to print label:\n" + e.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanPrint
        {
            get { return SelectedPrinter != null; }
        }


    }
}

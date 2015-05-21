using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using BarcodePrinter.ViewModels;
using System.Windows;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using BarcodePrinter.Printing;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BarcodePrinter.Logging;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BarcodePrinter
{
    sealed class AppBootstrapper : BootstrapperBase, IHandle<PrintCompletion>
    {
        private WindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator = new EventAggregator();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppBootstrapper()
        {
            StartRuntime();
#if (DEBUG==false)
            LogManager.GetLog = type => (ILog)new NLogLogger(type);
#endif
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            ILog logger = LogManager.GetLog(GetType());


            logger.Info("Starting barcode printer");

            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
            {
                string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                if (activationData != null && activationData.Length >= 1)
                {
                    var path = new Uri(activationData[0]).LocalPath;
                    OpenLabel(path);
                }
                else
                {
#if DEBUG


                    OpenLabel("../../test.obc");
#else
                    logger.Info("No input file found. Exiting.");
                    Application.Current.Shutdown();
#endif
                }
            }
            else
            {
#if DEBUG


                OpenLabel("../../test.obc");
#else
                    logger.Info("No input file found. Exiting.");
                    Application.Current.Shutdown();
#endif
            }

        }


        private void OpenLabel(string path)
        {
            ILog logger = LogManager.GetLog(GetType());

            logger.Info("Opening {0} for printing", path);
            try
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    dynamic o = JToken.ReadFrom(new JsonTextReader(reader))[0];

                    ShowPrintLabel((string)o.label, (string)o.epl);
                }
            }
            catch (FileNotFoundException ex)
            {
                logger.Error(ex);
                MessageBox.Show("Unable to open label file:\n" + ex.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

        }

        private void ShowPrintLabel(string label, string epl)
        {
            _eventAggregator.Subscribe(this);
            _windowManager.ShowWindow(new MainViewModel(new BarcodeLabel(label, epl), new ApplicationPrinter(), _eventAggregator));
        }


        protected override void Configure()
        {
            _windowManager = new WindowManager();
        }

        public void Handle(PrintCompletion message)
        {
            Application.Current.Shutdown();
        }
    }
}
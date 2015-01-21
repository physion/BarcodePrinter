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
using Squirrel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BarcodePrinter
{
    sealed class AppBootstrapper : BootstrapperBase, IDisposable, IHandle<PrintCompletion>
    {
        private CompositionContainer _container;
        private WindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator = new EventAggregator();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppBootstrapper()
        {
            StartRuntime();
            LogManager.GetLog = type => new DebugLogger(type);
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            ILog logger = LogManager.GetLog(GetType());


            AppUpater.Register();

            string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (args.Length >= 1)
            {
                var path = args[0];
                OpenLabel(path);

            }
            else
            {
                OpenLabel("../../test.obc");
                //              MessageBox.Show("Double-click an '.obc' file.", "Oops!",
                //                  MessageBoxButton.OK, MessageBoxImage.Hand);
                //
                //              Application.Current.Shutdown();
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
                    dynamic o = JToken.ReadFrom(new JsonTextReader(reader));

                    // Assumes width & height in inches
                    ShowPrintLabel((string)o.label, ((float)o.width) * QuantityTypes.Length.Inch, ((float)o.height) * QuantityTypes.Length.Inch);
                }
            }
            catch (FileNotFoundException ex)
            {
                logger.Error(ex);
                MessageBox.Show("Unable to open label file:\n" + ex.Message, "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

        }

        private void ShowPrintLabel(string label, QuantityTypes.IQuantity width, QuantityTypes.IQuantity height)
        {
            _eventAggregator.Subscribe(this);
            _windowManager.ShowWindow(new MainViewModel(new BarcodeLabel(label, width, height), new ApplicationPrinter(), _eventAggregator));
        }


        protected override void Configure()
        {
            _container = new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(_windowManager);
            batch.AddExportedValue<IEventAggregator>(_eventAggregator);
            batch.AddExportedValue(_container);

            _container.Compose(batch);

            _windowManager = new WindowManager();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }



        public void Dispose()
        {
            _container.Dispose();
        }

        public void Handle(PrintCompletion message)
        {
            Application.Current.Shutdown();
        }
    }
}
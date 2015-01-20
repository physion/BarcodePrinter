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

namespace BarcodePrinter
{
    class AppBootstrapper : BootstrapperBase, IDisposable
    {
        private CompositionContainer container;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppBootstrapper()
        {
            LogManager.GetLog = type => new DebugLogger(type);
            StartRuntime();
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //TODO read parameter and set it in BarcodeLabel
            string[] args = Environment.GetCommandLineArgs();
            if (args != null && args.Length >= 2)
            {
                string path = args[1];
                LogManager.GetLog(GetType()).Info("Opening {} for printing", path);
                using (StreamReader reader = File.OpenText(path))
                {
                    dynamic o = JToken.ReadFrom(new JsonTextReader(reader));
                    ShowPrintLabel(o.label, o.height, o.width);
                }

            }
            else
            {
                ShowPrintLabel(Guid.NewGuid().ToString(), 1000, 300);
            }

            
        }

        private void ShowPrintLabel(string label, int width, int height)
        {
            new WindowManager().ShowWindow(new MainViewModel(new BarcodeLabel(label, width, height), new ApplicationPrinter()));
        }

        protected override void Configure()
        {
            container = new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }



        public void Dispose()
        {
            container.Dispose();
        }
    }
}
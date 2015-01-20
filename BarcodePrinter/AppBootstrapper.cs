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

namespace BarcodePrinter
{
    class AppBootstrapper : BootstrapperBase, IDisposable
    {
        private CompositionContainer container;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppBootstrapper()
        {
            StartRuntime();
            LogManager.GetLog = type => new DebugLogger(type);
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            ILog logger = LogManager.GetLog(GetType());

            //string[] args = Environment.GetCommandLineArgs();
            //if (args != null && args.Length >= 2)
            //{
            //    string path = args[1];
            //    OpenLabel(path);

            //}

            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null && 
                AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
            {
                try
                {
                    string pathUri = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0];

                    // It comes in as a URI; this helps to convert it to a path.
                    Uri uri = new Uri(pathUri);
                    string path = uri.LocalPath;

                    OpenLabel(path);

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                OpenLabel("../../test.obc.txt");
                //ShowPrintLabel("TEST", 500, 120);
            }

            Task.Run(() => CheckUpdates());

        }

        async Task CheckUpdates()
        {

            //TODO we should do this at app exit

            using (var mgr = new UpdateManager("https://path/to/my/update/folder", "nuget-package-id", FrameworkVersion.Net45))
            {
                await mgr.UpdateApp();
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
                    ShowPrintLabel((string)o.label, (int)o.width, (int)o.height);
                }
            }
            catch (FileNotFoundException ex)
            {
                logger.Error(ex);
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
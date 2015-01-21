using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Squirrel;

namespace BarcodePrinter
{
    class AppUpater
    {

        public Task UpdateTask { get; private set; }

        public bool UpdateComplete
        {
            get { return UpdateTask.IsCompleted || UpdateTask.IsFaulted; }
        }

        public void CheckUpdates()
        {
            UpdateTask = Task.Run(() => RunCheck());
        }

        private static ILog logger = LogManager.GetLog(typeof (AppUpater));

        public static void Register()
        {
            using (var mgr = new UpdateManager(@"https://s3.amazonaws.com/download.ovation.io/barcode_printer", "PhysionBarcodePrinter", FrameworkVersion.Net45))
            {
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => MessageBox.Show("Barcode Printer installed succesfully", "Installation Succesful", MessageBoxButton.OK, MessageBoxImage.Information),
                    onAppUpdate: v => logger.Info("App updated"),
                    // ReSharper disable once AccessToDisposedClosure
                    onAppUninstall: v => mgr.RemoveShortcutForThisExe());

            }
        }

        private async Task RunCheck()
        {
            using (var mgr = new UpdateManager(@"https://s3.amazonaws.com/download.ovation.io/barcode_printer", "PhysionBarcodePrinter", FrameworkVersion.Net45))
            {
                await mgr.UpdateApp();
            }
        }
    }
}
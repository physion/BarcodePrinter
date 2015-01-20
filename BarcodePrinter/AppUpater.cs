using System.Threading.Tasks;
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

        private async Task RunCheck()
        {
            using (
                var mgr = new UpdateManager(@"https://s3.amazonaws.com/download.ovation.io/barcode_printer",
                    "us-physion-barcode-printer", FrameworkVersion.Net45))
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
    }
}
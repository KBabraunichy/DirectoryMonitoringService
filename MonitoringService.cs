using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;

namespace DirectoryMonitoringService
{
    public partial class MonitoringService : ServiceBase
    {
        Logger logger;
        public MonitoringService()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }

    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;

        string folderToStore;
        string fileName;

        public Logger()
        {
            fileName = ConfigurationManager.AppSettings["FileName"];
            folderToStore = ConfigurationManager.AppSettings["FolderToStore"];
            DirectoryInfo directoryInfo = Directory.CreateDirectory(folderToStore);

            watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["FolderToMonitor"]);
            watcher.Deleted += WatcherDeleted;
            watcher.Created += WatcherCreated;
            watcher.Changed += WatcherChanged;
            watcher.Renamed += WatcherRenamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            RecordEntry("renamed to " + e.FullPath, e.OldFullPath);
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            RecordEntry("changed", e.FullPath);
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            RecordEntry("created", e.FullPath);
        }

        private void WatcherDeleted(object sender, FileSystemEventArgs e)
        {
            RecordEntry("removed", e.FullPath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter($"{folderToStore}\\{fileName}", true))
                {
                    writer.WriteLine(String.Format("{0} {1} was {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
}

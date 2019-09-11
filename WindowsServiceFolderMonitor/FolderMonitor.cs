using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DataModelLibrary;
using LoggerLibrary;
using Newtonsoft.Json;
using WindowsServiceFolderMonitor.ServiceReference1;

namespace WindowsServiceFolderMonitor
{
    public class FolderMonitor
    {
        private readonly string _monitorFolder;
        private readonly string _compliteFolder;
        private readonly string _garbageFolder;

        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        private ServiceCheckClient _client;

        public FolderMonitor(ServiceCheckClient client, CancellationTokenSource tokenSource, CancellationToken token)
        {
            _monitorFolder = ConfigurationManager.AppSettings["monitorFolder"];
            _compliteFolder = ConfigurationManager.AppSettings["compliteFolder"];
            _garbageFolder = ConfigurationManager.AppSettings["garbageFolder"];

            _tokenSource = tokenSource;
            _token = token;
            _client = client;

            Logger.Log.Info("FolderMonitor constructor Ok");
        }

        private void Fail(string errorMessage)
        {
            Logger.Log.Error(errorMessage);
            throw new Exception(errorMessage);
        }

        private void CreateDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
                Logger.Log.Info($"Created directory {directoryName}");
            }
        }

        private string GetDestination(string folder, string fileName)
        {
            string destination = Path.Combine(folder, fileName);
            if (File.Exists(destination))
            {
                string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" +
                                     DateTime.Now.Ticks + Path.GetExtension(fileName);
                destination = Path.Combine(folder, newFileName);
            }

            Logger.Log.Info($"Destination: {destination}");
            return destination;
        }

        public async Task StartWatch()
        {
            if (string.IsNullOrEmpty(_monitorFolder))
            {
                Fail("MonitorFolder configuration file fail");
            }
            else
            {
                CreateDirectory(_monitorFolder);
            }

            if (string.IsNullOrEmpty(_compliteFolder))
            {
                Fail("CompileFolder configuration file fail");
            }
            else
            {
                CreateDirectory(_compliteFolder);
            }

            if (string.IsNullOrEmpty(_garbageFolder))
            {
                Fail("GarbageFolder configuration file fail");
            }
            else
            {
                CreateDirectory(_garbageFolder);
            }

            await Task.Factory.StartNew(() =>
            {
                using (FileSystemWatcher watcher = new FileSystemWatcher())
                {
                    watcher.Path = _monitorFolder;

                    watcher.NotifyFilter = NotifyFilters.LastAccess
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.FileName
                                           | NotifyFilters.DirectoryName;

                    watcher.Filter = "*";

                    watcher.Created += (s, ev) =>
                    {
                        Logger.Log.Info($"File {ev.FullPath} created");
                        if (Path.GetExtension(ev.Name).Equals(".txt"))
                        {
                            string json = File.ReadAllText(ev.FullPath);
                            try
                            {
                                Check check = JsonConvert.DeserializeObject<Check>(json);
                                if (check != null)
                                {
                                    _client.SaveCheck(check);
                                    File.Move(ev.FullPath, GetDestination(_compliteFolder, ev.Name));
                                    Logger.Log.Info($"File {ev.FullPath} moved to Completed");
                                }
                                else
                                {
                                    File.Move(ev.FullPath, GetDestination(_garbageFolder, ev.Name));
                                    Logger.Log.Info($"File {ev.FullPath} moved to Garbage");
                                }
                            }
                            catch (Exception e)
                            {
                                File.Move(ev.FullPath, GetDestination(_garbageFolder, ev.Name));
                                Logger.Log.Error($"File {ev.FullPath} moved to Garbage. Exception: {e.Message}");
                            }
                        }
                        else
                        {
                            File.Move(ev.FullPath, GetDestination(_garbageFolder, ev.Name));
                            Logger.Log.Info($"File {ev.FullPath} moved to Garbage");
                        }
                    };

                    watcher.EnableRaisingEvents = true;

                    Logger.Log.Info($"FileWatcher listen folder {_monitorFolder}...");

                    while (!_token.IsCancellationRequested)
                    {
                        Thread.Sleep(200);
                    }
                }
            }, _token);
        }

        public void StopWatch()
        {
            _tokenSource.Cancel();
            Logger.Log.Info("FileWatcher stopped");
        }
    }
}

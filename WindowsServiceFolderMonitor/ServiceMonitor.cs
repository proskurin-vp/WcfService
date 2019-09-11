using System;
using System.ServiceProcess;
using WindowsServiceFolderMonitor.ServiceReference1;
using System.Threading;
using System.Threading.Tasks;
using LoggerLibrary;

namespace WindowsServiceFolderMonitor
{
    public partial class ServiceMonitor : ServiceBase
    {
        private ServiceCheckClient _client;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;
        private Task _folderMonitorTask;

        public ServiceMonitor()
        {
            InitializeComponent();

            Logger.InitLogger();
            log4net.Config.XmlConfigurator.Configure();

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            Logger.Log.Info("ServiceMonitor constructor Ok");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _client = new ServiceCheckClient();
                FolderMonitor folder = new FolderMonitor(_client, _tokenSource,  _token);
                _folderMonitorTask = folder.StartWatch();
                Logger.Log.Info("ServiceMonitor OnStart Ok");
            }
            catch (Exception e)
            {
                Logger.Log.Error("ServiceMonitor OnStart Fail", e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                _client?.Close();
                _tokenSource.Cancel();
                Logger.Log.Info("ServiceMonitor OnStop Ok");
            }
            catch (Exception e)
            {
                Logger.Log.Error("ServiceMonitor OnStop Fail", e);
            }
        }
    }
}

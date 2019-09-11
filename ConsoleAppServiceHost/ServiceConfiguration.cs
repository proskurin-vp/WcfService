using LoggerLibrary;
using System;
using System.Configuration;
using System.Linq;
using WcfServiceLibraryCheck;
using static System.Environment;

namespace ConsoleAppServiceHost
{
    public class ServiceConfiguration 
    {
        private readonly string RepositoryKey = "repository";
        private readonly string AppDataKey = "appData";

        public ServiceConfiguration()
        {
            Logger.InitLogger();
            log4net.Config.XmlConfigurator.Configure();
        }

        private string GetAppDataFolder()
        {
            SpecialFolder specialFolderValue = SpecialFolder.MyDocuments;

            if (!ConfigurationManager.AppSettings.AllKeys.Contains(AppDataKey))
            {
              
                Logger.Log.Info("AppData key error. AppData value set to MyDocuments");
            }
            else
            {
                string appDataValue = ConfigurationManager.AppSettings.Get(AppDataKey);

                if (!Enum.TryParse(appDataValue, true, out specialFolderValue))
                {
                    Logger.Log.Info("AppData value error. AppData value set to MyDocuments");
                }
            }
            return GetFolderPath(specialFolderValue);
        }

        private string GetConnectionString()
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            string connectionString = null;
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            return connectionString;
        }

        public IRepository ConfigureRepository()
        {
            string folder = GetAppDataFolder();
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(RepositoryKey))
            {
                Logger.Log.Info("Repository key error. Repository value set to FakeRepository");
            }

            string repositoryName = ConfigurationManager.AppSettings.Get(RepositoryKey);
            IRepository repository = null;
            switch (repositoryName)
            {
                case "FakeRepository": repository = new FakeRepository(folder); break;

                case "MainRepository":
                    string connectionString = GetConnectionString();
                    if (connectionString != null)
                    {
                        repository = new MainRepository(connectionString);
                    }
                    break;
               

                default: repository = new FakeRepository(folder);
                    Logger.Log.Info("Created default FakeRepository");
                    break;
            }
            return repository;
        }
    }
}

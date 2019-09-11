using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataModelLibrary;
using LoggerLibrary;

namespace WcfServiceLibraryCheck
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceCheck : IServiceCheck
    {
        private readonly IRepository _repository;

        public  ServiceCheck()
        { }

        public ServiceCheck(IRepository repository)
        {
            _repository = repository;

            Logger.InitLogger();
            log4net.Config.XmlConfigurator.Configure();
        }

        public List<Check> GetLastPack(int packSize)
        {
            List<Check> checks = null;
            try
            {
                checks = _repository.GetLastPack(packSize);
            }
            catch (Exception e)
            {
               Logger.Log.Error(e.Message);
            }

            return checks;
        }

        public int SaveCheck(Check check)
        {
            int affectedRows = 0;
            try
            {
                affectedRows = _repository.SaveCheck(check);
                Logger.Log.Info($"Affected rows for check {check.CheckNumber} = {affectedRows}");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return affectedRows;
        }
    }
}

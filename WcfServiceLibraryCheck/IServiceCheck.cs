using System.Collections.Generic;
using System.ServiceModel;
using DataModelLibrary;

namespace WcfServiceLibraryCheck
{
    [ServiceContract]
    public interface IServiceCheck
    {
        [OperationContract]
        int SaveCheck(Check check);

        [OperationContract]
        List<Check> GetLastPack(int packSize);
    }
}

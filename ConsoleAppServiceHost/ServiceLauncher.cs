using System;
using System.ServiceModel;
using WcfServiceLibraryCheck;

namespace ConsoleAppServiceHost
{
    class ServiceLauncher
    {
        static void Main()
        {
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
            IRepository repository = serviceConfiguration.ConfigureRepository();
            var serviceCheck = new ServiceCheck(repository);
            using (ServiceHost serviceHost = new ServiceHost(serviceCheck))
            {
                try
                {
                    serviceHost.Open();
                    if (serviceHost.State == CommunicationState.Opened)
                    {
                        Console.WriteLine($"Service {serviceHost.Description.Name} started on {serviceHost.Description.Endpoints[0].Address}");
                        Console.WriteLine($"Repository: {repository.Name}");
                        Console.WriteLine("To stop service press any key...");
                        Console.ReadKey(true);
                        Console.WriteLine($"{serviceHost.Description.Name} stopped");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}

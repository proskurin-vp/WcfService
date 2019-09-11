namespace WcfServiceLibraryCheck
{
    public interface IRepository : IServiceCheck
    {
        string Name { get; }
    }
}

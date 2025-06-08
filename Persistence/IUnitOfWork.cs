namespace nexus_response.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Models.User> Users { get; }
        IRepository<Models.Incident> Incidents { get; }
        IRepository<Models.IoTData> IoTData { get; }
        IRepository<Models.IncidentComment> IncidentComments { get; }
        IRepository<Models.Device> Devices { get; }
        Task<int> CompleteAsync();
    }
}


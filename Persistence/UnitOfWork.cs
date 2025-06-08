using nexus_response.Data;
using nexus_response.Models;

namespace nexus_response.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            Incidents = new Repository<Incident>(_context);
            IoTData = new Repository<IoTData>(_context);
            IncidentComments = new Repository<IncidentComment>(_context);
            Devices = new Repository<Device>(_context);
        }
        public IRepository<User> Users { get; private set; }

        public IRepository<Incident> Incidents { get; private set; }

        public IRepository<IoTData> IoTData { get; private set; }

        public IRepository<IncidentComment> IncidentComments { get; private set; }

        public IRepository<Device> Devices { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


using nexus_response.Models;
using nexus_response.Persistence;
using System;
using System.Linq;

namespace nexus_response.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; 
            }

            var users = new User[]
            {
                new User { Name = "Agente Civil", CPF = "11122233344", Email = "agente@defesacivil.gov.br", Password = HashPassword("senha123"), CreatedAt = DateTime.UtcNow },
                new User { Name = "Supervisor Geral", CPF = "55566677788", Email = "supervisor@defesacivil.gov.br", Password = HashPassword("senha123"), CreatedAt = DateTime.UtcNow }
            };

            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            var devices = new Device[]
            {
                new Device { Name = "EnvNode01_ESP32Client", Type = "Environmental Node", Location = "Lab 1", Status = "Online", LastCommunication = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
                new Device { Name = "LocationTracker01", Type = "Location Tracker", Location = "Field 1", Status = "Online", LastCommunication = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
                new Device { Name = "StatusReporter01", Type = "Status Reporter", Location = "Command Center", Status = "Online", LastCommunication = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
            };

            foreach (Device d in devices)
            {
                context.Devices.Add(d);
            }
            context.SaveChanges();

            // Exemplo de dados IoT para o dashboard
            var iotData = new IoTData[]
            {
                new IoTData { DeviceId = devices[0].Id, Type = "environmental", RawData = "{\"temperature_c\":24,\"humidity_percent\":40,\"distance_cm\":362}", Temperature = 24, Humidity = 40, Distance = 362, Timestamp = DateTime.UtcNow.AddHours(-1) },
                new IoTData { DeviceId = devices[0].Id, Type = "environmental", RawData = "{\"temperature_c\":25,\"humidity_percent\":42,\"distance_cm\":350}", Temperature = 25, Humidity = 42, Distance = 350, Timestamp = DateTime.UtcNow }
            };

            foreach (IoTData data in iotData)
            {
                context.IoTData.Add(data);
            }
            context.SaveChanges();

            var incidents = new Incident[]
            {
                new Incident { Description = "Enchente atingiu residência", Latitude = -23.5505, Longitude = -46.6333, StatusReport = "Assistance Needed", Source = "manual", UrgencyLevel = "Alto", UrgencyScore = 70, UrgencyFactors = "Palavra-chave: enchente;", Status = "Aberto", Timestamp = DateTime.UtcNow.AddDays(-2), ReportedById = users[0].Id },
                new Incident { Description = "Deslizamento de terra", Latitude = -23.5000, Longitude = -46.6000, StatusReport = "Critical", Source = "manual", UrgencyLevel = "Crítico", UrgencyScore = 90, UrgencyFactors = "Palavra-chave: deslizamento;", Status = "Aberto", Timestamp = DateTime.UtcNow.AddDays(-1), ReportedById = users[1].Id }
            };

            foreach (Incident i in incidents)
            {
                context.Incidents.Add(i);
            }
            context.SaveChanges();

            var comments = new IncidentComment[]
            {
                new IncidentComment { IncidentId = incidents[0].Id, Content = "Equipe a caminho.", UserId = users[1].Id, UserName = users[1].Name, Timestamp = DateTime.UtcNow.AddHours(-1) }
            };

            foreach (IncidentComment c in comments)
            {
                context.IncidentComments.Add(c);
            }
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            // Usar em ambiente de prod
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}


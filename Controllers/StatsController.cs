using Microsoft.AspNetCore.Mvc;
using nexus_response.DTOs;
using nexus_response.Models;
using nexus_response.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace nexus_response.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtém estatísticas gerais para o dashboard principal.
        /// </summary>
        /// <returns>Estatísticas consolidadas de incidentes, dispositivos e últimos dados IoT.</returns>
        /// <response code="200">Estatísticas retornadas com sucesso.</response>
        /// <response code="500">Erro ao processar as estatísticas.</response>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats()
        {
            var totalIncidents = (await _unitOfWork.Incidents.GetAllAsync()).Count();
            var criticalIncidents = (await _unitOfWork.Incidents.GetAllAsync())
                .Count(i => i.UrgencyLevel == "Crítico");
            var activeDevices = (await _unitOfWork.Devices.GetAllAsync())
                .Count(d => d.Status == "Online");

            var latestIoTData = (await _unitOfWork.IoTData.GetAllAsync())
                .OrderByDescending(d => d.Timestamp)
                .FirstOrDefault();

            var dashboardStats = new DashboardStatsDTO
            {
                TotalIncidents = totalIncidents,
                CriticalIncidents = criticalIncidents,
                ActiveDevices = activeDevices,
                LastUpdate = DateTime.UtcNow,
                LatestTemperature = latestIoTData?.Temperature,
                LatestHumidity = latestIoTData?.Humidity,
                LatestDistance = latestIoTData?.Distance,
                LatestAccelerometer = latestIoTData != null ? new AccelerometerDTO { X = latestIoTData.AccelerometerX, Y = latestIoTData.AccelerometerY, Z = latestIoTData.AccelerometerZ } : null
            };

            return Ok(dashboardStats);
        }

        /// <summary>
        /// Obtém o histórico detalhado de um incidente específico, incluindo comentários e dados IoT relacionados.
        /// </summary>
        /// <param name="id">ID do incidente.</param>
        /// <returns>Objeto contendo incidente, comentários e dados IoT associados.</returns>
        /// <response code="200">Histórico retornado com sucesso.</response>
        /// <response code="404">Incidente não encontrado.</response>
        /// <response code="500">Erro ao processar o histórico.</response>
        [HttpGet("history/incident/{id}")]
        public async Task<ActionResult<IncidentHistoryDTO>> GetIncidentHistory(int id)
        {
            var incident = await _unitOfWork.Incidents.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { message = "Incidente não encontrado" });
            }

            var comments = (await _unitOfWork.IncidentComments.GetAllAsync())
                .Where(c => c.IncidentId == id)
                .OrderBy(c => c.Timestamp)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    UserId = c.UserId,
                    UserName = c.UserName,
                    Timestamp = c.Timestamp
                })
                .ToList();

            var iotData = (await _unitOfWork.IoTData.GetAllAsync())
                .Where(d => d.IncidentId == id)
                .OrderBy(d => d.Timestamp)
                .Select(d => new IoTDataDTO
                {
                    DeviceId = d.DeviceId,
                    Type = d.Type,
                    Data = new IoTDataDetailDTO
                    {
                        Temperature_C = d.Temperature,
                        Humidity_Percent = d.Humidity,
                        Distance_CM = d.Distance,
                        Accelerometer = new AccelerometerDTO { X = d.AccelerometerX, Y = d.AccelerometerY, Z = d.AccelerometerZ }
                    },
                    Timestamp = d.Timestamp
                })
                .ToList();

            var history = new IncidentHistoryDTO
            {
                Incident = incident,
                Comments = comments,
                IoTData = iotData
            };

            return Ok(history);
        }
    }
}


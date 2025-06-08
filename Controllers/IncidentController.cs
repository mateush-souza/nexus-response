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
    public class IncidentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncidentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Recebe dados brutos de sensores IoT e gera um incidente automaticamente.
        /// </summary>
        /// <param name="iotDataDto">Dados recebidos via MQTT bridge.</param>
        /// <returns>Mensagem de sucesso e classificação de urgência.</returns>
        /// <response code="200">Dados processados e incidente gerado.</response>
        /// <response code="400">Dispositivo não encontrado.</response>
        /// <response code="500">Erro interno no processamento.</response>
        [HttpPost("iot-data")]
        public async Task<IActionResult> ReceiveIoTData([FromBody] IoTDataDTO iotDataDto)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync(iotDataDto.DeviceId);
            if (device == null)
            {
                return BadRequest(new { message = "Dispositivo não encontrado" });
            }

            var iotData = new IoTData
            {
                DeviceId = iotDataDto.DeviceId,
                Type = iotDataDto.Type,
                RawData = iotDataDto.Data.ToString(), // Converter o objeto de dados para string
                Timestamp = iotDataDto.Timestamp,
                Temperature = iotDataDto.Data.Temperature_C,
                Humidity = iotDataDto.Data.Humidity_Percent,
                Distance = iotDataDto.Data.Distance_CM,
                AccelerometerX = iotDataDto.Data.Accelerometer?.X,
                AccelerometerY = iotDataDto.Data.Accelerometer?.Y,
                AccelerometerZ = iotDataDto.Data.Accelerometer?.Z
            };

            await _unitOfWork.IoTData.AddAsync(iotData);
            await _unitOfWork.CompleteAsync();

            // Lógica de classificação de urgência baseada em IA (simplificada para o exemplo)
            var urgencyLevel = ClassifyUrgency(iotData);

            // Criar um incidente ou atualizar um existente
            var incident = new Incident
            {
                Description = $"Incidente gerado por dados IoT do dispositivo {iotData.DeviceId}. Urgência: {urgencyLevel.Level}",
                Latitude = device.Latitude, // Assumindo que o dispositivo tem latitude e longitude
                Longitude = device.Longitude,
                StatusReport = "Novo dado IoT recebido",
                Source = "IoT",
                UrgencyLevel = urgencyLevel.Level,
                UrgencyScore = urgencyLevel.Score,
                UrgencyFactors = urgencyLevel.Factors,
                Status = "Aberto",
                Timestamp = DateTime.UtcNow,
                ReportedById = 1 // Assumindo um usuário padrão para incidentes IoT
            };

            await _unitOfWork.Incidents.AddAsync(incident);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Dados IoT recebidos e incidente processado", urgency = urgencyLevel.Level });
        }

        /// <summary>
        /// Registra manualmente um novo incidente.
        /// </summary>
        /// <param name="createIncidentDto">Dados do incidente manual.</param>
        /// <returns>Incidente criado e classificado.</returns>
        /// <response code="201">Incidente criado com sucesso.</response>
        /// <response code="400">Usuário reportador não encontrado.</response>
        /// <response code="500">Erro interno ao registrar o incidente.</response>
        [HttpPost("manual")]
        public async Task<IActionResult> CreateManualIncident([FromBody] CreateIncidentDTO createIncidentDto)
        {
            var user = (await _unitOfWork.Users.GetAllAsync())
                .FirstOrDefault(u => u.Email == createIncidentDto.ReportedBy);

            if (user == null)
            {
                return BadRequest(new { message = "Usuário reportador não encontrado" });
            }

            var incident = new Incident
            {
                Description = createIncidentDto.Description,
                Latitude = createIncidentDto.Location.Lat,
                Longitude = createIncidentDto.Location.Lng,
                StatusReport = createIncidentDto.StatusReport,
                Source = createIncidentDto.Source,
                Status = "Aberto",
                Timestamp = DateTime.UtcNow,
                ReportedById = user.Id
            };

            // Lógica de classificação de urgência baseada em IA (simplificada para o exemplo)
            var urgencyLevel = ClassifyUrgency(incident);
            incident.UrgencyLevel = urgencyLevel.Level;
            incident.UrgencyScore = urgencyLevel.Score;
            incident.UrgencyFactors = urgencyLevel.Factors;

            await _unitOfWork.Incidents.AddAsync(incident);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetIncidentById), new { id = incident.Id }, incident);
        }

        /// <summary>
        /// Retorna a classificação de urgência de um incidente existente.
        /// </summary>
        /// <param name="id">ID do incidente.</param>
        /// <returns>Classificação de urgência.</returns>
        /// <response code="200">Classificação retornada com sucesso.</response>
        /// <response code="404">Incidente não encontrado.</response>
        [HttpGet("{id}/urgency")]
        public async Task<ActionResult<UrgencyClassificationDTO>> GetUrgencyClassification(int id)
        {
            var incident = await _unitOfWork.Incidents.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { message = "Incidente não encontrado" });
            }

            var urgency = new UrgencyClassificationDTO
            {
                Level = incident.UrgencyLevel,
                Score = incident.UrgencyScore,
                Factors = incident.UrgencyFactors
            };

            return Ok(urgency);
        }

        /// <summary>
        /// Lista todos os incidentes cadastrados, com filtros opcionais por status e urgência.
        /// </summary>
        /// <param name="status">Status do incidente (Aberto, Em Andamento, Fechado).</param>
        /// <param name="urgency">Nível de urgência (Baixo, Médio, Alto, Crítico).</param>
        /// <returns>Lista filtrada de incidentes.</returns>
        /// <response code="200">Incidentes listados com sucesso.</response
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Incident>>> GetAllIncidents(string status = null, string urgency = null)
        {
            var incidents = await _unitOfWork.Incidents.GetAllAsync();

            if (!string.IsNullOrEmpty(status))
            {
                incidents = incidents.Where(i => i.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(urgency))
            {
                incidents = incidents.Where(i => i.UrgencyLevel.Equals(urgency, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(incidents);
        }

        /// <summary>
        /// Retorna os detalhes de um incidente específico.
        /// </summary>
        /// <param name="id">ID do incidente.</param>
        /// <returns>Detalhes do incidente.</returns>
        /// <response code="200">Incidente encontrado.</response>
        /// <response code="404">Incidente não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncidentById(int id)
        {
            var incident = await _unitOfWork.Incidents.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { message = "Incidente não encontrado" });
            }
            return Ok(incident);
        }

        /// <summary>
        /// Adiciona um comentário a um incidente existente.
        /// </summary>
        /// <param name="id">ID do incidente.</param>
        /// <param name="createCommentDto">Conteúdo do comentário.</param>
        /// <returns>Comentário registrado.</returns>
        /// <response code="201">Comentário adicionado com sucesso.</response>
        /// <response code="404">Incidente não encontrado.</response>
        /// <response code="500">Erro ao adicionar comentário.</response>
        [HttpPost("{id}/comment")]
        public async Task<IActionResult> AddCommentToIncident(int id, [FromBody] CreateCommentDTO createCommentDto)
        {
            var incident = await _unitOfWork.Incidents.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { message = "Incidente não encontrado" });
            }

            var comment = new IncidentComment
            {
                IncidentId = id,
                Content = createCommentDto.Content,
                UserId = createCommentDto.UserId,
                UserName = createCommentDto.UserName,
                Timestamp = DateTime.UtcNow
            };

            await _unitOfWork.IncidentComments.AddAsync(comment);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetIncidentById), new { id = incident.Id }, comment);
        }

        // Lógica de classificação de urgência (simplificada para o exemplo)
        private UrgencyClassification ClassifyUrgency(IoTData data)
        {
            int score = 0;
            string factors = "";

            if (data.Temperature > 30) { score += 10; factors += "Temperatura alta; "; }
            if (data.Humidity < 20) { score += 5; factors += "Umidade baixa; "; }
            if (data.Distance < 100) { score += 15; factors += "Proximidade de objeto; "; }

            if (data.AccelerometerX > 1 || data.AccelerometerY > 1 || data.AccelerometerZ > 1) { score += 20; factors += "Movimento brusco; "; }

            return DetermineUrgencyLevel(score, factors);
        }

        private UrgencyClassification ClassifyUrgency(Incident incident)
        {
            int score = 0;
            string factors = "";

            // Palavras-chave
            if (incident.Description.ToLower().Contains("pânico")) { score += 20; factors += "Palavra-chave: pânico; "; }
            if (incident.Description.ToLower().Contains("emergência")) { score += 15; factors += "Palavra-chave: emergência; "; }
            if (incident.Description.ToLower().Contains("enchente")) { score += 25; factors += "Palavra-chave: enchente; "; }
            if (incident.Description.ToLower().Contains("incêndio")) { score += 30; factors += "Palavra-chave: incêndio; "; }

            // Fonte
            if (incident.Source == "IoT") { score += 10; factors += "Fonte: IoT; "; }

            // Localização (simplificado)
            // if (incident.Latitude > X && incident.Longitude < Y) { score += 5; factors += "Área de risco; "; }

            return DetermineUrgencyLevel(score, factors);
        }

        private UrgencyClassification DetermineUrgencyLevel(int score, string factors)
        {
            string level;
            if (score >= 80) level = "Crítico";
            else if (score >= 60) level = "Alto";
            else if (score >= 40) level = "Médio";
            else level = "Baixo";

            return new UrgencyClassification { Level = level, Score = score, Factors = factors };
        }

        private class UrgencyClassification
        {
            public string Level { get; set; }
            public int Score { get; set; }
            public string Factors { get; set; }
        }
    }
}


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
    public class DeviceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Obtém o status de todos os dispositivos IoT registrados.
        /// </summary>
        /// <returns>Lista de dispositivos com informações de status e última comunicação.</returns>
        /// <response code="200">Retorna a lista de dispositivos</response>
        /// <response code="500">Erro interno ao acessar os dados</response>
        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<DeviceStatusDTO>>> GetDeviceStatus()
        {
            var devices = await _unitOfWork.Devices.GetAllAsync();
            var deviceStatusList = new List<DeviceStatusDTO>();

            foreach (var device in devices)
            {
                deviceStatusList.Add(new DeviceStatusDTO
                {
                    Id = device.Id,
                    Name = device.Name,
                    Type = device.Type,
                    Status = device.Status,
                    LastCommunication = device.LastCommunication
                });
            }
            return Ok(deviceStatusList);
        }
        /// <summary>
        /// Registra um novo dispositivo IoT no sistema.
        /// </summary>
        /// <param name="createDeviceDto">Objeto contendo as informações do novo dispositivo.</param>
        /// <returns>Dados do dispositivo criado.</returns>
        /// <response code="201">Dispositivo criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno ao registrar o dispositivo</response>
        [HttpPost]
        public async Task<ActionResult<Device>> RegisterDevice([FromBody] CreateDeviceDTO createDeviceDto)
        {
            var device = new Device
            {
                Name = createDeviceDto.Name,
                Type = createDeviceDto.Type,
                Location = createDeviceDto.Location,
                Status = "Online", // Status inicial
                LastCommunication = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Devices.AddAsync(device);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetDeviceStatus), new { id = device.Id }, device);
        }

        /// <summary>
        /// Atualiza o status de um dispositivo IoT.
        /// </summary>
        /// <param name="id">ID do dispositivo a ser atualizado.</param>
        /// <param name="updateDeviceStatusDto">Novo status do dispositivo.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Status atualizado com sucesso</response>
        /// <response code="404">Dispositivo não encontrado</response>
        /// <response code="500">Erro ao atualizar o status</response>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateDeviceStatus(int id, [FromBody] UpdateDeviceStatusDTO updateDeviceStatusDto)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync(id);
            if (device == null)
            {
                return NotFound(new { message = "Dispositivo não encontrado" });
            }

            device.Status = updateDeviceStatusDto.Status;
            device.LastCommunication = DateTime.UtcNow;
            _unitOfWork.Devices.Update(device);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Status do dispositivo atualizado com sucesso" });
        }
    }
}


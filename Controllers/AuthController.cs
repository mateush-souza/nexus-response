using Microsoft.AspNetCore.Mvc;
using nexus_response.DTOs;
using nexus_response.Models;
using nexus_response.Persistence;
using Microsoft.EntityFrameworkCore;

namespace nexus_response.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Realiza o login de um usuário no sistema.
        /// </summary>
        /// <param name="loginDto">Credenciais de login do usuário.</param>
        /// <returns>Token de autenticação e dados do usuário.</returns>
        /// <response code="200">Login realizado com sucesso.</response>
        /// <response code="401">Credenciais inválidas.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var user = (await _unitOfWork.Users.GetAllAsync())
                    .FirstOrDefault(u => u.Email == loginDto.Email);

                if (user == null || !VerifyPassword(loginDto.Password, user.Password))
                {
                    return Unauthorized(new { message = "Email ou senha inválidos" });
                }

                var response = new AuthResponseDTO
                {
                    Token = GenerateToken(user),
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="registerDto">Dados do novo usuário.</param>
        /// <returns>Dados do usuário criado com token de autenticação.</returns>
        /// <response code="201">Usuário registrado com sucesso.</response>
        /// <response code="400">Email ou CPF já em uso.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var existingUser = (await _unitOfWork.Users.GetAllAsync())
                    .FirstOrDefault(u => u.Email == registerDto.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email já está em uso" });
                }

                var existingCpf = (await _unitOfWork.Users.GetAllAsync())
                    .FirstOrDefault(u => u.CPF == registerDto.CPF);

                if (existingCpf != null)
                {
                    return BadRequest(new { message = "CPF já está em uso" });
                }

                var user = new User
                {
                    Name = registerDto.Name,
                    CPF = registerDto.CPF,
                    Email = registerDto.Email,
                    Password = HashPassword(registerDto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDTO
                {
                    Token = GenerateToken(user),
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };

                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            return hash == hashedPassword;
        }

        private string GenerateToken(User user)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.Id}:{user.Email}:{DateTime.UtcNow.Ticks}"));
        }
    }
}


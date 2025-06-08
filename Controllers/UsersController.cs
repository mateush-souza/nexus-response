using Microsoft.AspNetCore.Mvc;
using nexus_response.DTOs;
using nexus_response.Models;
using nexus_response.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nexus_response.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retorna todos os usuários registrados no sistema.
        /// </summary>
        /// <returns>Lista de usuários com nome, e-mail e ID.</returns>
        /// <response code="200">Lista de usuários retornada com sucesso.</response>
        /// <response code="500">Erro interno ao recuperar os usuários.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfoDTO>>> GetAllUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDTOs = new List<UserInfoDTO>();
            foreach (var user in users)
            {
                userDTOs.Add(new UserInfoDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });
            }
            return Ok(userDTOs);
        }

        /// <summary>
        /// Retorna um usuário específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Informações básicas do usuário.</returns>
        /// <response code="200">Usuário encontrado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfoDTO>> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }
            var userDto = new UserInfoDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
            return Ok(userDto);
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="createUserDto">Objeto contendo nome, email, CPF e senha.</param>
        /// <returns>Usuário criado com sucesso.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Email ou CPF já em uso.</response>
        /// <response code="500">Erro interno ao criar usuário.</response>
        [HttpPost]
        public async Task<ActionResult<UserInfoDTO>> CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            // Verificar se o email já existe
            var existingUser = (await _unitOfWork.Users.GetAllAsync())
                .FirstOrDefault(u => u.Email == createUserDto.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Email já está em uso" });
            }

            // Verificar se o CPF já existe
            var existingCpf = (await _unitOfWork.Users.GetAllAsync())
                .FirstOrDefault(u => u.CPF == createUserDto.CPF);

            if (existingCpf != null)
            {
                return BadRequest(new { message = "CPF já está em uso" });
            }

            var user = new User
            {
                Name = createUserDto.Name,
                CPF = createUserDto.CPF,
                Email = createUserDto.Email,
                Password = HashPassword(createUserDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var userDto = new UserInfoDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
        }

        /// <summary>
        /// Atualiza dados de um usuário existente, incluindo senha se necessário.
        /// </summary>
        /// <param name="id">ID do usuário a ser atualizado.</param>
        /// <param name="updateUserDto">Objeto com dados de atualização.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Usuário atualizado com sucesso.</response>
        /// <response code="400">Senha atual inválida ou ausente.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="500">Erro interno ao atualizar.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
            {
                if (string.IsNullOrEmpty(updateUserDto.CurrentPassword))
                {
                    return BadRequest(new { message = "Senha atual é obrigatória para alterar a senha" });
                }

                if (!VerifyPassword(updateUserDto.CurrentPassword, user.Password))
                {
                    return BadRequest(new { message = "Senha atual incorreta" });
                }

                user.Password = HashPassword(updateUserDto.NewPassword);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Name))
            {
                user.Name = updateUserDto.Name;
            }

            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Usuário atualizado com sucesso" });
        }

        /// <summary>
        /// Deleta permanentemente um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário a ser deletado.</param>
        /// <returns>Mensagem de confirmação.</returns>
        /// <response code="200">Usuário deletado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="500">Erro ao tentar deletar o usuário.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Usuário deletado com sucesso" });
        }

        private string HashPassword(string password)
        {
            // Em um ambiente de produção, use BCrypt ou similar
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Em um ambiente de produção, use BCrypt ou similar
            var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            return hash == hashedPassword;
        }
    }
}


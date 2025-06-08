namespace nexus_response.DTOs
{
    public class UserInfoDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }

    public class CreateUserDTO
    {
        public string Name { get; set; }

        public string CPF { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class UpdateUserDTO
    {
        public string Name { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}


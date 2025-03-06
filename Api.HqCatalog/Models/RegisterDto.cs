using HqCatalog.Business.Models;

namespace HqCatalog.Api.Models
{
    public class RegisterDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Tipo { get; set; } // Definir tipo de usuário ao registrar
    }
}

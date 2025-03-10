using HqCatalog.Business.Models;

namespace HqCatalog.Api.Models
{
    public class ApplicationUserRegisterDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nome { get; set; }
    }
}

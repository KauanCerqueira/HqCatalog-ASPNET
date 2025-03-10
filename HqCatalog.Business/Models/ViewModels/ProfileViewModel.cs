using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Business.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser CurrentUser { get; set; } = new ApplicationUser();
        public List<ApplicationUser> UsersList { get; set; } = new List<ApplicationUser>();

        // Campos adicionais (exemplo)
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}

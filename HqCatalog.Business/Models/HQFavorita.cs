using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HqCatalog.Business.Models;

namespace HqCatalog.Business.Models
{
    public class HQFavorita
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int HQId { get; set; }
        public virtual User Usuario { get; set; }
        public virtual Hq HQ { get; set; }
    }
}

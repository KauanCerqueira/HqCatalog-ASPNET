using HqCatalog.Business.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqCatalog.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public IDbSet<Hq> Hq { get; set; }
    }
}

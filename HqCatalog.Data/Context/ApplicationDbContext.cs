using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HqCatalog.Business.Models;

namespace HqCatalog.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Hq> HQs { get; set; }
        public DbSet<HQFavorita> HQsFavoritas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // 🔹 Mantém as configurações do Identity

            modelBuilder.Entity<HQFavorita>()
                .HasKey(hqf => new { hqf.UsuarioId, hqf.HQId });
        }
    }
}

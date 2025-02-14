using Microsoft.EntityFrameworkCore;
using HqCatalog.Business.Models;

namespace HqCatalog.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Hq> HQs { get; set; }
        public DbSet<Prateleira> Prateleiras { get; set; }
        public DbSet<User> Usuarios { get; set; }
        public DbSet<HQFavorita> HQsFavoritas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HQFavorita>()
                .HasKey(hqf => new { hqf.UsuarioId, hqf.HQId });

            modelBuilder.Entity<Hq>()
                .HasOne(h => h.Prateleira)
                .WithMany(p => p.HQs)
                .HasForeignKey(h => h.PrateleiraId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

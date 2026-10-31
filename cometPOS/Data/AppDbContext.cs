using Microsoft.EntityFrameworkCore;
using cometPOS.Models;
namespace cometPOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { 
        }

        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sucursal>()
                .HasIndex(s => s.Codigo)
                .IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}

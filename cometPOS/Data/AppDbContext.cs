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

        public DbSet<Empresa> Empresas => Set<Empresa>();
        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.Identificador)
                .IsUnique();
            modelBuilder.Entity<Sucursal>()
                .HasOne(s => s.Empresa)
                .WithMany(e => e.Sucursales)
                .HasForeignKey(s => s.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Empresa)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(u => u.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Sucursal>()
                .HasIndex(s => s.Codigo)
                .IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => r.TokenHash)
                .IsUnique();
            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AuditLog>(e =>
            {
                e.Property(a => a.Detalles).HasColumnType("jsonb");
                e.HasIndex(a => a.FechaUtc);
                e.HasIndex(a => new { a.Entidad, a.EntidadId });
                e.HasIndex(a => a.UsuarioId);
                e.HasIndex(a => a.EmpresaId);
            });
        }
    }
}

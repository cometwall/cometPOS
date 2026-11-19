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

        // ===== Catálogo =====
        public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<Producto> Productos => Set<Producto>();

        // ===== Retornables =====
        public DbSet<Retornable> Retornables => Set<Retornable>();

        // ===== Variantes =====
        public DbSet<ProductoVariante> ProductoVariantes => Set<ProductoVariante>();
        public DbSet<Atributo> Atributos => Set<Atributo>();
        public DbSet<AtributoValor> AtributoValores => Set<AtributoValor>();
        public DbSet<ProductoAtributo> ProductoAtributos => Set<ProductoAtributo>();
        public DbSet<VarianteValor> VarianteValores => Set<VarianteValor>();
        public DbSet<VarianteRetornable> VarianteRetornables => Set<VarianteRetornable>();

        // ===== Identificación y unidades =====
        public DbSet<IdentificadorProducto> IdentificadoresProducto => Set<IdentificadorProducto>();
        public DbSet<ProductoUnidad> ProductoUnidades => Set<ProductoUnidad>();

        // ===== Asignación e inventario =====
        public DbSet<SucursalProducto> SucursalProductos => Set<SucursalProducto>();
        public DbSet<InventarioSucursal> InventarioSucursal => Set<InventarioSucursal>();

        // ===== Trazabilidad =====
        public DbSet<ProductoLote> ProductoLotes => Set<ProductoLote>();
        public DbSet<InventarioLote> InventarioLotes => Set<InventarioLote>();
        public DbSet<ProductoSerie> ProductoSeries => Set<ProductoSerie>();
        public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();

        // ===== Inventario de retornables =====
        public DbSet<InventarioRetornable> InventarioRetornables => Set<InventarioRetornable>();
        public DbSet<MovimientoRetornable> MovimientosRetornables => Set<MovimientoRetornable>();

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

            ConfigureInventario(modelBuilder);
        }

        private static void ConfigureInventario(ModelBuilder modelBuilder)
        {
            // ----- Catálogo -----
            modelBuilder.Entity<UnidadMedida>()
                .HasOne(u => u.Empresa)
                .WithMany()
                .HasForeignKey(u => u.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadMedida>()
                .HasIndex(u => new { u.EmpresaId, u.Abreviatura }).IsUnique();

            modelBuilder.Entity<Categoria>(e =>
            {
                e.HasOne(c => c.Empresa)
                    .WithMany()
                    .HasForeignKey(c => c.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(c => c.CategoriaPadre)
                    .WithMany(c => c.Subcategorias)
                    .HasForeignKey(c => c.CategoriaPadreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Categoria>().HasIndex(c => new { c.EmpresaId, c.Nombre }).IsUnique();

            modelBuilder.Entity<Marca>()
                .HasOne(m => m.Empresa)
                .WithMany()
                .HasForeignKey(m => m.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Marca>().HasIndex(m => new { m.EmpresaId, m.Nombre }).IsUnique();

            modelBuilder.Entity<Producto>(e =>
            {
                e.HasOne(p => p.Empresa)
                    .WithMany()
                    .HasForeignKey(p => p.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Categoria)
                    .WithMany()
                    .HasForeignKey(p => p.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Marca)
                    .WithMany()
                    .HasForeignKey(p => p.MarcaId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.UnidadBase)
                    .WithMany()
                    .HasForeignKey(p => p.UnidadBaseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Retornables -----
            modelBuilder.Entity<Retornable>()
                .HasOne(r => r.Empresa)
                .WithMany()
                .HasForeignKey(r => r.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- Variantes -----
            modelBuilder.Entity<ProductoVariante>(e =>
            {
                e.HasOne(v => v.Empresa)
                    .WithMany()
                    .HasForeignKey(v => v.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(v => v.Producto)
                    .WithMany(p => p.Variantes)
                    .HasForeignKey(v => v.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(v => new { v.EmpresaId, v.Sku }).IsUnique();
            });

            modelBuilder.Entity<Atributo>()
                .HasOne(a => a.Empresa)
                .WithMany()
                .HasForeignKey(a => a.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AtributoValor>()
                .HasOne(av => av.Atributo)
                .WithMany()
                .HasForeignKey(av => av.AtributoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductoAtributo>(e =>
            {
                e.HasKey(pa => new { pa.ProductoId, pa.AtributoId });
                e.HasOne(pa => pa.Producto)
                    .WithMany(p => p.Atributos)
                    .HasForeignKey(pa => pa.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(pa => pa.Atributo)
                    .WithMany()
                    .HasForeignKey(pa => pa.AtributoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VarianteValor>(e =>
            {
                e.HasKey(vv => new { vv.VarianteId, vv.AtributoId });
                e.HasOne(vv => vv.Variante)
                    .WithMany()
                    .HasForeignKey(vv => vv.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(vv => vv.Atributo)
                    .WithMany()
                    .HasForeignKey(vv => vv.AtributoId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(vv => vv.AtributoValor)
                    .WithMany()
                    .HasForeignKey(vv => vv.AtributoValorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VarianteRetornable>(e =>
            {
                e.HasKey(vr => new { vr.VarianteId, vr.RetornableId });
                e.HasOne(vr => vr.Variante)
                    .WithMany()
                    .HasForeignKey(vr => vr.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(vr => vr.Retornable)
                    .WithMany()
                    .HasForeignKey(vr => vr.RetornableId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Identificación y unidades -----
            modelBuilder.Entity<IdentificadorProducto>(e =>
            {
                e.HasOne(ip => ip.Empresa)
                    .WithMany()
                    .HasForeignKey(ip => ip.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(ip => ip.Variante)
                    .WithMany()
                    .HasForeignKey(ip => ip.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(ip => ip.Unidad)
                    .WithMany()
                    .HasForeignKey(ip => ip.UnidadId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(ip => new { ip.EmpresaId, ip.Codigo }).IsUnique();
            });

            modelBuilder.Entity<ProductoUnidad>(e =>
            {
                e.HasKey(pu => new { pu.VarianteId, pu.UnidadId });
                e.HasOne(pu => pu.Variante)
                    .WithMany()
                    .HasForeignKey(pu => pu.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(pu => pu.Unidad)
                    .WithMany()
                    .HasForeignKey(pu => pu.UnidadId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Asignación e inventario -----
            modelBuilder.Entity<SucursalProducto>(e =>
            {
                e.HasKey(sp => new { sp.SucursalId, sp.VarianteId });
                e.HasOne(sp => sp.Sucursal)
                    .WithMany()
                    .HasForeignKey(sp => sp.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(sp => sp.Variante)
                    .WithMany()
                    .HasForeignKey(sp => sp.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventarioSucursal>(e =>
            {
                e.HasKey(i => new { i.SucursalId, i.VarianteId });
                e.HasOne(i => i.SucursalProducto)
                    .WithOne(sp => sp.Inventario)
                    .HasForeignKey<InventarioSucursal>(i => new { i.SucursalId, i.VarianteId })
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ----- Trazabilidad -----
            modelBuilder.Entity<ProductoLote>()
                .HasOne(l => l.Variante)
                .WithMany()
                .HasForeignKey(l => l.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventarioLote>(e =>
            {
                e.HasKey(il => new { il.SucursalId, il.LoteId });
                e.HasOne(il => il.Sucursal)
                    .WithMany()
                    .HasForeignKey(il => il.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(il => il.Lote)
                    .WithMany()
                    .HasForeignKey(il => il.LoteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(il => il.Variante)
                    .WithMany()
                    .HasForeignKey(il => il.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductoSerie>(e =>
            {
                e.HasOne(s => s.Variante)
                    .WithMany()
                    .HasForeignKey(s => s.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(s => s.Lote)
                    .WithMany()
                    .HasForeignKey(s => s.LoteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(s => s.Sucursal)
                    .WithMany()
                    .HasForeignKey(s => s.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(s => s.NumeroSerie);
            });

            modelBuilder.Entity<MovimientoInventario>(e =>
            {
                e.HasOne(m => m.Sucursal)
                    .WithMany()
                    .HasForeignKey(m => m.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Variante)
                    .WithMany()
                    .HasForeignKey(m => m.VarianteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Lote)
                    .WithMany()
                    .HasForeignKey(m => m.LoteId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Serie)
                    .WithMany()
                    .HasForeignKey(m => m.SerieId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Usuario)
                    .WithMany()
                    .HasForeignKey(m => m.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(m => m.FechaMovimiento);
                e.HasIndex(m => new { m.ReferenciaTipo, m.ReferenciaId });
            });

            // ----- Inventario de retornables -----
            modelBuilder.Entity<InventarioRetornable>(e =>
            {
                e.HasKey(ir => new { ir.SucursalId, ir.RetornableId });
                e.HasOne(ir => ir.Sucursal)
                    .WithMany()
                    .HasForeignKey(ir => ir.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(ir => ir.Retornable)
                    .WithMany()
                    .HasForeignKey(ir => ir.RetornableId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MovimientoRetornable>(e =>
            {
                e.HasOne(m => m.Sucursal)
                    .WithMany()
                    .HasForeignKey(m => m.SucursalId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Retornable)
                    .WithMany()
                    .HasForeignKey(m => m.RetornableId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Usuario)
                    .WithMany()
                    .HasForeignKey(m => m.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(m => m.FechaMovimiento);
            });
        }
    }
}

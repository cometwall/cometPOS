using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace cometPOS.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atributos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atributos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atributos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    CategoriaPadreId = table.Column<int>(type: "integer", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_Categorias_CategoriaPadreId",
                        column: x => x.CategoriaPadreId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categorias_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marcas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Retornables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    ValorDeposito = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Costo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    EsPropio = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Retornables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Retornables_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Abreviatura = table.Column<string>(type: "text", nullable: false),
                    ClaveSat = table.Column<string>(type: "text", nullable: true),
                    PermiteDecimales = table.Column<bool>(type: "boolean", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnidadesMedida_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AtributoValores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtributoId = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtributoValores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtributoValores_Atributos_AtributoId",
                        column: x => x.AtributoId,
                        principalTable: "Atributos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventarioRetornables",
                columns: table => new
                {
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    RetornableId = table.Column<int>(type: "integer", nullable: false),
                    CantidadVacios = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioRetornables", x => new { x.SucursalId, x.RetornableId });
                    table.ForeignKey(
                        name: "FK_InventarioRetornables_Retornables_RetornableId",
                        column: x => x.RetornableId,
                        principalTable: "Retornables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioRetornables_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosRetornables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    RetornableId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ValorDeposito = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ReferenciaTipo = table.Column<string>(type: "text", nullable: true),
                    ReferenciaId = table.Column<long>(type: "bigint", nullable: true),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosRetornables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosRetornables_Retornables_RetornableId",
                        column: x => x.RetornableId,
                        principalTable: "Retornables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosRetornables_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosRetornables_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CategoriaId = table.Column<int>(type: "integer", nullable: true),
                    MarcaId = table.Column<int>(type: "integer", nullable: true),
                    UnidadBaseId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    ControlaInventario = table.Column<bool>(type: "boolean", nullable: false),
                    TipoSeguimiento = table.Column<string>(type: "text", nullable: false),
                    ClaveSatProdServ = table.Column<string>(type: "text", nullable: true),
                    Especificaciones = table.Column<string>(type: "jsonb", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_UnidadesMedida_UnidadBaseId",
                        column: x => x.UnidadBaseId,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoAtributos",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    AtributoId = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoAtributos", x => new { x.ProductoId, x.AtributoId });
                    table.ForeignKey(
                        name: "FK_ProductoAtributos_Atributos_AtributoId",
                        column: x => x.AtributoId,
                        principalTable: "Atributos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoAtributos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoVariantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    Sku = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    EsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoVariantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoVariantes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoVariantes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentificadoresProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    UnidadId = table.Column<int>(type: "integer", nullable: true),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificadoresProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentificadoresProducto_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdentificadoresProducto_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdentificadoresProducto_UnidadesMedida_UnidadId",
                        column: x => x.UnidadId,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoLotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    NumeroLote = table.Column<string>(type: "text", nullable: false),
                    FechaFabricacion = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaCaducidad = table.Column<DateOnly>(type: "date", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoLotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoLotes_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoUnidades",
                columns: table => new
                {
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    UnidadId = table.Column<int>(type: "integer", nullable: false),
                    Factor = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    EsVenta = table.Column<bool>(type: "boolean", nullable: false),
                    EsCompra = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoUnidades", x => new { x.VarianteId, x.UnidadId });
                    table.ForeignKey(
                        name: "FK_ProductoUnidades_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoUnidades_UnidadesMedida_UnidadId",
                        column: x => x.UnidadId,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SucursalProductos",
                columns: table => new
                {
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    StockMinimo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    StockMaximo = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PuntoReorden = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SucursalProductos", x => new { x.SucursalId, x.VarianteId });
                    table.ForeignKey(
                        name: "FK_SucursalProductos_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SucursalProductos_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VarianteRetornables",
                columns: table => new
                {
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    RetornableId = table.Column<int>(type: "integer", nullable: false),
                    CantidadPorUnidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarianteRetornables", x => new { x.VarianteId, x.RetornableId });
                    table.ForeignKey(
                        name: "FK_VarianteRetornables_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VarianteRetornables_Retornables_RetornableId",
                        column: x => x.RetornableId,
                        principalTable: "Retornables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VarianteValores",
                columns: table => new
                {
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    AtributoId = table.Column<int>(type: "integer", nullable: false),
                    AtributoValorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarianteValores", x => new { x.VarianteId, x.AtributoId });
                    table.ForeignKey(
                        name: "FK_VarianteValores_AtributoValores_AtributoValorId",
                        column: x => x.AtributoValorId,
                        principalTable: "AtributoValores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VarianteValores_Atributos_AtributoId",
                        column: x => x.AtributoId,
                        principalTable: "Atributos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VarianteValores_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventarioLotes",
                columns: table => new
                {
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    LoteId = table.Column<int>(type: "integer", nullable: false),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioLotes", x => new { x.SucursalId, x.LoteId });
                    table.ForeignKey(
                        name: "FK_InventarioLotes_ProductoLotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "ProductoLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioLotes_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioLotes_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    NumeroSerie = table.Column<string>(type: "text", nullable: false),
                    LoteId = table.Column<int>(type: "integer", nullable: true),
                    SucursalId = table.Column<int>(type: "integer", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoSeries_ProductoLotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "ProductoLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoSeries_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoSeries_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventarioSucursal",
                columns: table => new
                {
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CantidadReservada = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoPromedio = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioSucursal", x => new { x.SucursalId, x.VarianteId });
                    table.ForeignKey(
                        name: "FK_InventarioSucursal_SucursalProductos_SucursalId_VarianteId",
                        columns: x => new { x.SucursalId, x.VarianteId },
                        principalTable: "SucursalProductos",
                        principalColumns: new[] { "SucursalId", "VarianteId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    VarianteId = table.Column<int>(type: "integer", nullable: false),
                    LoteId = table.Column<int>(type: "integer", nullable: true),
                    SerieId = table.Column<int>(type: "integer", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ReferenciaTipo = table.Column<string>(type: "text", nullable: true),
                    ReferenciaId = table.Column<long>(type: "bigint", nullable: true),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_ProductoLotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "ProductoLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_ProductoSeries_SerieId",
                        column: x => x.SerieId,
                        principalTable: "ProductoSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atributos_EmpresaId",
                table: "Atributos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_AtributoValores_AtributoId",
                table: "AtributoValores",
                column: "AtributoId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_CategoriaPadreId",
                table: "Categorias",
                column: "CategoriaPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_EmpresaId",
                table: "Categorias",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificadoresProducto_Codigo",
                table: "IdentificadoresProducto",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificadoresProducto_EmpresaId",
                table: "IdentificadoresProducto",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificadoresProducto_UnidadId",
                table: "IdentificadoresProducto",
                column: "UnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentificadoresProducto_VarianteId",
                table: "IdentificadoresProducto",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioLotes_LoteId",
                table: "InventarioLotes",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioLotes_VarianteId",
                table: "InventarioLotes",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioRetornables_RetornableId",
                table: "InventarioRetornables",
                column: "RetornableId");

            migrationBuilder.CreateIndex(
                name: "IX_Marcas_EmpresaId",
                table: "Marcas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_FechaMovimiento",
                table: "MovimientosInventario",
                column: "FechaMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_LoteId",
                table: "MovimientosInventario",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_ReferenciaTipo_ReferenciaId",
                table: "MovimientosInventario",
                columns: new[] { "ReferenciaTipo", "ReferenciaId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_SerieId",
                table: "MovimientosInventario",
                column: "SerieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_SucursalId",
                table: "MovimientosInventario",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_UsuarioId",
                table: "MovimientosInventario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_VarianteId",
                table: "MovimientosInventario",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRetornables_FechaMovimiento",
                table: "MovimientosRetornables",
                column: "FechaMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRetornables_RetornableId",
                table: "MovimientosRetornables",
                column: "RetornableId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRetornables_SucursalId",
                table: "MovimientosRetornables",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRetornables_UsuarioId",
                table: "MovimientosRetornables",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoAtributos_AtributoId",
                table: "ProductoAtributos",
                column: "AtributoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoLotes_VarianteId",
                table: "ProductoLotes",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_EmpresaId",
                table: "Productos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_MarcaId",
                table: "Productos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnidadBaseId",
                table: "Productos",
                column: "UnidadBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoSeries_LoteId",
                table: "ProductoSeries",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoSeries_NumeroSerie",
                table: "ProductoSeries",
                column: "NumeroSerie");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoSeries_SucursalId",
                table: "ProductoSeries",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoSeries_VarianteId",
                table: "ProductoSeries",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoUnidades_UnidadId",
                table: "ProductoUnidades",
                column: "UnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_EmpresaId",
                table: "ProductoVariantes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_ProductoId",
                table: "ProductoVariantes",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_Sku",
                table: "ProductoVariantes",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Retornables_EmpresaId",
                table: "Retornables",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_SucursalProductos_VarianteId",
                table: "SucursalProductos",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_EmpresaId",
                table: "UnidadesMedida",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_VarianteRetornables_RetornableId",
                table: "VarianteRetornables",
                column: "RetornableId");

            migrationBuilder.CreateIndex(
                name: "IX_VarianteValores_AtributoId",
                table: "VarianteValores",
                column: "AtributoId");

            migrationBuilder.CreateIndex(
                name: "IX_VarianteValores_AtributoValorId",
                table: "VarianteValores",
                column: "AtributoValorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentificadoresProducto");

            migrationBuilder.DropTable(
                name: "InventarioLotes");

            migrationBuilder.DropTable(
                name: "InventarioRetornables");

            migrationBuilder.DropTable(
                name: "InventarioSucursal");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "MovimientosRetornables");

            migrationBuilder.DropTable(
                name: "ProductoAtributos");

            migrationBuilder.DropTable(
                name: "ProductoUnidades");

            migrationBuilder.DropTable(
                name: "VarianteRetornables");

            migrationBuilder.DropTable(
                name: "VarianteValores");

            migrationBuilder.DropTable(
                name: "SucursalProductos");

            migrationBuilder.DropTable(
                name: "ProductoSeries");

            migrationBuilder.DropTable(
                name: "Retornables");

            migrationBuilder.DropTable(
                name: "AtributoValores");

            migrationBuilder.DropTable(
                name: "ProductoLotes");

            migrationBuilder.DropTable(
                name: "Atributos");

            migrationBuilder.DropTable(
                name: "ProductoVariantes");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");
        }
    }
}

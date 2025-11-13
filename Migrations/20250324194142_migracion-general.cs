using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIS_DIAF.Migrations
{
    /// <inheritdoc />
    public partial class migraciongeneral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Historico",
                columns: table => new
                {
                    historico_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    historico_archivoRuta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    historico_guidRegimen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    historico_guidArchivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    historico_tipoArchivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    historico_fecha = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historico", x => x.historico_id);
                });

            migrationBuilder.CreateTable(
                name: "Regimens",
                columns: table => new
                {
                    regimen_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    regimen_guid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    regimen_cod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    regimen_presupuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    regimen_objetivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    regimen_fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regimens", x => x.regimen_id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    rol_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rol_descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rol_estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rol_fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.rol_id);
                });

            migrationBuilder.CreateTable(
                name: "ServicioCorreo",
                columns: table => new
                {
                    srVCorreo_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    srvCorreo_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    srvCorreo_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    srvCorreo_host = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    srvCorreo_port = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicioCorreo", x => x.srVCorreo_id);
                });

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    sucursal_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sucursal_nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sucursal_fechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sucursal_estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.sucursal_id);
                });

            migrationBuilder.CreateTable(
                name: "TipoRegs",
                columns: table => new
                {
                    tiporeg_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_tipo_reg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoRegs", x => x.tiporeg_id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    usuario_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usuario_nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    usuario_cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    usuario_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    usuario_celular = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    usuario_passwd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    usuario_estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    usuario_nIntentos = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<long>(type: "bigint", nullable: false),
                    SucursalId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.usuario_id);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "rol_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuario_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "sucursal_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoArchivos",
                columns: table => new
                {
                    tipoarchivo_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo_nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tip_descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tip_prioridad = table.Column<int>(type: "int", nullable: false),
                    tip_fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoRegId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoArchivos", x => x.tipoarchivo_id);
                    table.ForeignKey(
                        name: "FK_TipoArchivos_TipoRegs_TipoRegId",
                        column: x => x.TipoRegId,
                        principalTable: "TipoRegs",
                        principalColumn: "tiporeg_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponsableRegimens",
                columns: table => new
                {
                    responsableReg_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    responsableReg_codigoRegimen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    responsableReg_estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    responsableReg_usuarioId = table.Column<long>(type: "bigint", nullable: false),
                    responsableReg_regimenId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsableRegimens", x => x.responsableReg_id);
                    table.ForeignKey(
                        name: "FK_ResponsableRegimens_Regimens_responsableReg_regimenId",
                        column: x => x.responsableReg_regimenId,
                        principalTable: "Regimens",
                        principalColumn: "regimen_id");
                    table.ForeignKey(
                        name: "FK_ResponsableRegimens_Usuario_responsableReg_usuarioId",
                        column: x => x.responsableReg_usuarioId,
                        principalTable: "Usuario",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Archivos",
                columns: table => new
                {
                    archivo_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    archivo_guid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    archivo_nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    archivo_ruta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_subida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    archivo_estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegimenId = table.Column<long>(type: "bigint", nullable: false),
                    TipoArchivoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archivos", x => x.archivo_id);
                    table.ForeignKey(
                        name: "FK_Archivos_Regimens_RegimenId",
                        column: x => x.RegimenId,
                        principalTable: "Regimens",
                        principalColumn: "regimen_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Archivos_TipoArchivos_TipoArchivoId",
                        column: x => x.TipoArchivoId,
                        principalTable: "TipoArchivos",
                        principalColumn: "tipoarchivo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_RegimenId",
                table: "Archivos",
                column: "RegimenId");

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_TipoArchivoId",
                table: "Archivos",
                column: "TipoArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsableRegimens_responsableReg_regimenId",
                table: "ResponsableRegimens",
                column: "responsableReg_regimenId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsableRegimens_responsableReg_usuarioId",
                table: "ResponsableRegimens",
                column: "responsableReg_usuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoArchivos_TipoRegId",
                table: "TipoArchivos",
                column: "TipoRegId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RolId",
                table: "Usuario",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_SucursalId",
                table: "Usuario",
                column: "SucursalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Archivos");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "Historico");

            migrationBuilder.DropTable(
                name: "ResponsableRegimens");

            migrationBuilder.DropTable(
                name: "ServicioCorreo");

            migrationBuilder.DropTable(
                name: "TipoArchivos");

            migrationBuilder.DropTable(
                name: "Regimens");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "TipoRegs");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Sucursales");
        }
    }
}

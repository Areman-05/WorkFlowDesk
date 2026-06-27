using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowDesk.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosActividad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: true),
                    EmpleadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Entidad = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EntidadId = table.Column<int>(type: "INTEGER", nullable: true),
                    Accion = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Detalle = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosActividad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosActividad_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosActividad_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosTiempo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TareaId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmpleadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Minutos = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Nota = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosTiempo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subtareas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TareaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Completada = table.Column<bool>(type: "INTEGER", nullable: false),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtareas_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TareaAdjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TareaId = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreArchivo = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    RutaRelativa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TamanoBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubidoPorEmpleadoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaAdjuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TareaAdjuntos_Empleados_SubidoPorEmpleadoId",
                        column: x => x.SubidoPorEmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TareaAdjuntos_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TareaDependencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TareaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DependeDeTareaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaDependencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TareaDependencias_Tareas_DependeDeTareaId",
                        column: x => x.DependeDeTareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TareaDependencias_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosActividad_EmpleadoId",
                table: "RegistrosActividad",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosActividad_FechaUtc",
                table: "RegistrosActividad",
                column: "FechaUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosActividad_UsuarioId",
                table: "RegistrosActividad",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_EmpleadoId",
                table: "RegistrosTiempo",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_TareaId",
                table: "RegistrosTiempo",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtareas_TareaId",
                table: "Subtareas",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_TareaAdjuntos_SubidoPorEmpleadoId",
                table: "TareaAdjuntos",
                column: "SubidoPorEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_TareaAdjuntos_TareaId",
                table: "TareaAdjuntos",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_TareaDependencias_DependeDeTareaId",
                table: "TareaDependencias",
                column: "DependeDeTareaId");

            migrationBuilder.CreateIndex(
                name: "IX_TareaDependencias_TareaId_DependeDeTareaId",
                table: "TareaDependencias",
                columns: new[] { "TareaId", "DependeDeTareaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosActividad");

            migrationBuilder.DropTable(
                name: "RegistrosTiempo");

            migrationBuilder.DropTable(
                name: "Subtareas");

            migrationBuilder.DropTable(
                name: "TareaAdjuntos");

            migrationBuilder.DropTable(
                name: "TareaDependencias");
        }
    }
}

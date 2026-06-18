using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowDesk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpleadoAvatarIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvatarIndex",
                table: "Empleados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarIndex",
                table: "Empleados");
        }
    }
}

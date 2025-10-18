using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNumeroGrupoAndEstadoToAsistente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Asistentes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NumeroGrupo",
                table: "Asistentes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Asistentes");

            migrationBuilder.DropColumn(
                name: "NumeroGrupo",
                table: "Asistentes");
        }
    }
}

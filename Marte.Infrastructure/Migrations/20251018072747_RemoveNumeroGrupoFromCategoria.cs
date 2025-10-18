using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marte.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNumeroGrupoFromCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroGrupo",
                table: "Categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroGrupo",
                table: "Categorias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

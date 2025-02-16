using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HqCatalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class DescricaoHQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescricaoCompleta",
                table: "HQs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescricaoCompleta",
                table: "HQs");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URL_Shortner.Migrations
{
    /// <inheritdoc />
    public partial class AddClickCountToUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "Urls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "Urls");
        }
    }
}

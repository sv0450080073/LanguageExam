using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLanguage.Migrations
{
    public partial class updatetbKanjiColumnVN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VNMean",
                table: "KanJis",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VNMean",
                table: "KanJis");
        }
    }
}

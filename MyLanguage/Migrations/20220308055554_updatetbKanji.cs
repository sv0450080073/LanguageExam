using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLanguage.Migrations
{
    public partial class updatetbKanji : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KanJiSingle",
                table: "KanJis",
                newName: "KanJiWord");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "KanJis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "KanJis");

            migrationBuilder.RenameColumn(
                name: "KanJiWord",
                table: "KanJis",
                newName: "KanJiSingle");
        }
    }
}

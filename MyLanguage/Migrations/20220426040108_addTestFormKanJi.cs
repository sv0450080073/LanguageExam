using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLanguage.Migrations
{
    public partial class addTestFormKanJi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamForm",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamForm", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExamFormDetail",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamForm_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanJiID = table.Column<int>(type: "int", nullable: false),
                    KanJiWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmHanViet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VietNamMean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmKun = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hiragana = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamFormDetail", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamForm");

            migrationBuilder.DropTable(
                name: "ExamFormDetail");
        }
    }
}

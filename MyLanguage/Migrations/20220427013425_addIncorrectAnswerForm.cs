using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLanguage.Migrations
{
    public partial class addIncorrectAnswerForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncorrectAnswerForm",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamForm_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncorrectAnswerForm", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IncorrectAnswerFormDetail",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncorrectAnswerForm_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamFormDetail_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanJiID = table.Column<int>(type: "int", nullable: false),
                    IncorrectKanJiWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncorrectAmHanViet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncorrectVietNamMean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncorrectAmOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncorrectAmKun = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncorrectHiragana = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncorrectAnswerFormDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserScores",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamForm_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: false),
                    ExamDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScores", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncorrectAnswerForm");

            migrationBuilder.DropTable(
                name: "IncorrectAnswerFormDetail");

            migrationBuilder.DropTable(
                name: "UserScores");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workman.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wm_projects",
                columns: table => new
                {
                    f_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    f_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wm_projects", x => x.f_id);
                });

            migrationBuilder.CreateTable(
                name: "wm_work_logs",
                columns: table => new
                {
                    f_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    f_project_id = table.Column<long>(type: "INTEGER", nullable: false),
                    f_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    f_content = table.Column<string>(type: "TEXT", nullable: false),
                    f_elapsed_time = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wm_work_logs", x => x.f_id);
                    table.ForeignKey(
                        name: "FK_wm_work_logs_wm_projects_f_project_id",
                        column: x => x.f_project_id,
                        principalTable: "wm_projects",
                        principalColumn: "f_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wm_work_logs_f_project_id",
                table: "wm_work_logs",
                column: "f_project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wm_work_logs");

            migrationBuilder.DropTable(
                name: "wm_projects");
        }
    }
}

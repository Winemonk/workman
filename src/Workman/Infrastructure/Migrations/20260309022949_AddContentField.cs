using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workman.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "f_content",
                table: "wm_work_tasks",
                newName: "f_name");

            migrationBuilder.AddColumn<string>(
                name: "f_content",
                table: "wm_work_logs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "f_content",
                table: "wm_work_logs");

            migrationBuilder.RenameColumn(
                name: "f_name",
                table: "wm_work_tasks",
                newName: "f_content");
        }
    }
}

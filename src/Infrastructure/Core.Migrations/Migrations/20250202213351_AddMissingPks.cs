using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingPks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_global_task_status",
                table: "global_task_status",
                columns: new[] { "task_key", "user_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_global_task",
                table: "global_task",
                column: "key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_global_task_status",
                table: "global_task_status");

            migrationBuilder.DropPrimaryKey(
                name: "PK_global_task",
                table: "global_task");
        }
    }
}

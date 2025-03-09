using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRolesDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short[]>(
                name: "roles",
                table: "user",
                type: "smallint[]",
                nullable: false,
                defaultValueSql: "'{0}'::smallint[]",
                oldClrType: typeof(short[]),
                oldType: "smallint[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short[]>(
                name: "roles",
                table: "user",
                type: "smallint[]",
                nullable: false,
                oldClrType: typeof(short[]),
                oldType: "smallint[]",
                oldDefaultValueSql: "'{0}'::smallint[]");
        }
    }
}

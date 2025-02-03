using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short[]>(
                name: "roles",
                table: "session",
                type: "smallint[]",
                nullable: false,
                defaultValueSql: "'{0}'::smallint[]");

            migrationBuilder.AlterColumn<string>(
                name: "referral_name",
                table: "referrer_transaction",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "roles",
                table: "session");

            migrationBuilder.AlterColumn<string>(
                name: "referral_name",
                table: "referrer_transaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "");
        }
    }
}

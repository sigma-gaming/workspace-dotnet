using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReferralNameDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "referral_name",
                table: "referrer_transaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "referral_name",
                table: "referrer_transaction",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

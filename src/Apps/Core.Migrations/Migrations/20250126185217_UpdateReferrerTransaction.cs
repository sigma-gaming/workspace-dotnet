using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReferrerTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "referral_image",
                table: "referrer_transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "referral_name",
                table: "referrer_transaction",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "referral_username",
                table: "referrer_transaction",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "referral_image",
                table: "referrer_transaction");

            migrationBuilder.DropColumn(
                name: "referral_name",
                table: "referrer_transaction");

            migrationBuilder.DropColumn(
                name: "referral_username",
                table: "referrer_transaction");
        }
    }
}

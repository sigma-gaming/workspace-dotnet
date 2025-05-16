using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderTransactionIdIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_provider_transaction_id",
                table: "withdrawal",
                column: "provider_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_provider_transaction_id",
                table: "deposit",
                column: "provider_transaction_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_withdrawal_provider_transaction_id",
                table: "withdrawal");

            migrationBuilder.DropIndex(
                name: "IX_deposit_provider_transaction_id",
                table: "deposit");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FixCurrencyPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "currency_amount",
                table: "withdrawal",
                type: "numeric(38,18)",
                precision: 38,
                scale: 18,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,18)",
                oldPrecision: 20,
                oldScale: 18);

            migrationBuilder.AlterColumn<decimal>(
                name: "currency_amount",
                table: "deposit",
                type: "numeric(38,18)",
                precision: 38,
                scale: 18,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,18)",
                oldPrecision: 20,
                oldScale: 18);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "currency_amount",
                table: "withdrawal",
                type: "numeric(20,18)",
                precision: 20,
                scale: 18,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)",
                oldPrecision: 38,
                oldScale: 18);

            migrationBuilder.AlterColumn<decimal>(
                name: "currency_amount",
                table: "deposit",
                type: "numeric(20,18)",
                precision: 20,
                scale: 18,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)",
                oldPrecision: 38,
                oldScale: 18);
        }
    }
}

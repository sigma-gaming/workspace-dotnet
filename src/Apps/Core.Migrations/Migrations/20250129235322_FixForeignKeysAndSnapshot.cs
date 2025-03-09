using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeysAndSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_balance_game_record_max_multiplier_game_id",
                table: "balance");

            migrationBuilder.DropForeignKey(
                name: "FK_balance_game_record_max_win_game_id",
                table: "balance");

            migrationBuilder.AddForeignKey(
                name: "FK_balance_game_record_max_multiplier_game_id",
                table: "balance",
                column: "max_multiplier_game_id",
                principalTable: "game_record",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_balance_game_record_max_win_game_id",
                table: "balance",
                column: "max_win_game_id",
                principalTable: "game_record",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_balance_game_record_max_multiplier_game_id",
                table: "balance");

            migrationBuilder.DropForeignKey(
                name: "FK_balance_game_record_max_win_game_id",
                table: "balance");

            migrationBuilder.AddForeignKey(
                name: "FK_balance_game_record_max_multiplier_game_id",
                table: "balance",
                column: "max_multiplier_game_id",
                principalTable: "game_record",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_balance_game_record_max_win_game_id",
                table: "balance",
                column: "max_win_game_id",
                principalTable: "game_record",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalIQ.Data.Migrations
{
    /// <inheritdoc />
    public partial class InternalOrderIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InternalOrderId",
                table: "Trades",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Trades_InternalOrderId",
                table: "Trades",
                column: "InternalOrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trades_InternalOrderId",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "InternalOrderId",
                table: "Trades");
        }
    }
}

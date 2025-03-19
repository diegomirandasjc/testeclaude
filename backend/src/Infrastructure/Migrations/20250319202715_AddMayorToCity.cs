using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMayorToCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MayorId",
                table: "Cities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_MayorId",
                table: "Cities",
                column: "MayorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Persons_MayorId",
                table: "Cities",
                column: "MayorId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Persons_MayorId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_MayorId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "MayorId",
                table: "Cities");
        }
    }
}

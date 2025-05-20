using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPlatformMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsAndRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Reviews",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BookshelfEntries",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedReadingDate",
                table: "BookshelfEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ownership",
                table: "BookshelfEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProgressPercent",
                table: "BookshelfEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedReadingDate",
                table: "BookshelfEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfEntries_UserId",
                table: "BookshelfEntries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookshelfEntries_AspNetUsers_UserId",
                table: "BookshelfEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookshelfEntries_AspNetUsers_UserId",
                table: "BookshelfEntries");

            migrationBuilder.DropIndex(
                name: "IX_BookshelfEntries_UserId",
                table: "BookshelfEntries");

            migrationBuilder.DropColumn(
                name: "FinishedReadingDate",
                table: "BookshelfEntries");

            migrationBuilder.DropColumn(
                name: "Ownership",
                table: "BookshelfEntries");

            migrationBuilder.DropColumn(
                name: "ProgressPercent",
                table: "BookshelfEntries");

            migrationBuilder.DropColumn(
                name: "StartedReadingDate",
                table: "BookshelfEntries");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BookshelfEntries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}

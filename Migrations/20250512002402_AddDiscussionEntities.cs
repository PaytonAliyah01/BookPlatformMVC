using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPlatformMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscussionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookClubMemberships_AspNetUsers_UserId",
                table: "BookClubMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_BookClubs_AspNetUsers_UserId",
                table: "BookClubs");

            migrationBuilder.DropIndex(
                name: "IX_BookClubs_UserId",
                table: "BookClubs");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BookClubs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BookClubMemberships",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DiscussionThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookClubId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookClubMembershipId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionThreads_BookClubMemberships_BookClubMembershipId",
                        column: x => x.BookClubMembershipId,
                        principalTable: "BookClubMemberships",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionThreads_BookClubs_BookClubId",
                        column: x => x.BookClubId,
                        principalTable: "BookClubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThreadId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscussionPosts_DiscussionThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "DiscussionThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionPosts_ThreadId",
                table: "DiscussionPosts",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionPosts_UserId",
                table: "DiscussionPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThreads_BookClubId",
                table: "DiscussionThreads",
                column: "BookClubId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionThreads_BookClubMembershipId",
                table: "DiscussionThreads",
                column: "BookClubMembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookClubMemberships_AspNetUsers_UserId",
                table: "BookClubMemberships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookClubMemberships_AspNetUsers_UserId",
                table: "BookClubMemberships");

            migrationBuilder.DropTable(
                name: "DiscussionPosts");

            migrationBuilder.DropTable(
                name: "DiscussionThreads");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BookClubs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BookClubMemberships",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_BookClubs_UserId",
                table: "BookClubs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookClubMemberships_AspNetUsers_UserId",
                table: "BookClubMemberships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookClubs_AspNetUsers_UserId",
                table: "BookClubs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

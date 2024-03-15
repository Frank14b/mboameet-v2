using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mboameet.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedcomments_feeds_FeedId",
                table: "feedcomments");

            migrationBuilder.DropForeignKey(
                name: "FK_feedfiles_feeds_FeedId",
                table: "feedfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_feeds_users_UserId",
                table: "feeds");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "feeds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "feeds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "feedfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "feedlikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Count = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FeedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedlikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_feedlikes_feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_feedlikes_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_feedcomments_UserId",
                table: "feedcomments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_feedlikes_FeedId",
                table: "feedlikes",
                column: "FeedId");

            migrationBuilder.CreateIndex(
                name: "IX_feedlikes_UserId",
                table: "feedlikes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedcomments_feeds_FeedId",
                table: "feedcomments",
                column: "FeedId",
                principalTable: "feeds",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_feedcomments_users_UserId",
                table: "feedcomments",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_feedfiles_feeds_FeedId",
                table: "feedfiles",
                column: "FeedId",
                principalTable: "feeds",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_feeds_users_UserId",
                table: "feeds",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedcomments_feeds_FeedId",
                table: "feedcomments");

            migrationBuilder.DropForeignKey(
                name: "FK_feedcomments_users_UserId",
                table: "feedcomments");

            migrationBuilder.DropForeignKey(
                name: "FK_feedfiles_feeds_FeedId",
                table: "feedfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_feeds_users_UserId",
                table: "feeds");

            migrationBuilder.DropTable(
                name: "feedlikes");

            migrationBuilder.DropIndex(
                name: "IX_feedcomments_UserId",
                table: "feedcomments");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "feeds");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "feeds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "feedfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_feedcomments_feeds_FeedId",
                table: "feedcomments",
                column: "FeedId",
                principalTable: "feeds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_feedfiles_feeds_FeedId",
                table: "feedfiles",
                column: "FeedId",
                principalTable: "feeds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_feeds_users_UserId",
                table: "feeds",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}

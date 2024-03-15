using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mboameet.Migrations
{
    /// <inheritdoc />
    public partial class CreateFeedsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_Id",
                table: "matches");

            migrationBuilder.CreateTable(
                name: "feeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_feeds_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "feedcomments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FeedId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedcomments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_feedcomments_feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "feeds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "feedfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FeedId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_feedfiles_feeds_FeedId",
                        column: x => x.FeedId,
                        principalTable: "feeds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_feedcomments_FeedId",
                table: "feedcomments",
                column: "FeedId");

            migrationBuilder.CreateIndex(
                name: "IX_feedfiles_FeedId",
                table: "feedfiles",
                column: "FeedId");

            migrationBuilder.CreateIndex(
                name: "IX_feeds_UserId",
                table: "feeds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_Id",
                table: "matches",
                column: "Id",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_Id",
                table: "matches");

            migrationBuilder.DropTable(
                name: "feedcomments");

            migrationBuilder.DropTable(
                name: "feedfiles");

            migrationBuilder.DropTable(
                name: "feeds");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_Id",
                table: "matches",
                column: "Id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}

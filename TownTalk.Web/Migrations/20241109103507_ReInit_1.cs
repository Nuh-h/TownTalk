using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TownTalk.Web.Migrations
{
    /// <inheritdoc />
    public partial class ReInit_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FollowedAt",
                table: "UserFollows",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Reactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowedAt",
                table: "UserFollows");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Reactions");
        }
    }
}

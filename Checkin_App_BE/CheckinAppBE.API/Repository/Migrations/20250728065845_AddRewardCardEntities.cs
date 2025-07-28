using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardCardEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Coin",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExperiencePoints",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Coin",
                table: "Treasures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExperiencePoints",
                table: "Treasures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TreasureType",
                table: "Treasures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Landmarks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RewardCards",
                columns: table => new
                {
                    RewardCardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rarity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DropRate = table.Column<double>(type: "float", nullable: false),
                    AnimationVideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnimationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardCards", x => x.RewardCardID);
                });

            migrationBuilder.CreateTable(
                name: "UserRewardCards",
                columns: table => new
                {
                    UserRewardCardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RewardCardID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRewardCards", x => x.UserRewardCardID);
                    table.ForeignKey(
                        name: "FK_UserRewardCards_RewardCards",
                        column: x => x.RewardCardID,
                        principalTable: "RewardCards",
                        principalColumn: "RewardCardID");
                    table.ForeignKey(
                        name: "FK_UserRewardCards_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRewardCards_RewardCardID",
                table: "UserRewardCards",
                column: "RewardCardID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRewardCards_UserID",
                table: "UserRewardCards",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRewardCards");

            migrationBuilder.DropTable(
                name: "RewardCards");

            migrationBuilder.DropColumn(
                name: "Coin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExperiencePoints",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Coin",
                table: "Treasures");

            migrationBuilder.DropColumn(
                name: "ExperiencePoints",
                table: "Treasures");

            migrationBuilder.DropColumn(
                name: "TreasureType",
                table: "Treasures");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Landmarks");
        }
    }
}

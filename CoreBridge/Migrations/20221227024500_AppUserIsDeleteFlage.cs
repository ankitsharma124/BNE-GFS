using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBridge.Migrations
{
    public partial class AppUserIsDeleteFlage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    UserId = table.Column<string>(type: "STRING", nullable: false),
                    TitleCode = table.Column<string>(type: "STRING", nullable: true),
                    Role = table.Column<int>(type: "INT64", nullable: false),
                    Email = table.Column<string>(type: "STRING", nullable: true),
                    Password = table.Column<string>(type: "STRING", nullable: false),
                    UpdateUser = table.Column<string>(type: "STRING", nullable: true),
                    IsDelete = table.Column<bool>(type: "BOOL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_UserId",
                table: "AppUsers",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");
        }
    }
}

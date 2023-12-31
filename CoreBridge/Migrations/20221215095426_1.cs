﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBridge.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    Name = table.Column<string>(type: "STRING", nullable: false),
                    EMail = table.Column<string>(type: "STRING", nullable: false),
                    Password = table.Column<string>(type: "STRING", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebugInfoList",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    TitleCode = table.Column<string>(type: "STRING", nullable: true),
                    UserId = table.Column<string>(type: "STRING", nullable: true),
                    RequestPath = table.Column<string>(type: "STRING", nullable: true),
                    RequestBody = table.Column<string>(type: "STRING", nullable: true),
                    ResponseBody = table.Column<string>(type: "STRING", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebugInfoList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TitleInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    TitleName = table.Column<string>(type: "STRING", nullable: false),
                    TitleCode = table.Column<string>(type: "STRING", nullable: false),
                    TrialTitleCode = table.Column<string>(type: "STRING", nullable: false),
                    Ptype = table.Column<int>(type: "INT64", nullable: false),
                    SwitchAppId = table.Column<string>(type: "STRING", nullable: true),
                    XboxTitleId = table.Column<string>(type: "STRING", nullable: true),
                    PsClientId = table.Column<string>(type: "STRING", nullable: true),
                    PsClientSecoret = table.Column<string>(type: "STRING", nullable: true),
                    SteamAppId = table.Column<string>(type: "STRING", nullable: true),
                    SteamPublisherKey = table.Column<string>(type: "STRING", nullable: true),
                    DevUrl = table.Column<string>(type: "STRING", nullable: true),
                    TestUrl = table.Column<string>(type: "STRING", nullable: true),
                    QaUrl = table.Column<string>(type: "STRING", nullable: true),
                    ProdUrl = table.Column<string>(type: "STRING", nullable: true),
                    HashKey = table.Column<string>(type: "STRING", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlatforms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    UserId = table.Column<string>(type: "STRING", nullable: false),
                    PlatformType = table.Column<int>(type: "INT64", nullable: false),
                    PlatformUserId = table.Column<string>(type: "STRING", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    CountryCode = table.Column<string>(type: "STRING", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    Platform = table.Column<int>(type: "INT64", nullable: false),
                    TitleCode = table.Column<string>(type: "STRING", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TitleInfo",
                columns: new[] { "Id", "CreatedAt", "DevUrl", "HashKey", "ProdUrl", "PsClientId", "PsClientSecoret", "Ptype", "QaUrl", "SteamAppId", "SteamPublisherKey", "SwitchAppId", "TestUrl", "TitleCode", "TitleName", "TrialTitleCode", "UpdatedAt", "XboxTitleId" },
                values: new object[] { "TestTitleId", new DateTime(2022, 12, 15, 9, 54, 25, 885, DateTimeKind.Utc).AddTicks(5387), null, "TEST111111111111", null, null, null, 1, null, null, null, null, null, "TestTitleCode", "testTitleName", "TestTrialTitleCode", new DateTime(2022, 12, 15, 9, 54, 25, 885, DateTimeKind.Utc).AddTicks(5388), null });

            migrationBuilder.InsertData(
                table: "UserPlatforms",
                columns: new[] { "Id", "CountryCode", "CreatedAt", "PlatformType", "PlatformUserId", "UpdatedAt", "UserId" },
                values: new object[] { "UserPlatformId", "ja", new DateTime(2022, 12, 15, 9, 54, 25, 885, DateTimeKind.Utc).AddTicks(5503), 1, "PlatformUserId", new DateTime(2022, 12, 15, 9, 54, 25, 885, DateTimeKind.Utc).AddTicks(5504), "TestUserId" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Platform", "TitleCode", "UpdatedAt" },
                values: new object[] { "TestUserId", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "TestTitleCode", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_TitleInfo_TitleCode",
                table: "TitleInfo",
                column: "TitleCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "DebugInfoList");

            migrationBuilder.DropTable(
                name: "TitleInfo");

            migrationBuilder.DropTable(
                name: "UserPlatforms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

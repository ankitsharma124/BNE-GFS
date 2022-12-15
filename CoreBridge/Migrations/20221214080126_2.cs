using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBridge.Migrations
{
    public partial class _2 : Migration
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
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    UserId = table.Column<string>(type: "STRING", nullable: false),
                    TitleCode = table.Column<string>(type: "STRING", nullable: true),
                    Password = table.Column<string>(type: "STRING", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    Name = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "STRING", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "STRING", nullable: false),
                    UserName = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "STRING(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "BOOL", nullable: false),
                    PasswordHash = table.Column<string>(type: "STRING", nullable: true),
                    SecurityStamp = table.Column<string>(type: "STRING", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "STRING", nullable: true),
                    PhoneNumber = table.Column<string>(type: "STRING", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "BOOL", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "BOOL", nullable: false),
                    LockoutEnd = table.Column<string>(type: "STRING(48)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "BOOL", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INT64", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT64", nullable: false),
                    RoleId = table.Column<string>(type: "STRING", nullable: false),
                    ClaimType = table.Column<string>(type: "STRING", nullable: true),
                    ClaimValue = table.Column<string>(type: "STRING", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT64", nullable: false),
                    UserId = table.Column<string>(type: "STRING", nullable: false),
                    ClaimType = table.Column<string>(type: "STRING", nullable: true),
                    ClaimValue = table.Column<string>(type: "STRING", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "STRING", nullable: false),
                    ProviderKey = table.Column<string>(type: "STRING", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "STRING", nullable: true),
                    UserId = table.Column<string>(type: "STRING", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "STRING", nullable: false),
                    RoleId = table.Column<string>(type: "STRING", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "STRING", nullable: false),
                    LoginProvider = table.Column<string>(type: "STRING", nullable: false),
                    Name = table.Column<string>(type: "STRING", nullable: false),
                    Value = table.Column<string>(type: "STRING", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TitleInfo",
                columns: new[] { "Id", "CreatedAt", "DevUrl", "HashKey", "ProdUrl", "PsClientId", "PsClientSecoret", "Ptype", "QaUrl", "SteamAppId", "SteamPublisherKey", "SwitchAppId", "TestUrl", "TitleCode", "TitleName", "TrialTitleCode", "UpdatedAt", "XboxTitleId" },
                values: new object[] { "TestTitleId", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "TEST111111111111", null, null, null, 0, null, null, null, null, null, "TestTitleCode", "testTitleName", "TestTrialTitleCode", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_UserId",
                table: "AppUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

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
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DebugInfoList");

            migrationBuilder.DropTable(
                name: "TitleInfo");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}

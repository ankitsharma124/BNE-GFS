using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBridge.Migrations
{
    public partial class AddTitleInfoPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TitleInfo",
                columns: table => new
                {
                    TitleCode = table.Column<string>(type: "STRING", nullable: false),
                    TitleName = table.Column<string>(type: "STRING", nullable: false),
                    TrialTitleCode = table.Column<string>(type: "STRING", nullable: false),
                    Ptype = table.Column<int>(type: "INT64", nullable: false),
                    SwitchAppId = table.Column<string>(type: "STRING", nullable: true),
                    XboxTitleId = table.Column<string>(type: "STRING", nullable: true),
                    PsClientId = table.Column<string>(type: "STRING", nullable: true),
                    PsClientSecoret = table.Column<string>(type: "STRING", nullable: true),
                    SteamAppId = table.Column<string>(type: "STRING", nullable: true),
                    SteamPublisherKey = table.Column<string>(type: "STRING", nullable: true),
                    DevUrl = table.Column<string>(type: "STRING", nullable: true),
                    QaUrl = table.Column<string>(type: "STRING", nullable: true),
                    ProdUrl = table.Column<string>(type: "STRING", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    Id = table.Column<string>(type: "STRING", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleInfo", x => x.TitleCode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TitleInfo");
        }
    }
}

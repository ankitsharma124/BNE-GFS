﻿// <auto-generated />
using System;
using CoreBridge.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CoreBridge.Migrations
{
    [DbContext(typeof(CoreBridgeContext))]
    [Migration("20221210044813_1")]
    partial class _1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("CoreBridge.Models.Entity.AdminUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("STRING");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("EMail")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.HasKey("Id");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("CoreBridge.Models.Entity.DebugInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("STRING");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("RequestBody")
                        .HasColumnType("STRING");

                    b.Property<string>("RequestPath")
                        .HasColumnType("STRING");

                    b.Property<string>("ResponseBody")
                        .HasColumnType("STRING");

                    b.Property<string>("TitleCode")
                        .HasColumnType("STRING");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("UserId")
                        .HasColumnType("STRING");

                    b.HasKey("Id");

                    b.ToTable("DebugInfoList");
                });

            modelBuilder.Entity("CoreBridge.Models.Entity.GFSUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("STRING");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.Property<int>("Platform")
                        .HasColumnType("INT64");

                    b.Property<string>("TitleCode")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = "TestUserId",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Platform = 1,
                            TitleCode = "TestTitleCode",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("CoreBridge.Models.Entity.TitleInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("STRING");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("DevUrl")
                        .HasColumnType("STRING");

                    b.Property<string>("HashKey")
                        .HasColumnType("STRING");

                    b.Property<string>("ProdUrl")
                        .HasColumnType("STRING");

                    b.Property<string>("PsClientId")
                        .HasColumnType("STRING");

                    b.Property<string>("PsClientSecoret")
                        .HasColumnType("STRING");

                    b.Property<int>("Ptype")
                        .HasColumnType("INT64");

                    b.Property<string>("QaUrl")
                        .HasColumnType("STRING");

                    b.Property<string>("SteamAppId")
                        .HasColumnType("STRING");

                    b.Property<string>("SteamPublisherKey")
                        .HasColumnType("STRING");

                    b.Property<string>("SwitchAppId")
                        .HasColumnType("STRING");

                    b.Property<string>("TestUrl")
                        .HasColumnType("STRING");

                    b.Property<string>("TitleCode")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<string>("TitleName")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<string>("TrialTitleCode")
                        .IsRequired()
                        .HasColumnType("STRING");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TIMESTAMP");

                    b.Property<string>("XboxTitleId")
                        .HasColumnType("STRING");

                    b.HasKey("Id");

                    b.HasIndex("TitleCode");

                    b.ToTable("TitleInfo");

                    b.HasData(
                        new
                        {
                            Id = "TestTitleId",
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Ptype = 0,
                            TitleCode = "TestTitleCode",
                            TitleName = "testTitleName",
                            TrialTitleCode = "TestTrialTitleCode",
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });
#pragma warning restore 612, 618
        }
    }
}

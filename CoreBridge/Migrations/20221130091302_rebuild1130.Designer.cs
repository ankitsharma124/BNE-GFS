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
    [Migration("20221130091302_rebuild1130")]
    partial class rebuild1130
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
                });
#pragma warning restore 612, 618
        }
    }
}

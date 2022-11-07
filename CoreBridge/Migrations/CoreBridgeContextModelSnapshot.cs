﻿// <auto-generated />
using System;
using CoreBridge.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CoreBridge.Migrations
{
    [DbContext(typeof(CoreBridgeContext))]
    partial class CoreBridgeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
#pragma warning restore 612, 618
        }
    }
}

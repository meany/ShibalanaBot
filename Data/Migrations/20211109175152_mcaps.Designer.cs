﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dm.Shibalana.Data;

namespace dm.Shibalana.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211109175152_mcaps")]
    partial class mcaps
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("dm.Shibalana.Data.Models.Price", b =>
                {
                    b.Property<int>("PriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CircMarketCapUSD")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("FullMarketCapUSD")
                        .HasColumnType("decimal(16,8)");

                    b.Property<Guid>("Group")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MarketCapUSDChange")
                        .HasColumnType("int");

                    b.Property<decimal>("MarketCapUSDChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.Property<decimal>("PriceSHIBAForOneUSDC")
                        .HasColumnType("decimal(16,8)");

                    b.Property<decimal>("PriceUSD")
                        .HasColumnType("decimal(11,6)");

                    b.Property<decimal>("PriceUSDCForOneSHIBA")
                        .HasColumnType("decimal(16,8)");

                    b.Property<int>("PriceUSDChange")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceUSDChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.Property<int>("Source")
                        .HasColumnType("int");

                    b.Property<int>("VolumeUSD")
                        .HasColumnType("int");

                    b.Property<int>("VolumeUSDChange")
                        .HasColumnType("int");

                    b.Property<decimal>("VolumeUSDChangePct")
                        .HasColumnType("decimal(12,8)");

                    b.HasKey("PriceId");

                    b.HasIndex("Date");

                    b.HasIndex("Group");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("dm.Shibalana.Data.Models.Request", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Command")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DiscordUserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("DiscordUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRateLimited")
                        .HasColumnType("bit");

                    b.Property<int>("Response")
                        .HasColumnType("int");

                    b.HasKey("RequestId");

                    b.HasIndex("Date");

                    b.HasIndex("Response", "Command");

                    b.ToTable("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}

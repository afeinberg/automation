﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using automation.Models;

namespace automation.Migrations
{
    [DbContext(typeof(TelemetryDbContext))]
    [Migration("20190704035032_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("automation.Models.Sensor", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<byte[]>("CustomData");

                    b.Property<string>("Description");

                    b.Property<string>("FriendlyName");

                    b.HasKey("Id");

                    b.ToTable("Sensor");
                });

            modelBuilder.Entity("automation.Models.TelemetryData", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<byte[]>("CustomData");

                    b.Property<double?>("Humidity");

                    b.Property<decimal?>("SensorId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<double?>("Temperature");

                    b.Property<DateTime?>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("TelemetryData");
                });

            modelBuilder.Entity("automation.Models.TelemetryData", b =>
                {
                    b.HasOne("automation.Models.Sensor", "Sensor")
                        .WithMany("TelemetryData")
                        .HasForeignKey("SensorId");
                });
#pragma warning restore 612, 618
        }
    }
}
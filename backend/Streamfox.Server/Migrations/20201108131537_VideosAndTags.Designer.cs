﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Streamfox.Server.Persistence.Database;

namespace Streamfox.Server.Migrations
{
    [DbContext(typeof(VideoDatabaseContext))]
    [Migration("20201108131537_VideosAndTags")]
    partial class VideosAndTags
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Streamfox.Server.Persistence.Database.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Streamfox.Server.Persistence.Database.Video", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Codec")
                        .HasColumnType("integer");

                    b.Property<int>("Format")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("Streamfox.Server.Persistence.Database.VideoTag", b =>
                {
                    b.Property<long>("VideoId")
                        .HasColumnType("bigint");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint");

                    b.HasKey("VideoId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("VideoTag");
                });

            modelBuilder.Entity("Streamfox.Server.Persistence.Database.VideoTag", b =>
                {
                    b.HasOne("Streamfox.Server.Persistence.Database.Tag", "Tag")
                        .WithMany("VideoTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Streamfox.Server.Persistence.Database.Video", "Video")
                        .WithMany("VideoTags")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
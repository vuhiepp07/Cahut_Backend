﻿// <auto-generated />
using System;
using Cahut_Backend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CahutBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cahut_Backend.Models.EmailSender", b =>
                {
                    b.Property<string>("usr")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EmailSended")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("pwd")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("usr");

                    b.ToTable("EmailSender", (string)null);
                });

            modelBuilder.Entity("Cahut_Backend.Models.Group", b =>
                {
                    b.Property<Guid>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("JoinGrLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GroupId");

                    b.ToTable("Group", (string)null);
                });

            modelBuilder.Entity("Cahut_Backend.Models.GroupDetail", b =>
                {
                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.HasKey("GroupId", "MemberId");

                    b.HasIndex("GroupId");

                    b.HasIndex("MemberId");

                    b.HasIndex("RoleId")
                        .IsUnique();

                    b.ToTable("GroupDetail", (string)null);
                });

            modelBuilder.Entity("Cahut_Backend.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("Cahut_Backend.Models.Token", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Token", (string)null);
                });

            modelBuilder.Entity("Cahut_Backend.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccountStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Cahut_Backend.Models.GroupDetail", b =>
                {
                    b.HasOne("Cahut_Backend.Models.Group", "Group")
                        .WithOne("GroupDetail")
                        .HasForeignKey("Cahut_Backend.Models.GroupDetail", "GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cahut_Backend.Models.User", "User")
                        .WithOne("GroupDetail")
                        .HasForeignKey("Cahut_Backend.Models.GroupDetail", "MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cahut_Backend.Models.Role", "Role")
                        .WithOne("GroupDetail")
                        .HasForeignKey("Cahut_Backend.Models.GroupDetail", "RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Cahut_Backend.Models.Token", b =>
                {
                    b.HasOne("Cahut_Backend.Models.User", "User")
                        .WithOne("Token")
                        .HasForeignKey("Cahut_Backend.Models.Token", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Cahut_Backend.Models.Group", b =>
                {
                    b.Navigation("GroupDetail")
                        .IsRequired();
                });

            modelBuilder.Entity("Cahut_Backend.Models.Role", b =>
                {
                    b.Navigation("GroupDetail")
                        .IsRequired();
                });

            modelBuilder.Entity("Cahut_Backend.Models.User", b =>
                {
                    b.Navigation("GroupDetail")
                        .IsRequired();

                    b.Navigation("Token")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

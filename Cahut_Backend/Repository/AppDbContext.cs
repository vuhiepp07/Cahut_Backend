﻿using Cahut_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cahut_Backend.Repository
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupDetail> GroupDetail { get; set; }
        public DbSet<EmailSender> EmailSender { get; set; }
        public DbSet<Presentation> Presentation { get; set; }
        public DbSet<Slide> Slide { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            builder.UseSqlServer(configuration.GetConnectionString("Cahut"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<EmailSender>(entity => {
                entity.ToTable("EmailSender");
                entity.HasKey(p => p.usr);
                entity.Property(p => p.EmailSended).HasDefaultValue(0);
            });

            builder.Entity<User>(entity => {
                entity.ToTable("User");
                entity.HasKey(p => p.UserId);
                entity.HasIndex(p => p.Email).IsUnique(true);
                entity.Property(p => p.Avatar).IsRequired(false);
                entity.Property(p => p.Phone).IsRequired(false);
                entity.Property(p => p.RefreshToken).IsRequired(false);
                entity.Property(p => p.RefreshTokenExpiredTime).HasDefaultValue(DateTime.UtcNow);
                entity.Property(p => p.AccountStatus).HasDefaultValue(0);
            });

            builder.Entity<Role>(entity => {
                entity.ToTable("Role");
                entity.HasKey(p => p.RoleId);
                entity.Property(p => p.RoleId).ValueGeneratedOnAdd();
            });

            builder.Entity<Group>(entity => {
                entity.ToTable("Group");
                entity.HasKey(p => p.GroupId);
                entity.Property(p => p.JoinGrString).IsRequired(false);
                entity.Property(p => p.GroupName).IsRequired(true);
                entity.HasIndex(p => p.GroupName).IsUnique(true);
                entity.Property(p => p.NumOfMems).HasDefaultValue(1);
            });

            builder.Entity<GroupDetail>(entity => {
                entity.ToTable("GroupDetail");
                entity.HasKey(p => new { p.GroupId, p.MemberId });
                entity.HasIndex(p => p.GroupId).IsUnique(false);
                entity.HasIndex(p => p.MemberId).IsUnique(false);
                entity.HasIndex(p => p.RoleId).IsUnique(false);
                entity.Property(p => p.RoleId).HasDefaultValue(1);
                entity.HasOne(p => p.Group)
                        .WithOne(p => p.GroupDetail)
                        .HasForeignKey<GroupDetail>(p => p.GroupId);
                entity.HasOne(p => p.User)
                        .WithOne(p => p.GroupDetail)
                        .HasForeignKey<GroupDetail>(p => p.MemberId);
                entity.HasOne(p => p.Role)
                        .WithOne(p => p.GroupDetail)
                        .HasForeignKey<GroupDetail>(p => p.RoleId);
            });

            builder.Entity<Presentation>(entity =>
            {
                entity.ToTable("Presentation");
                entity.HasKey(p => p.PresentationId);
                entity.HasOne(p => p.User)
                            .WithMany(p => p.Presentations)
                            .HasForeignKey(p => p.TeacherId);
            });

            builder.Entity<Slide>(entity =>
            {
                entity.ToTable("Slide");
                entity.HasKey(p => p.SlideId);
                entity.HasOne(p => p.Presentation)
                            .WithMany(p => p.Slides)
                            .HasForeignKey(p => p.PresentationId);
                entity.Property(p => p.SlideOrder).HasDefaultValue(0);
            });

            builder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");
                entity.HasKey(p => p.QuestionId);
                entity.HasOne(p => p.Slide)
                        .WithOne(p => p.Question)
                        .HasForeignKey<Question>(p => p.SlideId);
                entity.Property(p => p.QuestionType).IsRequired(false);
                entity.Property(p => p.RightAnswer).IsRequired(false);
            });

            builder.Entity<Answer>(entity =>
            {
                entity.ToTable("Answer");
                entity.HasKey(p => p.AnswerId);
                entity.HasOne(p => p.Question)
                            .WithMany(p => p.Answers)
                            .HasForeignKey(p => p.QuestionId);
                entity.Property(p => p.NumSelected).HasDefaultValue(0);
            });
        }
    }
}

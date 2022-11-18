using Cahut_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cahut_Backend.Repository
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupDetail> GroupDetail { get; set; }
        public DbSet<Token> Token { get; set; }
        public DbSet<EmailSender> EmailSender { get; set; }
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
            });

            builder.Entity<User>(entity => {
                entity.ToTable("User");
                entity.HasKey(p => p.UserId);
                entity.HasIndex(p => p.UserName).IsUnique(true);
                entity.Property(p => p.Avatar).IsRequired(false);
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
                entity.Property(p => p.JoinGrLink).IsRequired(false);
            });

            builder.Entity<GroupDetail>(entity => {
                entity.ToTable("GroupDetail");
                entity.HasKey(p => new { p.GroupId, p.MemberId });
                entity.HasIndex(p => p.GroupId).IsUnique(false);
                entity.HasIndex(p => p.MemberId).IsUnique(false);
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

            builder.Entity<Token>(entity => {
                entity.ToTable("Token");
                entity.HasKey(p => p.UserId);
                entity.Property(p => p.AccessToken).IsRequired(false);
                entity.Property(p => p.RefreshToken).IsRequired(false);
                entity.HasOne(p => p.User)
                        .WithOne(p => p.Token)
                        .HasForeignKey<Token>(p => p.UserId);
            });
        }
    }
}

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
        public DbSet<EmailSender> EmailSender { get; set; }
        public DbSet<Presentation> Presentation { get; set; }
        public DbSet<PresentationDetail> PresentationDetail { get; set; }
        public DbSet<PresentationQuestion> PresentationQuestion { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestion { get; set; }
        public DbSet<MultipleChoiceOption> MultipleChoiceOption { get; set; }
        public DbSet<MultipleChoiceSlide> MultipleChoiceSlide { get; set; }
        public DbSet<HeadingSlide> HeadingSlide { get; set; }
        public DbSet<ParagraphSlide> ParagraphSlide { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<Slide> Slide { get; set; }
        public DbSet<Question> Question { get; set; }

        public DbSet<UserUpvoteQuestion> UserUpvoteQuestion { get; set; }
        public DbSet<UserSubmitChoice> UserSubmitChoice { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            builder.UseSqlServer(configuration.GetConnectionString("Cahut"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<Slide>()
            //            .ToTable("Slides")
            //            .HasDiscriminator<int>("SlideType")
            //            .HasValue<MultipleChoiceSlide>(1)
            //            .HasValue<ParagraphSlide>(2)
            //            .HasValue<HeadingSlide>(3);

            builder.Entity<UserUpvoteQuestion>(entity =>
            {
                entity.ToTable("UserUpvoteQuestion");
                entity.HasKey(p => new { p.UserId, p.PresentationId, p.QuestionId });
                entity.HasIndex(p => p.UserId).IsUnique(false);
                entity.HasIndex(p => p.PresentationId).IsUnique(false);
                entity.HasIndex(p => p.QuestionId).IsUnique(false);
                entity.HasOne(p => p.User)
                        .WithMany(p => p.UserUpvoteQuestions)
                        .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.Group)
                        .WithMany(p => p.UserUpvoteQuestions)
                        .HasForeignKey(p => p.GroupId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.Presentation)
                        .WithMany(p => p.UserUpvoteQuestions)
                        .HasForeignKey(p => p.PresentationId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.PresentationQuestion)
                        .WithMany(p => p.UserUpvoteQuestions)
                        .HasForeignKey(p => p.QuestionId).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<UserSubmitChoice>(entity =>
            {
                entity.ToTable("UserSubmitChoice");
                entity.HasKey(p => new {p.UserId, p.OptionId, p.QuestionId});
                entity.HasIndex(p => p.UserId).IsUnique(false);
                entity.HasIndex(p => p.OptionId).IsUnique(false);
                entity.HasIndex(p => p.QuestionId).IsUnique(false);
                entity.HasOne(p => p.User)
                        .WithMany(p => p.UserSubmitChoices)
                        .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.Group)
                        .WithMany(p => p.UserSubmitChoices)
                        .HasForeignKey(p => p.GroupId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.MultipleChoiceQuestion)
                        .WithMany(p => p.UserSubmitChoices)
                        .HasForeignKey(p => p.QuestionId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.MultipleChoiceOption)
                        .WithMany(p => p.UserSubmitChoices)
                        .HasForeignKey(p => p.OptionId).OnDelete(DeleteBehavior.NoAction);
            });

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
                entity.Property(p => p.ResetPasswordString).IsRequired(false);
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
                entity.HasOne(p => p.User)
                        .WithMany(p => p.Group)
                        .HasForeignKey(p => p.OwnerId);
                entity.Property(p => p.PresentationId).IsRequired(false);
            });

            builder.Entity<GroupDetail>(entity => {
                entity.ToTable("GroupDetail");
                entity.HasKey(p => new { p.GroupId, p.MemberId });
                entity.HasIndex(p => p.GroupId).IsUnique(false);
                entity.HasIndex(p => p.MemberId).IsUnique(false);
                entity.HasIndex(p => p.RoleId).IsUnique(false);
                entity.Property(p => p.RoleId).HasDefaultValue(1);
                entity.HasOne(p => p.Group)
                        .WithMany(p => p.GroupDetails)
                        .HasForeignKey(p => p.GroupId);
                //entity.HasOne(p => p.User)
                //        .WithMany(p => p.GroupDetails)
                //        .HasForeignKey(p => p.MemberId);
                entity.HasOne(p => p.Role)
                        .WithMany(p => p.GroupDetails)
                        .HasForeignKey(p => p.RoleId);
            });

            builder.Entity<Presentation>(entity =>
            {
                entity.ToTable("Presentation");
                entity.HasKey(p => p.PresentationId);
                entity.HasOne(p => p.User)
                            .WithMany(p => p.Presentations)
                            .HasForeignKey(p => p.OwnerId);
                entity.Property(p => p.IsBeingPresented).HasDefaultValue(false);
                entity.Property(p => p.PresentationType).IsRequired(false);
            });

            builder.Entity<Slide>().UseTpcMappingStrategy();
            builder.Entity<Slide>(entity =>
            {
                entity.HasKey(p => p.SlideId);
            });

            builder.Entity<Question>().UseTpcMappingStrategy();
            builder.Entity<Question>(entity =>
            {
                entity.HasKey(p => p.QuestionId);
            });

            builder.Entity<MultipleChoiceSlide>(entity =>
            {
                entity.ToTable("MultipleChoiceSlide");
                entity.HasOne(p => p.Presentation)
                            .WithMany(p => p.MultipleChoiceSlides)
                            .HasForeignKey(p => p.PresentationId);
                entity.Property(p => p.SlideOrder).HasDefaultValue(0);
            });

            builder.Entity<HeadingSlide>(entity =>
            {
                entity.ToTable("HeadingSlide");
                entity.HasOne(p => p.Presentation)
                            .WithMany(p => p.HeadingSlides)
                            .HasForeignKey(p => p.PresentationId);
                entity.Property(p => p.SlideOrder).HasDefaultValue(0);
            });

            builder.Entity<ParagraphSlide>(entity =>
            {
                entity.ToTable("ParagraphSlide");
                entity.HasOne(p => p.Presentation)
                            .WithMany(p => p.ParagraphSlides)
                            .HasForeignKey(p => p.PresentationId);
                entity.Property(p => p.SlideOrder).HasDefaultValue(0);
            });

            builder.Entity<MultipleChoiceQuestion>().UseTpcMappingStrategy();
            builder.Entity<MultipleChoiceQuestion>(entity =>
            {
                entity.ToTable("MultipleChoiceQuestion");
                entity.HasOne(p => p.MultipleChoiceSlide)
                        .WithMany(p => p.MultipleChoiceQuestions)
                        .HasForeignKey(p => p.SlideId);
                entity.Property(p => p.QuestionType).IsRequired(false);
                entity.Property(p => p.RightAnswer).IsRequired(false);
            });

            builder.Entity<PresentationQuestion>(entity =>
            {
                entity.ToTable("PresentationQuestion");
                entity.HasOne(p => p.Presentation)
                        .WithMany(p => p.PresentationQuestions)
                        .HasForeignKey(p => p.PresentationId);
                entity.Property(p => p.QuestionType).IsRequired(false);
            });

            builder.Entity<PresentationDetail>(entity =>
            {
                entity.ToTable("PresentationDetail");
                entity.HasKey(p => new { p.PresentationId, p.ColaboratorId });
                entity.HasIndex(p => p.PresentationId).IsUnique(false);
                //entity.HasIndex(p => p.ColaboratorId).IsUnique(false);
                //entity.HasOne(p => p.User)
                //        .WithMany(p => p.PresentationDetails)
                //        .HasForeignKey(p => p.ColaboratorId);
                entity.HasOne(p => p.Presentation)
                        .WithMany(p => p.PresentationDetails)
                        .HasForeignKey(p => p.PresentationId);
            });

            builder.Entity<MultipleChoiceOption>(entity =>
            {
                entity.ToTable("MultipleChoiceOption");
                entity.HasKey(p => p.OptionId);
                entity.HasOne(p => p.MultipleChoiceQuestion)
                            .WithMany(p => p.MultipleChoiceOptions)
                            .HasForeignKey(p => p.QuestionId);
                entity.Property(p => p.NumSelected).HasDefaultValue(0);
            });

            builder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chat");
                entity.HasKey(p => p.ChatId);
                entity.HasOne(p => p.Presentation)
                            .WithOne(p => p.Chat)
                            .HasForeignKey<Chat>(p => p.PresentationId);
                entity.Property(p => p.NumOfMessage).HasDefaultValue(0);
            });

            builder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessage");
                entity.HasKey(p => new { p.ChatId, p.TimeSend });
                entity.HasIndex(p => p.ChatId).IsUnique(false);
                entity.HasIndex(p => p.TimeSend).IsUnique(false);
                entity.HasOne(p => p.Chat)
                            .WithMany(p => p.ChatMessages)
                            .HasForeignKey(p => p.ChatId);
            });
        }
    }
}

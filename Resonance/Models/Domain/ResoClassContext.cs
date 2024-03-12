using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ResoClassAPI.Models.Domain;

public partial class ResoClassContext : DbContext
{
    public ResoClassContext()
    {
    }

    public ResoClassContext(DbContextOptions<ResoClassContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssessmentConfiguration> AssessmentConfigurations { get; set; }

    public virtual DbSet<AssessmentSession> AssessmentSessions { get; set; }

    public virtual DbSet<AssessmentSessionQuestion> AssessmentSessionQuestions { get; set; }

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Choice> Choices { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<DifficultyLevel> DifficultyLevels { get; set; }

    public virtual DbSet<Logger> Loggers { get; set; }

    public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionBank> QuestionBanks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<SubTopic> SubTopics { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=SqlConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssessmentConfiguration>(entity =>
        {
            entity.ToTable("AssessmentConfiguration");

            entity.Property(e => e.CreatedBy).HasMaxLength(250);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(250);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.AssessmentConfigurations)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentConfiguration_Course");
        });

        modelBuilder.Entity<AssessmentSession>(entity =>
        {
            entity.ToTable("AssessmentSession");

            entity.Property(e => e.AssessmentType).HasMaxLength(50);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Student).WithMany(p => p.AssessmentSessions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentSession_AssessmentSession");
        });

        modelBuilder.Entity<AssessmentSessionQuestion>(entity =>
        {
            entity.ToTable("AssessmentSession_Questions");

            entity.Property(e => e.SelectedAnswer).HasMaxLength(10);

            entity.HasOne(d => d.AssessmentSession).WithMany(p => p.AssessmentSessionQuestions)
                .HasForeignKey(d => d.AssessmentSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentSession_Questions_AssessmentSession1");

            entity.HasOne(d => d.Question).WithMany(p => p.AssessmentSessionQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentSession_Questions_QuestionBank");
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.ToTable("Audit");

            entity.Property(e => e.ColumnName).HasMaxLength(128);
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.TableName).HasMaxLength(128);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.ToTable("Chapter");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Choice>(entity =>
        {
            entity.ToTable("Choice");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.CreatedBy).HasMaxLength(250);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(250);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<DifficultyLevel>(entity =>
        {
            entity.ToTable("DifficultyLevel");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Logger>(entity =>
        {
            entity.ToTable("Logger");

            entity.Property(e => e.CreateOn).HasColumnType("datetime");
            entity.Property(e => e.LogType).HasMaxLength(50);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(250);
        });

        modelBuilder.Entity<MultipleChoiceQuestion>(entity =>
        {
            entity.ToTable("MultipleChoiceQuestion");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Chapter).WithMany(p => p.MultipleChoiceQuestions)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_MultipleChoiceQuestion_Chapter");

            entity.HasOne(d => d.CorrectChoice).WithMany(p => p.MultipleChoiceQuestionCorrectChoices)
                .HasForeignKey(d => d.CorrectChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice4");

            entity.HasOne(d => d.FirstChoice).WithMany(p => p.MultipleChoiceQuestionFirstChoices)
                .HasForeignKey(d => d.FirstChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice");

            entity.HasOne(d => d.FourthChoice).WithMany(p => p.MultipleChoiceQuestionFourthChoices)
                .HasForeignKey(d => d.FourthChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice3");

            entity.HasOne(d => d.Question).WithMany(p => p.MultipleChoiceQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Question");

            entity.HasOne(d => d.SecondChoice).WithMany(p => p.MultipleChoiceQuestionSecondChoices)
                .HasForeignKey(d => d.SecondChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice1");

            entity.HasOne(d => d.SubTopic).WithMany(p => p.MultipleChoiceQuestions)
                .HasForeignKey(d => d.SubTopicId)
                .HasConstraintName("FK_MultipleChoiceQuestion_SubTopic");

            entity.HasOne(d => d.ThirdChoice).WithMany(p => p.MultipleChoiceQuestionThirdChoices)
                .HasForeignKey(d => d.ThirdChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice2");

            entity.HasOne(d => d.Topic).WithMany(p => p.MultipleChoiceQuestions)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK_MultipleChoiceQuestion_Topic");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Questions)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_Question_Chapter");

            entity.HasOne(d => d.CorrectChoice).WithMany(p => p.QuestionCorrectChoices)
                .HasForeignKey(d => d.CorrectChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Choice4");

            entity.HasOne(d => d.FirstChoice).WithMany(p => p.QuestionFirstChoices)
                .HasForeignKey(d => d.FirstChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Choice");

            entity.HasOne(d => d.FourthChoice).WithMany(p => p.QuestionFourthChoices)
                .HasForeignKey(d => d.FourthChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Choice3");

            entity.HasOne(d => d.SecondChoice).WithMany(p => p.QuestionSecondChoices)
                .HasForeignKey(d => d.SecondChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Choice1");

            entity.HasOne(d => d.ThirdChoice).WithMany(p => p.QuestionThirdChoices)
                .HasForeignKey(d => d.ThirdChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Choice2");

            entity.HasOne(d => d.Topic).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK_Question_Topic");
        });

        modelBuilder.Entity<QuestionBank>(entity =>
        {
            entity.ToTable("QuestionBank");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Chapter).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_QuestionBank_Chapter");

            entity.HasOne(d => d.SubTopic).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.SubTopicId)
                .HasConstraintName("FK_QuestionBank_SubTopic");

            entity.HasOne(d => d.Topic).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK_QuestionBank_Topic");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.Property(e => e.CreatedBy).HasMaxLength(250);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(250);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");

            entity.Property(e => e.AdmissionDate).HasColumnType("datetime");
            entity.Property(e => e.AdmissionId).HasMaxLength(128);
            entity.Property(e => e.AlternateMobileNumber).HasMaxLength(15);
            entity.Property(e => e.BranchId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.EmailAddress).HasMaxLength(80);
            entity.Property(e => e.FatherName).HasMaxLength(128);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsPasswordChangeRequired).HasDefaultValueSql("((1))");
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasMaxLength(50);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.MotherName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.PinCode).HasMaxLength(10);

            entity.HasOne(d => d.City).WithMany(p => p.Students)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_City");

            entity.HasOne(d => d.State).WithMany(p => p.Students)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_State");
        });

        modelBuilder.Entity<SubTopic>(entity =>
        {
            entity.ToTable("SubTopic");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SubTopic)
                .HasForeignKey<SubTopic>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubTopic_Topic");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");

            entity.Property(e => e.ColorCode).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subject_Course");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("Topic");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.Chapter).WithMany(p => p.Topics)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Topic_Chapter");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ResoUser");

            entity.ToTable("User");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasDefaultValueSql("('Resonance@123')");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.ToTable("Video");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.HomeDisplay).HasMaxLength(5);
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Chapter).WithMany(p => p.Videos)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_Video_Chapter");

            entity.HasOne(d => d.SubTopic).WithMany(p => p.Videos)
                .HasForeignKey(d => d.SubTopicId)
                .HasConstraintName("FK_Video_SubTopic");

            entity.HasOne(d => d.Topic).WithMany(p => p.Videos)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK_Video_Topic");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

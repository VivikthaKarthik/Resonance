﻿using System;
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

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Choice> Choices { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamResult> ExamResults { get; set; }

    public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

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

            entity.HasOne(d => d.Course).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chapter_Course");

            entity.HasOne(d => d.Subject).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chapter_Subject");
        });

        modelBuilder.Entity<Choice>(entity =>
        {
            entity.ToTable("Choice");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.ToTable("Exam");

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(512);
            entity.Property(e => e.ScheduledOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.ToTable("Exam_Result");

            entity.Property(e => e.ConductedOn).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ExamType).HasMaxLength(80);
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.Answer).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Result_Choice");

            entity.HasOne(d => d.Chapter).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_Exam_Result_Chapter");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Result_Exam");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Result_Question");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Result_Exam_Result");

            entity.HasOne(d => d.SubTopic).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.SubTopicId)
                .HasConstraintName("FK_Exam_Result_SubTopic");

            entity.HasOne(d => d.Subject).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Exam_Result_Subject");

            entity.HasOne(d => d.Topic).WithMany(p => p.ExamResults)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK_Exam_Result_Topic");
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

            entity.HasOne(d => d.Choice1).WithMany(p => p.MultipleChoiceQuestionChoice1s)
                .HasForeignKey(d => d.Choice1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice");

            entity.HasOne(d => d.Choice2).WithMany(p => p.MultipleChoiceQuestionChoice2s)
                .HasForeignKey(d => d.Choice2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice1");

            entity.HasOne(d => d.Choice3).WithMany(p => p.MultipleChoiceQuestionChoice3s)
                .HasForeignKey(d => d.Choice3Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice2");

            entity.HasOne(d => d.Choice4).WithMany(p => p.MultipleChoiceQuestionChoice4s)
                .HasForeignKey(d => d.Choice4Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice3");

            entity.HasOne(d => d.CorrectChoice).WithMany(p => p.MultipleChoiceQuestionCorrectChoices)
                .HasForeignKey(d => d.CorrectChoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Choice4");

            entity.HasOne(d => d.Question).WithMany(p => p.MultipleChoiceQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MultipleChoiceQuestion_Question");

            entity.HasOne(d => d.SubTopic).WithMany(p => p.MultipleChoiceQuestions)
                .HasForeignKey(d => d.SubTopicId)
                .HasConstraintName("FK_MultipleChoiceQuestion_SubTopic");

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
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasMaxLength(50);
            entity.Property(e => e.Longitude).HasMaxLength(50);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.MotherName).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.PinCode).HasMaxLength(10);
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

            entity.Property(e => e.CreatedBy).HasMaxLength(128);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
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

            entity.HasIndex(e => e.UserId, "UQ__User__1788CC4D8E078A27").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasDefaultValueSql("('Resonance@123')");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResoUser_Role");
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

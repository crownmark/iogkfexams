using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using IOGKFExams.Server.Models.IOGKFExamsDb;

namespace IOGKFExams.Server.Data
{
    public partial class IOGKFExamsDbContext : DbContext
    {
        public IOGKFExamsDbContext()
        {
        }

        public IOGKFExamsDbContext(DbContextOptions<IOGKFExamsDbContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>()
              .HasOne(i => i.ExamQuestion)
              .WithMany(i => i.ExamAnswers)
              .HasForeignKey(i => i.ExamQuestionsId)
              .HasPrincipalKey(i => i.ExamQuestionsId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>()
              .HasOne(i => i.Exam)
              .WithMany(i => i.ExamQuestions)
              .HasForeignKey(i => i.ExamId)
              .HasPrincipalKey(i => i.ExamId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>()
              .HasOne(i => i.Language)
              .WithMany(i => i.ExamQuestions)
              .HasForeignKey(i => i.LanguageId)
              .HasPrincipalKey(i => i.LanguageId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>()
              .HasOne(i => i.Rank)
              .WithMany(i => i.ExamQuestions)
              .HasForeignKey(i => i.MinimumRankRequiredId)
              .HasPrincipalKey(i => i.RankId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>()
              .HasOne(i => i.Country)
              .WithMany(i => i.Exams)
              .HasForeignKey(i => i.CountryId)
              .HasPrincipalKey(i => i.CountryId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>()
              .HasOne(i => i.ExamStatus)
              .WithMany(i => i.Exams)
              .HasForeignKey(i => i.ExamStatusId)
              .HasPrincipalKey(i => i.ExamStatusId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer>()
              .HasOne(i => i.ExamTemplateQuestion)
              .WithMany(i => i.ExamTemplateAnswers)
              .HasForeignKey(i => i.ExamTemplateQuestionsId)
              .HasPrincipalKey(i => i.ExamTemplateQuestionsId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>()
              .HasOne(i => i.ExamTemplate)
              .WithMany(i => i.ExamTemplateQuestions)
              .HasForeignKey(i => i.ExamTemplateId)
              .HasPrincipalKey(i => i.ExamTemplateId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>()
              .HasOne(i => i.Language)
              .WithMany(i => i.ExamTemplateQuestions)
              .HasForeignKey(i => i.LanguageId)
              .HasPrincipalKey(i => i.LanguageId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>()
              .HasOne(i => i.Rank)
              .WithMany(i => i.ExamTemplateQuestions)
              .HasForeignKey(i => i.MinimumRankRequiredId)
              .HasPrincipalKey(i => i.RankId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>()
              .HasOne(i => i.Language)
              .WithMany(i => i.ExamTemplates)
              .HasForeignKey(i => i.LanguageId)
              .HasPrincipalKey(i => i.LanguageId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer>()
              .Property(p => p.IsSelectedAnswer)
              .HasDefaultValueSql(@"((0))");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion>()
              .Property(p => p.Active)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion>()
              .Property(p => p.Active)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate>()
              .Property(p => p.Active)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.Language>()
              .Property(p => p.Active)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.Rank>()
              .Property(p => p.Active)
              .HasDefaultValueSql(@"((1))");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>()
              .Property(p => p.CreatedDate)
              .HasColumnType("datetimeoffset");

            builder.Entity<IOGKFExams.Server.Models.IOGKFExamsDb.Exam>()
              .Property(p => p.CompletedDate)
              .HasColumnType("datetimeoffset");
            this.OnModelBuilding(builder);
        }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.Country> Countries { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamAnswer> ExamAnswers { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamQuestion> ExamQuestions { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.Exam> Exams { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamStatus> ExamStatuses { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateAnswer> ExamTemplateAnswers { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplateQuestion> ExamTemplateQuestions { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.ExamTemplate> ExamTemplates { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.Language> Languages { get; set; }

        public DbSet<IOGKFExams.Server.Models.IOGKFExamsDb.Rank> Ranks { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
            configurationBuilder.Conventions.Remove(typeof(Microsoft.EntityFrameworkCore.Metadata.Conventions.CascadeDeleteConvention));
            configurationBuilder.Conventions.Remove(typeof(Microsoft.EntityFrameworkCore.Metadata.Conventions.SqlServerOnDeleteConvention));
        }
    }
}
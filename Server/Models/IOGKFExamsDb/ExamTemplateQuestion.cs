using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamTemplateQuestions", Schema = "dbo")]
    public partial class ExamTemplateQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamTemplateQuestionsId { get; set; }

        [Required]
        public int ExamTemplateId { get; set; }

        public ExamTemplate ExamTemplate { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public Language Language { get; set; }

        [Required]
        [MaxLength(500)]
        public string Question { get; set; }

        public bool Active { get; set; }

        [Required]
        public int MinimumRankRequiredId { get; set; }

        public Rank Rank { get; set; }

        [MaxLength(255)]
        public string QuestionImageUrl { get; set; }

        public ICollection<ExamTemplateAnswer> ExamTemplateAnswers { get; set; }
    }
}
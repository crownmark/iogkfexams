using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamQuestions", Schema = "dbo")]
    public partial class ExamQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamQuestionsId { get; set; }

        [Required]
        public int ExamId { get; set; }

        public Exam Exam { get; set; }

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

        public ICollection<ExamAnswer> ExamAnswers { get; set; }
    }
}
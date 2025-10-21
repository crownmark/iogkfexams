using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamTemplateAnswers", Schema = "dbo")]
    public partial class ExamTemplateAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamTemplateAnswerId { get; set; }

        [Required]
        public int ExamTemplateQuestionsId { get; set; }

        public ExamTemplateQuestion ExamTemplateQuestion { get; set; }

        [Column("ExamTemplateAnswer")]
        [Required]
        [MaxLength(500)]
        public string ExamTemplateAnswer1 { get; set; }

        public bool IsCorrectAnswer { get; set; }
    }
}
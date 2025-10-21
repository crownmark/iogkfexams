using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamAnswers", Schema = "dbo")]
    public partial class ExamAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamAnswerId { get; set; }

        [Required]
        public int ExamQuestionsId { get; set; }

        public ExamQuestion ExamQuestion { get; set; }

        [Column("ExamAnswer")]
        [Required]
        [MaxLength(500)]
        public string ExamAnswer1 { get; set; }

        public bool IsCorrectAnswer { get; set; }

        public bool IsSelectedAnswer { get; set; }
    }
}
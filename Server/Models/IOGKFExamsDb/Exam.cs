using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("Exams", Schema = "dbo")]
    public partial class Exam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamId { get; set; }

        [Column("ExamGUID")]
        [MaxLength(36)]
        public string ExamGuid { get; set; }

        public int? ExamStatusId { get; set; }

        public ExamStatus ExamStatus { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public decimal? ExamGrade { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
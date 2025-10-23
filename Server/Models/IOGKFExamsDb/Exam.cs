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

        public DateTimeOffset? CreatedDate { get; set; }

        public DateTimeOffset? CompletedDate { get; set; }

        public decimal? ExamGrade { get; set; }

        public int? ExamSessionCode { get; set; }

        [MaxLength(255)]
        public string StudentFirstName { get; set; }

        [MaxLength(255)]
        public string StudentLastName { get; set; }

        [Column("IOGKFNumber")]
        [MaxLength(50)]
        public string Iogkfnumber { get; set; }

        public int? CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamStatuses", Schema = "dbo")]
    public partial class ExamStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamStatusId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public ICollection<Exam> Exams { get; set; }
    }
}
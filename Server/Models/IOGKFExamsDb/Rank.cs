using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("Ranks", Schema = "dbo")]
    public partial class Rank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RankId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RankName { get; set; }

        public bool Active { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }

        public ICollection<ExamTemplateQuestion> ExamTemplateQuestions { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamTemplates", Schema = "dbo")]
    public partial class ExamTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamTemplateId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ExamTemplateTitle { get; set; }

        public int? LanguageId { get; set; }

        public Language Language { get; set; }

        public bool Active { get; set; }

        public ICollection<ExamTemplateQuestion> ExamTemplateQuestions { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("Languages", Schema = "dbo")]
    public partial class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }

        [MaxLength(255)]
        public string LanguageName { get; set; }

        public bool Active { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }

        public ICollection<ExamTemplateQuestion> ExamTemplateQuestions { get; set; }

        public ICollection<ExamTemplate> ExamTemplates { get; set; }
    }
}
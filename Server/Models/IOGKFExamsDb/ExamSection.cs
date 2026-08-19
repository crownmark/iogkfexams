using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("ExamSections", Schema = "dbo")]
    public partial class ExamSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamSectionId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ExamSectionName { get; set; }

        public bool Active { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }

        public ICollection<ExamTemplateQuestion> ExamTemplateQuestions { get; set; }
    }
}
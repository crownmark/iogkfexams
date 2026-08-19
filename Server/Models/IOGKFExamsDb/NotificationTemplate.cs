using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IOGKFExams.Server.Models.IOGKFExamsDb
{
    [Table("NotificationTemplates", Schema = "dbo")]
    public partial class NotificationTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationTemplateId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public Language Language { get; set; }

        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(4000)]
        public string MessageBody { get; set; }

        [Required]
        [MaxLength(255)]
        public string Channel { get; set; }

        public bool Active { get; set; }
    }
}
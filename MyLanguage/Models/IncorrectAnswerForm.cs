using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Models
{
    public class IncorrectAnswerForm
    {
        [Key]
        public Guid ID { get; set; }
        public string UserName { get; set; }
        public DateTime ExamDate { get; set; }
        public Guid ExamForm_Id { get; set; }

    }
}

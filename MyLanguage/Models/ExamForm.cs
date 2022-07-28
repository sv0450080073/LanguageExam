using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Models
{
    public class ExamForm
    {
        [Key]
        public Guid ID { get; set; }
        public string ExamName {get;set;}
        public DateTime ExamDate { get; set; }
    }
}

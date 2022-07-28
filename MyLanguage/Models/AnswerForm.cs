using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Models
{
    public class AnswerForm
    {
        [Key]
        public Guid ID { get; set; }
        public Guid Exam_ID { get; set; }
       
    }
}

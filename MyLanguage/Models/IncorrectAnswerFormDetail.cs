using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Models
{
    public class IncorrectAnswerFormDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid IncorrectAnswerForm_Id { get; set; }
        public Guid ExamFormDetail_Id { get; set; }
        public int KanJiID { get; set; }
        public string IncorrectKanJiWord { get; set; }
        public string IncorrectAmHanViet { get; set; }
        public string IncorrectVietNamMean { get; set; }
        public string IncorrectAmOn { get; set; }
        public string IncorrectAmKun { get; set; }
        public string IncorrectHiragana { get; set; }
    }
}

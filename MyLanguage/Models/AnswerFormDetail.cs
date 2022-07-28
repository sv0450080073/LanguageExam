using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Models
{
    public class AnswerFormDetail
    {
        [Key]
        public Guid ID { get; set; }
        public Guid AnserForm_ID { get; set; }
        public int KanJiID { get; set; }
        public string KanJiWord { get; set; }
        public string AmHanViet { get; set; }
        public string VietNamMean { get; set; }
        public string AmOn { get; set; }
        public string AmKun { get; set; }
        public string Hiragana { get; set; }
    }
}

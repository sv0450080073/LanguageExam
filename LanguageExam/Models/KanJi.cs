using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExam.Models
{
    public class KanJi
    {
        [Key]
        public int ID { get; set; }
        public string KanJiSingle { get; set; }
        public string HanViet { get; set; }
    }
}

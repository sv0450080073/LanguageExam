using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Models
{
    public class KanJi
    {
        [Key]
        public int ID { get; set; }
        public string KanJiWord { get; set; }
        public string HanViet { get; set; }
        public string VNMean { get; set; }
        public int Level { get; set; }
    }
}

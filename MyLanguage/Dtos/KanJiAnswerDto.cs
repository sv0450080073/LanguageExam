using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class KanJiAnswerDto
    {
        public string HanViet { get; set; } = "";
        public string VNMean { get; set; } = "";
        public string AmOn { get; set; } = "";
        public string AmKun { get; set; } = "";
        public int KanJiId { get; set; } = 0;
        public string ExamFormId { get; set; } = "";
        public string HanVietSave
        {
            get
            {
                return HanViet == null ? "" : HanViet;
            }
        }
        public string VNMeanSave
        {
            get
            {
                return VNMean == null ? "" : VNMean;
            }
        }
        public string AmKunSave
        {
            get
            {
                return AmKun == null ? "" : AmKun;
            }
        }
        public string AmOnSave
        {
            get
            {
                return AmOn == null ? "" : AmOn;
            }
        }

    }
}

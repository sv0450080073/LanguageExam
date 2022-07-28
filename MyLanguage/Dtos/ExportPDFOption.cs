using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class ExportPDFOption
    {
        public int KanJiNumber { get; set; }
        public bool IsAnswer { get; set; }
        public int LevelFrom { get; set; }
        public int LevelTo { get; set; }
        public string UseFor { get; set; }
        public int TestNumber { get; set; }
        public bool IsHideKanJi { get; set; }
        public int KanJiToTal
        {
            get
            {
                return KanJiNumber * TestNumber;
            }
        }
        public string TestAre { get; set; }


    }
}

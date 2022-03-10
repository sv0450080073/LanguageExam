using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class KanJiDto
    {
        public string KanJi { get; set; }
        public string HanViet { get; set; }
        public string Level { get; set; }
        public int LevelNumber
        {
            get
            {
                if (!string.IsNullOrEmpty(Level))
                {
                    switch (Level.ToLower())
                    {
                        case "n5":
                            return 1;
                        case "n4":
                            return 2;
                        case "n3":
                            return 3;
                        case "n2":
                            return 4;
                        case "n1":
                            return 5;
                        default:
                            return 1;
                    }
                }
                return 1;
            }
        }
        public string VietNam { get; set; }
    }
}

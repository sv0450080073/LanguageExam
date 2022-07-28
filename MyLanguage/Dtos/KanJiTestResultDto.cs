using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class KanJiTestResultDto
    {
        public int Score { get; set; }
        public int IncorrectQuestion { get; set; }
        public int TotalQuestion { get; set; }
        public Guid ExamForm_Id { get; set; }
        public Guid IncorrectAnswerForm_Id { get; set; }

    }
}

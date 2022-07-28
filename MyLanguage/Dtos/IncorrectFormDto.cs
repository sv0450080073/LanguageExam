using MyLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class IncorrectFormDto
    {
        public IncorrectAnswerForm IncorrectAnswerForm { get; set; } = new IncorrectAnswerForm();
        public List<IncorrectAnswerFormDetail> IncorrectAnswerFormDetail { get; set; } = new List<IncorrectAnswerFormDetail>();
    }
}

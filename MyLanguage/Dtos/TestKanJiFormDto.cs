using MyLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class TestKanJiFormDto
    {
        public ExamForm ExamForm { get; set; } = new ExamForm();
        public List<ExamFormDetail> ExamFormDetails { get; set; } = new List<ExamFormDetail>();
        
    }
}

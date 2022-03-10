using LanguageExam.Data;
using LanguageExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExam.Services
{
    public interface IKanJiService
    {
        List<KanJi> GetKanJis();
        List<KanJi> SaveKanJis(List<KanJi> kanJis);

    }
    public class KanJiService : IKanJiService
    {
        ExamDbContext _examDbContext = null;
        public KanJiService(ExamDbContext examDbContext)
        {
            _examDbContext = examDbContext;
        }
        public List<KanJi> GetKanJis()
        {
            return _examDbContext.KanJis.ToList();
        }

        public List<KanJi> SaveKanJis(List<KanJi> kanJis)
        {
            _examDbContext.AddRange(kanJis);
            _examDbContext.SaveChanges();
            return kanJis;
        }

    }
}

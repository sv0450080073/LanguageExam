using Microsoft.AspNetCore.Mvc;
using MyLanguage.Dtos;
using MyLanguage.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Controllers
{
    public class TestKanJiPerDayController : Controller
    {
        ITestKanJiPerDayService _testKanJiPerDayService = null;

        public TestKanJiPerDayController(ITestKanJiPerDayService testKanJiPerDayService)
        {
            _testKanJiPerDayService = testKanJiPerDayService;
        }
        public IActionResult Index()
        {
           // _testKanJiPerDayService.InsertTestKanJi();
            return View();
        }
        public IActionResult TestKanJi(string id)
        {
            var kanjiTestForm = new TestKanJiFormDto();
            try
            {
                Guid examFormId = Guid.Parse(id);
                if (examFormId != Guid.Empty)
                {
                    kanjiTestForm = _testKanJiPerDayService.GetTestKanJiByExamFormId(examFormId);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

            }

            return View(kanjiTestForm);
        }

        [HttpPost]
        public JsonResult HandlerTestKanJi(KanJiAnswerDto[] kanJiAnswers)
        {
            var result = new KanJiTestResultDto();
            try
            {
                if (kanJiAnswers.Any())
                {
                    var kanJiAnswerList = kanJiAnswers.ToList();
                    result = _testKanJiPerDayService.HandlerScoreKanJiExam(kanJiAnswerList);
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return new JsonResult(result);
        }
    }
}

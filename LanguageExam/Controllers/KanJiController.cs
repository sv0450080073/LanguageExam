using LanguageExam.Common.ImportExcel;
using LanguageExam.Models;
using LanguageExam.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExam.Controllers
{
    public class KanJiController : Controller
    {
        IKanJiService _kanJiService = null;
        List<KanJi> _kanJis = new List<KanJi>();
        public KanJiController(IKanJiService kanJiService)
        {
            _kanJiService = kanJiService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult SaveKanJis(List<KanJi> kanJis)
        {
            _kanJis = _kanJiService.SaveKanJis(kanJis);
            return Json(_kanJis);
        }
        public string GenerateAndDownloadExcel(int kanJi , string hanViet)
        {
            List<KanJi> kanJis = _kanJiService.GetKanJis();
            var dataTable = ConvertListToDataTable.ConvertListToDataTableT(kanJis);
            byte[] fileContents = null;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage pck  = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("KanJis");
                ws.Cells["A1"].Value = "Kanji";
                ws.Cells["A2"].Value = "HanViet";
                pck.Save();
                fileContents = pck.GetAsByteArray();
            }
            return Convert.ToBase64String(fileContents);
        }

    }
}

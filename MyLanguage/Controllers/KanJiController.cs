using Microsoft.AspNetCore.Mvc;
using MyLanguage.Common;
using MyLanguage.Models;
using MyLanguage.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using SelectPdf;
using MyLanguage.Dtos;
using MyLanguage.Data;
using IronOcr;
using System.IO.Compression;
using static MyLanguage.Common.Enum;

namespace MyLanguage.Controllers
{
    public class KanJiController : Controller
    {
        IKanJiService _kanJiService = null;
        MyLanguageDbContext _dbContext = null;
        List<KanJi> _kanJis = new List<KanJi>();
        public KanJiController(IKanJiService kanJiService, MyLanguageDbContext dbContext)
        {
            _kanJiService = kanJiService;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult SaveKanJis(List<KanJiDto> kanJiDtos)
        {
            _kanJis = _kanJiService.SaveKanJis(kanJiDtos);
            return Json(_kanJis);
        }
        public string GenerateAndDownloadExcel(int kanJiId, string hanViet)
        {
            List<KanJi> kanJis = _kanJiService.GetKanJis();
            var dataTable = ConvertListToDataTable.ConvertListToDataTableT(kanJis);
            byte[] fileContents = null;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("KanJis");
                ws.Cells["A1"].Value = "Kanji";
                ws.Cells["A2"].Value = "HanViet";
                pck.Save();
                fileContents = pck.GetAsByteArray();
            }
            return Convert.ToBase64String(fileContents);
        }

        [HttpPost]
        public IActionResult GenerateAndDownloadPDF(ExportPDFOption exportPDFOption)
        {
            try
            {
                var kanJisByParamSearch = _kanJiService.GetKanJisPDFByParamsSearch(exportPDFOption);
                var resultMutil = _kanJiService.GenerateMutilPDFZip(kanJisByParamSearch, exportPDFOption);
                return File(resultMutil, "application/zip");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public IActionResult GenerateAndDownloadPDFItem(ExportPDFOption exportPDFOption)
        {
            #region Declare string html
            string breakPage = "<div style = 'page-break-after: always'>";
            string breakPagEnd = "</div>";
            string htmlEndTag = "</table> </html>";
            string htmlContent = "<html>" +
                /* breakPage +*/
                "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:10px 30px 0px 100px;'>" +
                "<tr style = 'border: 1px solid #000;'>" +
                "<th style = 'border: 1px solid #000;'> STT </th>" +
                "<th style = 'border: 1px solid #000;'> KanJi </th> " +
                "<th style = 'border: 1px solid #000;' > Âm hán việt </th>" +
                "<th style = 'border: 1px solid #000;' > Ý nghĩa </th>" +
                "</tr>";
            string headerHtml =
               "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:0px 30px 0px 100px;border-bottom: none !important'>" +
               "<tr style = 'border: 1px solid #000;border-bottom: none !important'>" +
               "<th style = 'border: 1px solid #000;width: 15%;border-bottom: none !important'> STT </th>" +
               "<th style = 'border: 1px solid #000;width: 30%;border-bottom: none !important'> KanJi </th> " +
               "<th style = 'border: 1px solid #000;width: 25%;border-bottom: none !important' > Âm hán việt </th>" +
               "<th style = 'border: 1px solid #000;width: 30%;border-bottom: none !important' > Ý nghĩa </th>" +
               "</tr></table>";
            #endregion
            try
            {
                var kanJisByParamSearch = _kanJiService.GetKanJisPDFByParamsSearch(exportPDFOption);
                kanJisByParamSearch.ShuffleRNGCrypto();
                List<InMemoryFileDto> inMemoryFiles = new List<InMemoryFileDto>();
                var strDateTime = DateTime.Now.ToString("yyyyMMddhhmm");
                var extensinonFile = 0 + "_" + strDateTime + ".pdf";
                string pdfBodyNoAnswer = GenerateBodyKanJiPDF(kanJisByParamSearch, Answer.NoAnswer);
                string pdfNoAnswerHtml = pdfBodyNoAnswer;
                HtmlToPdf converter = new HtmlToPdf();

                //converter.Options.DisplayHeader = true;
                //converter.Header.DisplayOnFirstPage = true;
                //converter.Header.DisplayOnOddPages = true;
                //converter.Header.DisplayOnEvenPages = true;
                //converter.Header.Height = 20;
                //PdfHtmlSection headerPDF = new PdfHtmlSection(headerHtml, "");
                //headerPDF.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                //converter.Header.Add(headerPDF);

                SelectPdf.PdfDocument docNoAnswer = converter.ConvertHtmlString(pdfNoAnswerHtml);
                byte[] pdfNoAnswer = docNoAnswer.Save();
                docNoAnswer.Close();
                InMemoryFileDto inMemoryFileNoAnswer = new InMemoryFileDto
                {
                    FileName = "Test_KanJi_" + extensinonFile,
                    Content = pdfNoAnswer
                };
               FileResult fileResult = new FileContentResult(pdfNoAnswer, "application/pdf");
                return fileResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult ReadTextFromImage()
        {
            string imgText = "";
            string imgName = "";
            List<string> fileList = new List<string>();
            string path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}";
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                fileList.Add(Path.GetFileName(file));

            }
            //var Ocr = new AdvancedOcr()
            //{
            //    CleanBackgroundNoise = true,
            //    Language = IronOcr.Languages

            //};
            var result = new IronTesseract();//.Read(path + "/" + fileList[1]);
            result.Language = OcrLanguage.Vietnamese;
            var test = result.Read(path + "/" + fileList[1]);
            imgText = test.Text;
            ViewBag.Text = imgText;
            return View();
        }
        private string GenerateBodyKanJiPDF(List<KanJi> kanJis, Answer answer)
        {
            string result = string.Empty;
            string tableBody = "<table style ='width:75%;border-collapse:collapse;border:1px solid;margin:0px 30px 0px 100px;'>";
            string endTableBody = "</table>";
            result += tableBody;
            foreach (var (item, index) in kanJis.Select((value, i) => (value, i)))
            {
                var hanViet = answer == Answer.NoAnswer ? "" : item.HanViet;
                var vnMean = answer == Answer.NoAnswer ? "" : item.VNMean;
                int stt = index + 1;
                string row =
                    " <tr style='border: 1px solid #000;text-align: center;'>" +
                    " <th style= 'border: 1px solid #000;height: 25px;width: 15%;'>" + stt + "</th>" +
                    " <th style= 'border: 1px solid #000;height: 25px;width: 30%;'> " + item.KanJiWord + " </th>" +
                    "  <th style= 'border: 1px solid #000;height: 25px;width: 25%;'> " + hanViet + "</th>" +
                    " <th style = 'border: 1px solid #000;height: 25px;width: 30%;'> " + vnMean + " </th>" +
                    " </tr>";
                result += row;
                /*string row = "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:0px 30px 0px 100px;'>" +
                "<tr style = 'text-align: center;'>" +
                "<td style = 'border: 1px solid #000;height: 25px;'>" + stt + "</td> " +
                "<td style = 'border: 1px solid #000;height: 25px;'> " + item.KanJiWord + "</td>" +
                "<td style = 'border: 1px solid #000;height: 25px;'> " + hanViet + " </td>" +
                "<td style = 'border: 1px solid #000;height: 25px;'> " + vnMean + " </td>" +
                "</tr></table>";
                result += row;*/
            }
            return result + endTableBody;
        }

    }
}

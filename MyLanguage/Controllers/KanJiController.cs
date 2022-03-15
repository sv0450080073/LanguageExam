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
using Microsoft.Extensions.Logging;

namespace MyLanguage.Controllers
{
    public class KanJiController : Controller
    {
        IKanJiService _kanJiService = null;
        MyLanguageDbContext _dbContext = null;
        private readonly ILogger<KanJiController> _log;
        List<KanJi> _kanJis = new List<KanJi>();
        public KanJiController(IKanJiService kanJiService, MyLanguageDbContext dbContext , ILogger<KanJiController> log)
        {
            _kanJiService = kanJiService;
            _dbContext = dbContext;
            _log = log;
        }
        public IActionResult Index()
        {
            var kanJisByParamSearch = _kanJiService.GetKanJis();
           
            ViewBag.List = kanJisByParamSearch;
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
            _log.LogInformation("Test ne");
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
            string headerHtml0=
               "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:0px 30px 0px 100px;border-bottom: none !important'>" +
               "<tr style = 'border: 1px solid #000;border-bottom: none !important'>" +
               "<th style = 'border: 1px solid #000;width: 10%;border-bottom: none !important'> STT </th>" +
               "<th style = 'border: 1px solid #000;width: 30%;border-bottom: none !important'> KanJi </th> " +
               "<th style = 'border: 1px solid #000;width: 30%;border-bottom: none !important' > Âm hán việt </th>" +
               "<th style = 'border: 1px solid #000;width: 30%;border-bottom: none !important' > Ý nghĩa </th>" +
               "</tr></table>";
            #endregion
            try
            {
                var kanJisByParamSearch = _kanJiService.GetKanJisPDFByParamsSearch(exportPDFOption);
               // kanJisByParamSearch.ShuffleRNGCrypto();
                List<InMemoryFileDto> inMemoryFiles = new List<InMemoryFileDto>();
                var strDateTime = DateTime.Now.ToString("yyyyMMddhhmm");
                var extensinonFile = 0 + "_" + strDateTime + ".pdf";
                string pdfBodyNoAnswer = _kanJiService.GenerateBodyKanJiPDF(kanJisByParamSearch, Answer.NoAnswer, exportPDFOption);
                HtmlToPdf converter = new HtmlToPdf();
                #region HeaderHTML
                converter.Options.DisplayHeader = true;
                converter.Header.DisplayOnFirstPage = true;
                converter.Header.DisplayOnOddPages = true;
                converter.Header.DisplayOnEvenPages = true;
                converter.Header.Height =35;

                // add some html content to the header
                PdfHtmlSection headerHtml = new PdfHtmlSection(GenerateHeaderKanJiPDF(),"");
                headerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                converter.Header.Add(headerHtml);
                #endregion
                #region Footer HTML
                // footer settings
                converter.Options.DisplayFooter = true;
                converter.Footer.DisplayOnFirstPage = true;
                converter.Footer.DisplayOnOddPages = true;
                converter.Footer.DisplayOnEvenPages = true;
                converter.Footer.Height = 40;
                string date = DateTime.Now.ToString("yyyyMMdd");
              // add some html content to the footer
                PdfHtmlSection footerHtml = new PdfHtmlSection(date, "Test");
                footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                converter.Footer.Add(footerHtml);
                // page numbers can be added using a PdfTextSection object
                PdfTextSection text = new PdfTextSection(0, 10, "Page: {page_number}/{total_pages}  ", new System.Drawing.Font("Arial", 8));
                text.HorizontalAlign = PdfTextHorizontalAlign.Right;
                converter.Footer.Add(text);
                #endregion
                converter.Options.MarginTop = 0;
                converter.Options.MarginBottom = 0;
                converter.Options.MarginLeft = 10;
                converter.Options.MarginRight = 10;
                SelectPdf.PdfDocument docNoAnswer = converter.ConvertHtmlString(pdfBodyNoAnswer);
                byte[] pdfNoAnswer = docNoAnswer.Save();
                docNoAnswer.Close();
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
            result.Language = OcrLanguage.Japanese;
            result.Language = OcrLanguage.JapaneseAlphabet;

            var test = result.Read(path + "/" + fileList[0]);
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

        private string GenerateHeaderKanJiPDF()
        {
            string htmlHeader = string.Empty;
            htmlHeader  =
             "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:0px 30px 0px 100px;'>" +
             "<tr style = 'border: 1px solid #000;border-bottom: none !important'>" +
             "<th style = 'border: 1px solid #000;width: 10%;'> Name: </th>" +
             "<th style = 'border: 1px solid #000;width: 30%;'>  </th> " +
             "</tr>" +
             "<tr style = 'border: 1px solid #000;border-bottom: none !important'>" +
             "<th style = 'border: 1px solid #000;width: 10%;'> Class: </th>" +
             "<th style = 'border: 1px solid #000;width: 30%;'>  </th> " +
             "</tr>" +
             "</table>";
            return htmlHeader;
        }

    }
}

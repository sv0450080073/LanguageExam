using MyLanguage.Common;
using MyLanguage.Data;
using MyLanguage.Dtos;
using MyLanguage.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyLanguage.Common.Enum;

namespace MyLanguage.Services
{
    public interface IKanJiService
    {
        List<KanJi> GetKanJis();
        List<KanJi> SaveKanJis(List<KanJiDto> kanJis);
        List<KanJi> GetKanJisPDFByParamsSearch(ExportPDFOption paramSearch);
        List<InMemoryFileDto> GeneratePDFZip(List<KanJi> kanJis, ExportPDFOption paramSearch, int testNumber = 0);
        public byte[] GenerateMutilPDFZip(List<KanJi> kanJis, ExportPDFOption paramSearch);
        List<KanJi> FilterKanjs(List<KanJi> kanJis, Answer answer);
        bool IsExistKanJiInDb(KanJi kanJi);
    }
    public class KanJiService : IKanJiService
    {
        MyLanguageDbContext _dbContext = null;
        public KanJiService(MyLanguageDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<KanJi> GetKanJis()
        {
            return _dbContext.KanJis.ToList();
        }

        public List<KanJi> GetKanJisPDFByParamsSearch(ExportPDFOption paramSearch)
        {
            //todo validate 
            try
            {
                var param = paramSearch;
                var result = (from kanji in _dbContext.KanJis
                              where kanji.Level >= param.LevelFrom
                              && kanji.Level <= param.LevelTo
                              orderby Guid.NewGuid()
                              select kanji).Take(param.KanJiToTal).ToList();
                return result;
            }
            catch (Exception ex)
            {
                //todo log
                throw;
            }
        }

        public List<InMemoryFileDto> GeneratePDFZip(List<KanJi> kanJis, ExportPDFOption paramSearch, int testNumber = 0)
        {
            #region Declare string html
            string headerHtml =
               "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:0px 30px 0px 100px;'>" +
               "<tr style = 'border: 1px solid #000;'>" +
               "<th style = 'border: 1px solid #000;width: 15%'> STT </th>" +
               "<th style = 'border: 1px solid #000;width: 30%'> KanJi </th> " +
               "<th style = 'border: 1px solid #000;width: 25%' > Âm hán việt </th>" +
               "<th style = 'border: 1px solid #000;width: 30%' > Ý nghĩa </th>" +
               "</tr></table>";
            #endregion
            try
            {
                kanJis.ShuffleRNGCrypto();
                List<InMemoryFileDto> inMemoryFiles = new List<InMemoryFileDto>();
                var strDateTime = DateTime.Now.ToString("yyyyMMddhhmm");
                var extensinonFile = testNumber + "_" + strDateTime + ".pdf";
                string pdfBodyNoAnswer = GenerateBodyKanJiPDF(kanJis, Answer.NoAnswer);
                string pdfNoAnswerHtml = pdfBodyNoAnswer;
                HtmlToPdf converter = new HtmlToPdf();
                //converter.Options.DisplayHeader = true;
                //converter.Header.DisplayOnFirstPage = true;
                //converter.Header.DisplayOnOddPages = true;
                //converter.Header.DisplayOnEvenPages = true;
                //converter.Header.Height = 50;
                //PdfHtmlSection headerPDF = new PdfHtmlSection(headerHtml, "");
                //headerPDF.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                //converter.Header.Add(headerPDF);
                converter.Options.MarginTop = 10;
                converter.Options.MarginBottom = 10;
                converter.Options.MarginLeft = 10;
                converter.Options.MarginRight = 10;

                SelectPdf.PdfDocument docNoAnswer = converter.ConvertHtmlString(pdfNoAnswerHtml);
                byte[] pdfNoAnswer = docNoAnswer.Save();
                docNoAnswer.Close();
                InMemoryFileDto inMemoryFileNoAnswer = new InMemoryFileDto
                {
                    FileName = "Test_KanJi_" + extensinonFile,
                    Content = pdfNoAnswer
                };

                if (paramSearch.IsAnswer)
                {
                    string pdfBodyHasAnswer = GenerateBodyKanJiPDF(kanJis, Answer.HasAnswer);
                    string pdfHasAnswerHtml = pdfBodyHasAnswer;
                    SelectPdf.PdfDocument docHasAnswer = converter.ConvertHtmlString(pdfHasAnswerHtml);
                    byte[] pdfHasAnswer = docHasAnswer.Save();
                    docHasAnswer.Close();
                    InMemoryFileDto inMemoryFileHasAnswer = new InMemoryFileDto
                    {
                        FileName = "KanJi_Answer_" + extensinonFile,
                        Content = pdfHasAnswer
                    };
                    inMemoryFiles.Add(inMemoryFileHasAnswer);
                }
                inMemoryFiles.Add(inMemoryFileNoAnswer);
                return inMemoryFiles;
            }
            catch (Exception ex)
            {
                //to do log
                throw;
            }
        }

        public byte[] GenerateMutilPDFZip(List<KanJi> kanJis, ExportPDFOption paramSearch)
        {
            List<InMemoryFileDto> inMemoryFileDtos = new List<InMemoryFileDto>();
            for (int i = 0; i < paramSearch.TestNumber; i++)
            {
                var kanjisPerTest = kanJis.Skip(i * paramSearch.KanJiNumber).Take(paramSearch.KanJiNumber).ToList();
                var pdfItem = GeneratePDFZip(kanjisPerTest, paramSearch, i + 1);
                inMemoryFileDtos.AddRange(pdfItem);
            }
            var result = HandlerFile.GetZipArchive(inMemoryFileDtos);
            return result;

        }
        public List<KanJi> SaveKanJis(List<KanJiDto> kanJiDtos)
        {
            List<KanJi> kanJis = new List<KanJi>();
            if (kanJiDtos.Any())
            {
                foreach (var item in kanJiDtos)
                {
                    var kanJiItem = CreateKanJiItem(item);
                    /*   if (!IsExistKanJiInDb(kanJiItem))*/
                    kanJis.Add(kanJiItem);
                }
            }
            _dbContext.AddRange(kanJis);
            _dbContext.SaveChanges();
            return kanJis;
        }
        public bool IsExistKanJiInDb(KanJi kanJi)
        {
            if (string.IsNullOrEmpty(kanJi.KanJiWord)) return true;
            return _dbContext.KanJis.Any(x => x.KanJiWord == kanJi.KanJiWord);
        }
        private KanJi CreateKanJiItem(KanJiDto data)
        {
            KanJi kanJi = new KanJi();
            try
            {
                if (data != null)
                {
                    kanJi.KanJiWord = data.KanJi;
                    kanJi.HanViet = data.HanViet;
                    kanJi.Level = data.LevelNumber;
                    kanJi.VNMean = data.VietNam;
                }
                return kanJi;
            }
            catch (Exception)
            {
                //todo log
                return kanJi;
            }
        }
        public List<KanJi> FilterKanjs(List<KanJi> kanJis, Answer answer)
        {
            //todo validate 
            try
            {
                if (answer == Answer.HasAnswer)
                {
                    return kanJis;
                }
                else
                {
                    var result = (from kanji in kanJis
                                  select new KanJi
                                  {
                                      KanJiWord = kanji.KanJiWord
                                  }).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //todo log
                throw;
            }
        }
        private string GenerateBodyKanJiPDF(List<KanJi> kanJis, Answer answer)
        {
            string result = string.Empty;
            string htmlEndTag = "</table> </body> </html>";
            string htmlStartTag = "<html> <body>";
            try
            {
                int kanJiNumberPerPage = 50;
                int kanjiNumberTotal = kanJis.Count();
                int loopNumber = kanjiNumberTotal / kanJiNumberPerPage;
                result += htmlStartTag;
                for (int i = 0; i < loopNumber; i++)
                {
                    var kanjiPerPage = kanJis.Skip(i * kanJiNumberPerPage).Take(kanJiNumberPerPage).ToList();
                    result += GenerateRowBodyKanJiPDF(kanjiPerPage, answer, i == loopNumber - 1 ? false : true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result + htmlEndTag;
        }
        private string GenerateRowBodyKanJiPDF(List<KanJi> kanJis, Answer answer, bool isBreakPage)
        {
            #region HTML          
            string htmlContent =
                "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:10px 30px 0px 100px;'>" +
                "<tr style = 'border: 1px solid #000;font-size: 20px;'>" +
                "<th style = 'border: 1px solid #000;width:15%;'> STT </th>" +
                "<th style = 'border: 1px solid #000;width:30%;'> KanJi </th> " +
                "<th style = 'border: 1px solid #000;width:25%;' > Âm hán việt </th>" +
                "<th style = 'border: 1px solid #000;width:30%;' > Ý nghĩa </th>" +
                "</tr>";
            #endregion
            try
            {
                //50 row into function
                string breakPage = isBreakPage ? "<div style = 'page-break-after: always'>" : string.Empty;
                string breakPageEnd = isBreakPage ? "</div>" : string.Empty;
                string result = string.Empty;
                result += breakPage + htmlContent;
                string row = string.Empty;
                foreach (var (item, index) in kanJis.Select((value, i) => (value, i)))
                {
                    var hanViet = answer == Answer.NoAnswer ? "" : item.HanViet;
                    var vnMean = answer == Answer.NoAnswer ? "" : item.VNMean;
                    int stt = index + 1;
                    row = "<table style = 'width:75%;border-collapse:collapse; border: 1px solid;margin:0px 30px 0px 100px;'>" +
                        "<tr style = 'text-align: center;'>" +
                        "<td style = 'border: 1px solid #000;height: 25px;width:15%;'>" + stt + "</td> " +
                        "<td style = 'border: 1px solid #000;height: 25px;width:30%;'> " + item.KanJiWord + "</td>" +
                        "<td style = 'border: 1px solid #000;height: 25px;width:25%;'> " + hanViet + " </td>" +
                        "<td style = 'border: 1px solid #000;height: 25px;width:30%;'> " + vnMean + " </td>" +
                        "</tr></table>";
                    result += row;
                    //row =
                    //    " <tr style='border: 1px solid #000;text-align: center;'>" +
                    //    " <th style= 'border: 1px solid #000;height: 25px;width: 15%;'>" + stt + "</th>" +
                    //    " <th style= 'border: 1px solid #000;height: 25px;width: 30%;'> " + item.KanJiWord + " </th>" +
                    //    "  <th style= 'border: 1px solid #000;height: 25px;width: 25%;'> " + hanViet + "</th>" +
                    //    " <th style = 'border: 1px solid #000;height: 25px;width: 30%;'> " + vnMean + " </th>" +
                    //    " </tr>";
                }
                return result + breakPageEnd;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public class InMemoryFile
        {
            public string FileName { get; set; }
            public byte[] Content { get; set; }
        }
    }
}

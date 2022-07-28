using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyLanguage.Data;
using MyLanguage.Dtos;
using MyLanguage.ManagerLog;
using MyLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Services
{
    public interface ITestKanJiPerDayService
    {
        Guid InsertExamForm(List<KanJi> kanJis);
        Guid InsertAnserForm(List<KanJi> kanJis, Guid answerFormID);
        void InsertTestKanJi();
        ExamForm GetExamFormById(Guid id);
        List<ExamFormDetail> GetExamFormDetailByExamFormId(Guid examFormID);
        TestKanJiFormDto GetTestKanJiByExamFormId(Guid examFormID);
        KanJiTestResultDto HandlerScoreKanJiExam(List<KanJiAnswerDto> kanJiAnswers);

    }
    public class TestKanJiPerDayService : ITestKanJiPerDayService
    {
        MyLanguageDbContext _dbContext = null;
        private readonly ILoggerManager _logger;
        private readonly IKanJiService _kanJiService;
        public TestKanJiPerDayService(MyLanguageDbContext dbContext, ILoggerManager logger,
            IKanJiService kanJiService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _kanJiService = kanJiService;
        }
        public Guid InsertAnserForm(List<KanJi> kanJis, Guid examFormID)
        {
            try
            {
                //todo begin transaction 


                //add Answer
                Guid answerFormIDMax = new Guid();
                if (kanJis.Any())
                {
                    //insert answer
                    var anserForm = new AnswerForm
                    {
                        Exam_ID = examFormID
                    };
                    _dbContext.AddRange(anserForm);
                    _dbContext.SaveChanges();
                    Guid answerIDInserted = anserForm.ID;
                    //  answerFormIDMax = _dbContext.AnswerForm.Last().ID;

                    //   var answerFormList = CreateAnswerFormDetailList(kanJis, answerIDInserted);
                    // _dbContext.AddRange(answerFormList);
                    _dbContext.SaveChanges();
                    _logger.Info(typeof(string), "Save AFDT success !");
                }
                else
                {
                    _logger.Info(typeof(string), string.Format("InsertAnserForm: {0} - {1} ", DateTime.Now, 0));
                }


                return answerFormIDMax;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetType(), $"GetKanJiByNumber Error: ", ex);
                throw;
            }
        }

        public Guid InsertExamForm(List<KanJi> kanJis)
        {
            try
            {
                //add examform
                var examItem = new ExamForm
                {
                    ExamName = "Test" + DateTime.Now.ToString("ddMMyy"),
                    ExamDate = DateTime.Now
                };
                _dbContext.AddRange(examItem);
                _dbContext.SaveChanges();
                Guid examIDInserted = examItem.ID;
                Guid examIDMax = _dbContext.ExamForm.OrderBy(x => x.ID).Last().ID;
                if (kanJis.Any())
                {
                    var examFormList = CreateExamFormDetailList(kanJis, examIDInserted);
                    _dbContext.AddRange(examFormList);
                    _dbContext.SaveChanges();
                    _logger.Info(typeof(string), "Save EXFDT success !");
                }
                else
                {
                    _logger.Info(typeof(string), string.Format("InsertExamForm: {0} - {1} ", DateTime.Now, 0));
                }
                return examIDMax;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetType(), $"GetKanJiByNumber Error: ", ex);
                throw;
            }

        }
        public List<ExamFormDetail> CreateExamFormDetailList(List<KanJi> kanJis, Guid examFormID)
        {
            List<ExamFormDetail> examFormDetailList = new List<ExamFormDetail>();
            foreach (var item in kanJis)
            {
                examFormDetailList.Add(CreateExamFormDetailItem(item, examFormID));
            }
            return examFormDetailList;
        }

        public ExamFormDetail CreateExamFormDetailItem(KanJi kanJi, Guid examFormID)
        {
            var examFormDetail = new ExamFormDetail
            {
                ExamForm_ID = examFormID,
                KanJiID = kanJi.ID,
                KanJiWord = kanJi.KanJiWord,
                AmHanViet = kanJi.HanViet,
                VietNamMean = kanJi.VNMean,
                AmOn = "",
                AmKun = "",
                Hiragana = ""
            };
            return examFormDetail;
        }

        public List<AnswerFormDetail> CreateAnswerFormDetailList(List<KanJi> kanJis, Guid answerFormID)
        {
            List<AnswerFormDetail> answerFormDetailList = new List<AnswerFormDetail>();
            foreach (var item in kanJis)
            {
                answerFormDetailList.Add(CreateAnswerFormDetailItem(item, answerFormID));
            }
            return answerFormDetailList;
        }

        public AnswerFormDetail CreateAnswerFormDetailItem(KanJi kanJi, Guid answerFormID)
        {
            var answerFormDetail = new AnswerFormDetail
            {
                AnserForm_ID = answerFormID,
                KanJiID = kanJi.ID,
                KanJiWord = kanJi.KanJiWord,
                AmHanViet = kanJi.HanViet,
                VietNamMean = kanJi.VNMean,
                AmOn = "",
                AmKun = "",
                Hiragana = "",
            };
            return answerFormDetail;
        }

        public void InsertTestKanJi()
        {

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var kanjiTestList = _kanJiService.GetKanJiByNumber(50);
                    var examFormId = InsertExamForm(kanjiTestList);
                    _logger.Info(typeof(string), "InsertTestKanJi success ! ");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex.GetType(), "Error InsertTestKanJi:  ", ex);
                    throw;
                }
            }
        }

        public ExamForm GetExamFormById(Guid id)
        {
            var examForm = new ExamForm();
            try
            {
                examForm = _dbContext.ExamForm.Where(x => x.ID == id).FirstOrDefault();
                return examForm;
            }
            catch (Exception ex)
            {
                return examForm;
                throw;
            }
        }

        public List<ExamFormDetail> GetExamFormDetailByExamFormId(Guid examFormID)
        {
            var examFormDetails = new List<ExamFormDetail>();
            try
            {
                examFormDetails = _dbContext.ExamFormDetail.Where(x => x.ExamForm_ID == examFormID).ToList();
                return examFormDetails;
            }
            catch (Exception ex)
            {
                return examFormDetails;
                throw;
            }
        }

        public TestKanJiFormDto GetTestKanJiByExamFormId(Guid examFormID)
        {
            var result = new TestKanJiFormDto();
            try
            {
                result.ExamForm = GetExamFormById(examFormID);
                result.ExamFormDetails = GetExamFormDetailByExamFormId(examFormID);
                return result;
            }
            catch (Exception ex)
            {
                return result;
                throw;
            }
        }

        public KanJiTestResultDto HandlerScoreKanJiExam(List<KanJiAnswerDto> kanJiAnswers)
        {
            var result = new KanJiTestResultDto();
            int score = 0;
            try
            {

                if (kanJiAnswers.Any())
                {
                    var examForm_ID = Guid.Parse(kanJiAnswers.FirstOrDefault().ExamFormId);
                    var testKanJiForm = new TestKanJiFormDto();
                    testKanJiForm.ExamForm = GetExamFormById(examForm_ID);
                    testKanJiForm.ExamFormDetails = GetExamFormDetailByExamFormId(examForm_ID);
                    var incorrectFormDto = CalculateIncorrectQuestions(kanJiAnswers, testKanJiForm);
                    int incorrectQuestion = incorrectFormDto.IncorrectAnswerFormDetail.Count();
                    score = 2 * (50 - incorrectQuestion);
                    result.ExamForm_Id = incorrectFormDto.IncorrectAnswerForm.ExamForm_Id;
                    result.Score = score;
                    result.TotalQuestion = 50;
                    result.IncorrectAnswerForm_Id = incorrectFormDto.IncorrectAnswerForm.ID;
                    result.IncorrectQuestion = incorrectQuestion;
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
                _logger.Error(ex.GetType(), "HandlerScore: ", ex);
            }
        }
        public IncorrectFormDto CalculateIncorrectQuestions(List<KanJiAnswerDto> kanJiAnswers, TestKanJiFormDto testKanJiFormDtos)
        {
            var incorrectFormDto = new IncorrectFormDto();
            var result = new IncorrectFormDto();
            try
            {
                if (kanJiAnswers.Any() && testKanJiFormDtos.ExamFormDetails.Any())
                {
                    foreach (var item in testKanJiFormDtos.ExamFormDetails)
                    {
                        var kanJiAnswerItem = kanJiAnswers.Where(x => x.KanJiId == item.KanJiID).FirstOrDefault();
                        if (kanJiAnswerItem != null)
                        {
                            var incorrectAnswerFormDetailItem = CompareAnswerAndQuestionItem(kanJiAnswerItem, item);
                            if (incorrectAnswerFormDetailItem != null)
                            {
                                incorrectFormDto.IncorrectAnswerFormDetail.Add(incorrectAnswerFormDetailItem);
                            }
                        }
                    }
                    int incorrectFormDetailRow = incorrectFormDto.IncorrectAnswerFormDetail.Count();
                    if (incorrectFormDetailRow > 0)
                    {
                        var incorrectAnswerFormItem = new IncorrectAnswerForm
                        {
                            ExamForm_Id = testKanJiFormDtos.ExamForm.ID,
                            ExamDate = DateTime.Now,
                            UserName = "Manh"
                        };
                        incorrectFormDto.IncorrectAnswerForm = incorrectAnswerFormItem;
                        result = InsertIncorrectAnswer(incorrectFormDto);
                    }

                }
                return incorrectFormDto;
            }
            catch (Exception ex)
            {
                return result;
                throw;
            }
        }
        public IncorrectAnswerFormDetail CompareAnswerAndQuestionItem(KanJiAnswerDto kanJiAnswerItem, ExamFormDetail examFormDetailItem)
        {
            IncorrectAnswerFormDetail incorrectFormDetailItem = new IncorrectAnswerFormDetail();
            if (kanJiAnswerItem.KanJiId == examFormDetailItem.KanJiID)
            {
                if (kanJiAnswerItem.HanVietSave == null || kanJiAnswerItem.HanVietSave.ToLower() != examFormDetailItem.AmHanViet.ToLower())
                {
                    incorrectFormDetailItem.IncorrectAmHanViet = kanJiAnswerItem.HanVietSave == null ? "" : kanJiAnswerItem.HanVietSave;
                }
                if (kanJiAnswerItem.VNMeanSave == null || kanJiAnswerItem.VNMean.ToLower() != examFormDetailItem.VietNamMean.ToLower())
                {
                    incorrectFormDetailItem.IncorrectVietNamMean = kanJiAnswerItem.VNMeanSave == null ? "" : kanJiAnswerItem.VNMeanSave;
                }
                if (kanJiAnswerItem.AmKunSave == null || kanJiAnswerItem.AmKun.ToLower() != examFormDetailItem.AmKun.ToLower())
                {
                    incorrectFormDetailItem.IncorrectAmKun = kanJiAnswerItem.AmKunSave == null ? "" : kanJiAnswerItem.AmKunSave;
                }
                if (kanJiAnswerItem.AmOnSave == null || kanJiAnswerItem.AmOn.ToLower() != examFormDetailItem.AmOn.ToLower())
                {
                    incorrectFormDetailItem.IncorrectAmOn = kanJiAnswerItem.AmOnSave == null ? "" : kanJiAnswerItem.AmOnSave;
                }
                incorrectFormDetailItem.ExamFormDetail_Id = examFormDetailItem.ID;
            }
            else
            {
                _logger.ErrorMessage("CompareAnswerAndQuestionItem: KanJiID Form and KanJiID Answer Compare fail with ExamFormId =  " + kanJiAnswerItem.ExamFormId);
            }
            return incorrectFormDetailItem;
        }
        public IncorrectFormDto InsertIncorrectAnswer(IncorrectFormDto incorrectFormDto)
        {
            try
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (incorrectFormDto.IncorrectAnswerFormDetail.Count > 0)
                        {
                            var incorrectAnswerForm = incorrectFormDto.IncorrectAnswerForm;
                            var incorrectAnswerFormDetail = incorrectFormDto.IncorrectAnswerFormDetail;
                            _dbContext.AddRange(incorrectAnswerForm);
                            _dbContext.SaveChanges();
                            Guid incorrectAnswer = _dbContext.IncorrectAnswerForm.OrderBy(x => x.ID).Last().ID;
                            incorrectAnswerFormDetail.ForEach(x => x.IncorrectAnswerForm_Id = incorrectAnswer);
                            _dbContext.AddRange(incorrectAnswerFormDetail);
                            _dbContext.SaveChanges();
                            incorrectFormDto.IncorrectAnswerForm.ID = incorrectAnswer;
                        }
                        transaction.Commit();
                        _logger.Info(typeof(string), "InsertIncorrectAnswer success ! ");
                        return incorrectFormDto;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.Error(ex.GetType(), "Error InsertIncorrectAnswer:  ", ex);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
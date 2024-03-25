using AutoMapper;
using Azure;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly ICommonService commonService;
        public ReportService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper, ICommonService _commonService)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
            commonService = _commonService;
        }
                
        public async Task<SubjectWiseTestDto> TrackYourProgressBySubject(long subjectId)
        {
            var currentUser = authService.GetCurrentUser();
            SubjectWiseTestDto report = new SubjectWiseTestDto();
            try
            {
                if (subjectId > 0 && dbContext.Subjects.Any(x => x.Id == subjectId))
                {
                    var subject = dbContext.VwSubjects.First(x => x.Id == subjectId);
                    report.Title = "CWT";
                    report.Course = subject.Course;
                    report.Subject = subject.Name;
                    report.Class = subject.Class;

                    var chapters = dbContext.VwChapters.Any(x => x.SubjectId == subjectId) ? dbContext.VwChapters.Where(x => x.SubjectId == subjectId).ToList() : null;
                    if (chapters != null && chapters.Count > 0)
                    {
                        var chapterIds = chapters.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.ChapterId != null && chapterIds.Contains(session.ChapterId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var chapterQuestions = questionsQuery.ToList();

                        if (chapterQuestions != null && chapterQuestions.Count > 0)
                        {
                            report.TotalQuestions = chapterQuestions.Count;
                            report.TotalQuestionsAttempted = chapterQuestions.Count(x => x.Result != null);
                            report.TotalCorrect = chapterQuestions.Count(x => x.Result != null && x.Result == true);
                            report.TotalWrong = chapterQuestions.Count(x => x.Result != null && x.Result == false);
                            report.CorrectAnswersPercentage = ((double)report.TotalCorrect / report.TotalQuestions) * 100;
                            report.WrongAnswersPercentage = ((double)report.TotalWrong / report.TotalQuestions) * 100;
                        }
                        else
                        {
                            report.TotalQuestions = 0;
                            report.TotalQuestionsAttempted = 0;
                            report.TotalCorrect = 0;
                            report.TotalWrong = 0;
                            report.CorrectAnswersPercentage = 0;
                            report.WrongAnswersPercentage = 0;
                        }
                        report.Reports = new List<ItemWiseReportDto>();
                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        foreach (var chapter in chapters)
                        {
                            ItemWiseReportDto itemWiseReportDto = new ItemWiseReportDto();
                            itemWiseReportDto.Id = chapter.Id;
                            itemWiseReportDto.Name = chapter.Name;

                            var levels = dbContext.AssessmentLevels.ToList();
                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelReportDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelReportDto item = new AssessmentLevelReportDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (chapterQuestions != null && chapterQuestions.Count > 0 && assessmentSessions.Any(x => x.AssessmentLevelId == level.Id && x.ChapterId == chapter.Id && x.EndTime != null))
                                    {
                                        item.Status = "Completed";
                                        var session = assessmentSessions.First(x => x.AssessmentLevelId == level.Id && x.ChapterId == chapter.Id && x.EndTime != null);
                                        var questions = chapterQuestions.Where(x => x.AssessmentSessionId == session.Id).ToList();
                                        item.AssessmentId = session.Id;
                                        if (questions != null && questions.Count > 0)
                                        {
                                            item.TotalQuestions = questions.Count;
                                            item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                            item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                        }
                                    }
                                    else
                                    {
                                        item.Status = "Pending";
                                        item.TotalQuestions = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }
        public async Task<ChapterWiseTestDto> TrackYourProgressByChapter(long chapterId)
        {
            var currentUser = authService.GetCurrentUser();
            ChapterWiseTestDto report = new ChapterWiseTestDto();
            try
            {
                if (chapterId > 0 && dbContext.Chapters.Any(x => x.Id == chapterId))
                {
                    var chapter = dbContext.VwChapters.First(x => x.Id == chapterId);
                    report.Title = "TWT";
                    report.Course = chapter.Course;
                    report.Subject = chapter.Subject;
                    report.Class = chapter.Class;
                    report.Chapter = chapter.Name;

                    var topics = dbContext.VwTopics.Any(x => x.ChapterId == chapterId) ? dbContext.VwTopics.Where(x => x.ChapterId == chapterId).ToList() : null;
                    if (topics != null && topics.Count > 0)
                    {
                        var topicIds = topics.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.TopicId != null && topicIds.Contains(session.TopicId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var topicQuestions = questionsQuery.ToList();

                        if (topicQuestions != null && topicQuestions.Count > 0)
                        {
                            report.TotalQuestions = topicQuestions.Count;
                            report.TotalQuestionsAttempted = topicQuestions.Count(x => x.Result != null);
                            report.TotalCorrect = topicQuestions.Count(x => x.Result != null && x.Result == true);
                            report.TotalWrong = topicQuestions.Count(x => x.Result != null && x.Result == false);
                            report.CorrectAnswersPercentage = ((double)report.TotalCorrect / report.TotalQuestions) * 100;
                            report.WrongAnswersPercentage = ((double)report.TotalWrong / report.TotalQuestions) * 100;
                        }
                        else
                        {
                            report.TotalQuestions = 0;
                            report.TotalQuestionsAttempted = 0;
                            report.TotalCorrect = 0;
                            report.TotalWrong = 0;
                            report.CorrectAnswersPercentage = 0;
                            report.WrongAnswersPercentage = 0;
                        }
                        report.Reports = new List<ItemWiseReportDto>();
                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        foreach (var topic in topics)
                        {
                            ItemWiseReportDto itemWiseReportDto = new ItemWiseReportDto();
                            itemWiseReportDto.Id = topic.Id;
                            itemWiseReportDto.Name = topic.Name;

                            var levels = dbContext.AssessmentLevels.ToList();
                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelReportDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelReportDto item = new AssessmentLevelReportDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (topicQuestions != null && topicQuestions.Count > 0 && assessmentSessions.Any(x => x.AssessmentLevelId == level.Id && x.TopicId == topic.Id && x.EndTime != null))
                                    {
                                        item.Status = "Completed";
                                        var session = assessmentSessions.First(x => x.AssessmentLevelId == level.Id && x.TopicId == topic.Id && x.EndTime != null);
                                        var questions = topicQuestions.Where(x => x.AssessmentSessionId == session.Id).ToList();
                                        item.AssessmentId = session.Id;
                                        if (questions != null && questions.Count > 0)
                                        {
                                            item.TotalQuestions = questions.Count;
                                            item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                            item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                        }
                                    }
                                    else
                                    {
                                        item.Status = "Pending";
                                        item.TotalQuestions = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return report;
        }
        public async Task<TopicWiseTestDto> TrackYourProgressByTopic(long topicId)
        {
            var currentUser = authService.GetCurrentUser();
            TopicWiseTestDto report = new TopicWiseTestDto();
            try
            {
                if (topicId > 0 && dbContext.Topics.Any(x => x.Id == topicId))
                {
                    var topic = dbContext.VwTopics.First(x => x.Id == topicId);
                    report.Title = "TWT";
                    report.Course = topic.Course;
                    report.Subject = topic.Subject;
                    report.Class = topic.Class;
                    report.Chapter = topic.Chapter;
                    report.Topic = topic.Name;

                    var subTopics = dbContext.VwSubTopics.Any(x => x.TopicId == topicId) ? dbContext.VwSubTopics.Where(x => x.TopicId == topicId).ToList() : null;
                    if (subTopics != null && subTopics.Count > 0)
                    {
                        var subTopicIds = subTopics.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.SubTopicId != null && subTopicIds.Contains(session.SubTopicId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var subTopicQuestions = questionsQuery.ToList();

                        if (subTopicQuestions != null && subTopicQuestions.Count > 0)
                        {
                            report.TotalQuestions = subTopicQuestions.Count;
                            report.TotalQuestionsAttempted = subTopicQuestions.Count(x => x.Result != null);
                            report.TotalCorrect = subTopicQuestions.Count(x => x.Result != null && x.Result == true);
                            report.TotalWrong = subTopicQuestions.Count(x => x.Result != null && x.Result == false);
                            report.CorrectAnswersPercentage = ((double)report.TotalCorrect / report.TotalQuestions) * 100;
                            report.WrongAnswersPercentage = ((double)report.TotalWrong / report.TotalQuestions) * 100;
                        }
                        else
                        {
                            report.TotalQuestions = 0;
                            report.TotalQuestionsAttempted = 0;
                            report.TotalCorrect = 0;
                            report.TotalWrong = 0;
                            report.CorrectAnswersPercentage = 0;
                            report.WrongAnswersPercentage = 0;
                        }
                        report.Reports = new List<ItemWiseReportDto>();
                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        foreach (var subTopic in subTopics)
                        {
                            ItemWiseReportDto itemWiseReportDto = new ItemWiseReportDto();
                            itemWiseReportDto.Id = subTopic.Id;
                            itemWiseReportDto.Name = subTopic.Name;

                            var levels = dbContext.AssessmentLevels.ToList();
                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelReportDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelReportDto item = new AssessmentLevelReportDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (subTopicQuestions != null && subTopicQuestions.Count > 0 && assessmentSessions.Any(x => x.AssessmentLevelId == level.Id && x.SubTopicId == subTopic.Id && x.EndTime != null))
                                    {
                                        item.Status = "Completed";
                                        var session = assessmentSessions.First(x => x.AssessmentLevelId == level.Id && x.SubTopicId == subTopic.Id && x.EndTime != null);
                                        var questions = subTopicQuestions.Where(x => x.AssessmentSessionId == session.Id).ToList();
                                        item.AssessmentId = session.Id;
                                        if (questions != null && questions.Count > 0)
                                        {
                                            item.TotalQuestions = questions.Count;
                                            item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                            item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                        }
                                    }
                                    else
                                    {
                                        item.Status = "Pending";
                                        item.TotalQuestions = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return report;
        }

        public async Task<AssessmentReportDto> GetAssessmentReport(long assessmentId)
        {
            var currentUser = authService.GetCurrentUser();
            AssessmentReportDto report = new AssessmentReportDto();
            var assessmentSession = dbContext.AssessmentSessions.Where(x => x.Id == assessmentId).FirstOrDefault();

            if (assessmentSession != null)
            {
                report.AssessmentId = assessmentSession.Id;
                report.AssessmentName = assessmentSession.Name;
                report.PracticedOn = assessmentSession.StartTime.Value;
                report.AssessmentLevel = dbContext.AssessmentLevels.First(x => x.Id == assessmentSession.AssessmentLevelId).Name;

                if(assessmentSession.ChapterId != null && assessmentSession.ChapterId > 0)
                {
                    report.AssessmentType = "CWT";
                    report.ChapterName = dbContext.Chapters.First(x => x.Id == assessmentSession.ChapterId).Name;
                }
                else if (assessmentSession.TopicId != null && assessmentSession.TopicId > 0)
                {
                    report.AssessmentType = "TWT";
                    report.TopicName = dbContext.Topics.First(x => x.Id == assessmentSession.TopicId).Name;
                }
                else if (assessmentSession.SubTopicId != null && assessmentSession.SubTopicId > 0)
                {
                    report.AssessmentType = "QPT";
                    report.SubTopicName = dbContext.SubTopics.First(x => x.Id == assessmentSession.SubTopicId).Name;
                }

                var questions = dbContext.AssessmentSessionQuestions.Where(x => x.AssessmentSessionId == assessmentId && x.Result != null).ToList();
                if (questions != null && questions.Count > 0)
                {
                    report.TotalAttempted = questions.Count;
                    report.CorrectAnswers = questions.Where(x => x.Result.Value).Count();
                    report.WrongAnswers = questions.Where(x => !x.Result.Value).Count();

                    report.Keys = new List<QuestionKeyDto>();
                    foreach (var item in questions)
                    {
                        var question = dbContext.QuestionBanks.First(x => x.Id == item.QuestionId);
                        question.Question = ReplaceMobileText(question.Question);
                        question.FirstAnswer = ReplaceMobileText(question.FirstAnswer);
                        question.SecondAnswer = ReplaceMobileText(question.SecondAnswer);
                        question.ThirdAnswer = ReplaceMobileText(question.ThirdAnswer);
                        question.FourthAnswer = ReplaceMobileText(question.FourthAnswer);
                        if (question != null)
                        {
                            var key = mapper.Map<QuestionKeyDto>(question);
                            if (key != null)
                            {
                                key.SelectedAnswer = item.SelectedAnswer;
                                report.Keys.Add(key);
                            }

                        }
                    }
                }

            }

            return report;
        }

        private string ReplaceMobileText(string source)
        {
            string updated = source.
                Replace(QuestionAndAnswerTags.QuestionImageOpeningTag, "<Image style={styles.questionImage} source={{ uri: ").
                Replace(QuestionAndAnswerTags.ImageClosingTag, "}} />").
                Replace(QuestionAndAnswerTags.QuestionTextOpeningTag, "<Text style={styles.questionText}>").
                Replace(QuestionAndAnswerTags.AnswerImageOpeningTag, "<Image style={styles.answerImage} source={{ uri: ").
                Replace(QuestionAndAnswerTags.AnswerTextOpeningTag, "<Text>").
                Replace(QuestionAndAnswerTags.TextClosingTag, "</Text>").
                Replace(QuestionAndAnswerTags.NewLineTag, "\n");
            return updated;
        }

        public async Task<SubjectWiseTimeAnalysisDto> TimeSpentAnalysisBySubject(long subjectId)
        {
            var currentUser = authService.GetCurrentUser();
            SubjectWiseTimeAnalysisDto report = new SubjectWiseTimeAnalysisDto();
            try
            {
                if (subjectId > 0 && dbContext.Subjects.Any(x => x.Id == subjectId))
                {
                    var subject = dbContext.VwSubjects.First(x => x.Id == subjectId);
                    report.Title = "CWT";
                    report.Course = subject.Course;
                    report.Subject = subject.Name;
                    report.Class = subject.Class;

                    var chapters = dbContext.VwChapters.Any(x => x.SubjectId == subjectId) ? dbContext.VwChapters.Where(x => x.SubjectId == subjectId).ToList() : null;
                    if (chapters != null && chapters.Count > 0)
                    {
                        var chapterIds = chapters.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.ChapterId != null && chapterIds.Contains(session.ChapterId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var chapterQuestions = questionsQuery.ToList();

                        if (chapterQuestions != null && chapterQuestions.Count > 0)
                        {
                            report.AverageTimeSpentOnEachQuestion = chapterQuestions.Sum(x => x.TimeToComplete) / chapterQuestions.Count;
                            report.AverageTimeSpentOnEasyQuestions = chapterQuestions.Where(x=>x.DifficultyLevelId == 1000001).Sum(x => x.TimeToComplete) / chapterQuestions.Count;
                            report.AverageTimeSpentOnMediumQuestions = chapterQuestions.Where(x => x.DifficultyLevelId == 1000002).Sum(x => x.TimeToComplete) / chapterQuestions.Count;
                            report.AverageTimeSpentOnDifficultQuestions = chapterQuestions.Where(x => x.DifficultyLevelId == 1000003).Sum(x => x.TimeToComplete) / chapterQuestions.Count;
                        }
                        else
                        {
                            report.AverageTimeSpentOnEachQuestion = 0;
                            report.AverageTimeSpentOnEasyQuestions = 0;
                            report.AverageTimeSpentOnMediumQuestions = 0;
                            report.AverageTimeSpentOnDifficultQuestions = 0;
                        }
                        report.Reports = new List<ItemWiseTimeAnalysisReportDto>();
                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        foreach (var chapter in chapters)
                        {
                            ItemWiseTimeAnalysisReportDto itemWiseReportDto = new ItemWiseTimeAnalysisReportDto();
                            itemWiseReportDto.Id = chapter.Id;
                            itemWiseReportDto.Name = chapter.Name;

                            var levels = dbContext.DifficultyLevels.ToList();
                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelTimeAnalysisReportDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelTimeAnalysisReportDto item = new AssessmentLevelTimeAnalysisReportDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (chapterQuestions != null && chapterQuestions.Count > 0)
                                    {
                                        var sessionIds = assessmentSessions.Where(x => x.ChapterId == chapter.Id && x.EndTime != null).ToList().Select(x => x.Id);
                                        var questions = chapterQuestions.Where(x => sessionIds.Contains(x.AssessmentSessionId) && x.DifficultyLevelId == level.Id).ToList();
                                        if (questions != null && questions.Count > 0)
                                            item.AverageTime = questions.Sum(x => x.TimeToComplete) / questions.Count;
                                        else
                                            item.AverageTime = 0;
                                    }
                                    else
                                    {
                                        item.AverageTime = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }

        public async Task<ChapterWiseTimeAnalysisDto> TimeSpentAnalysisByChapter(long chapterId)
        {
            var currentUser = authService.GetCurrentUser();
            ChapterWiseTimeAnalysisDto report = new ChapterWiseTimeAnalysisDto();
            try
            {
                if (chapterId > 0 && dbContext.Chapters.Any(x => x.Id == chapterId))
                {
                    var chapter = dbContext.VwChapters.First(x => x.Id == chapterId);
                    report.Title = "TWT";
                    report.Course = chapter.Course;
                    report.Chapter = chapter.Name;
                    report.Class = chapter.Class;
                    report.Subject = chapter.Subject;

                    var topics = dbContext.VwTopics.Any(x => x.ChapterId == chapterId) ? dbContext.VwTopics.Where(x => x.ChapterId == chapterId).ToList() : null;
                    if (topics != null && topics.Count > 0)
                    {
                        var topicIds = topics.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.TopicId != null && topicIds.Contains(session.TopicId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var topicQuestions = questionsQuery.ToList();

                        if (topicQuestions != null && topicQuestions.Count > 0)
                        {
                            report.AverageTimeSpentOnEachQuestion = topicQuestions.Sum(x => x.TimeToComplete) / topicQuestions.Count;
                            report.AverageTimeSpentOnEasyQuestions = topicQuestions.Where(x => x.DifficultyLevelId == 1000001).Sum(x => x.TimeToComplete) / topicQuestions.Count;
                            report.AverageTimeSpentOnMediumQuestions = topicQuestions.Where(x => x.DifficultyLevelId == 1000002).Sum(x => x.TimeToComplete) / topicQuestions.Count;
                            report.AverageTimeSpentOnDifficultQuestions = topicQuestions.Where(x => x.DifficultyLevelId == 1000003).Sum(x => x.TimeToComplete) / topicQuestions.Count;
                        }
                        else
                        {
                            report.AverageTimeSpentOnEachQuestion = 0;
                            report.AverageTimeSpentOnEasyQuestions = 0;
                            report.AverageTimeSpentOnMediumQuestions = 0;
                            report.AverageTimeSpentOnDifficultQuestions = 0;
                        }
                        report.Reports = new List<ItemWiseTimeAnalysisReportDto>();
                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        foreach (var topic in topics)
                        {
                            ItemWiseTimeAnalysisReportDto itemWiseReportDto = new ItemWiseTimeAnalysisReportDto();
                            itemWiseReportDto.Id = topic.Id;
                            itemWiseReportDto.Name = topic.Name;

                            var levels = dbContext.DifficultyLevels.ToList();
                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelTimeAnalysisReportDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelTimeAnalysisReportDto item = new AssessmentLevelTimeAnalysisReportDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (topicQuestions != null && topicQuestions.Count > 0)
                                    {
                                        var sessionIds = assessmentSessions.Where(x => x.TopicId == topic.Id && x.EndTime != null).ToList().Select(x => x.Id);
                                        var questions = topicQuestions.Where(x => sessionIds.Contains(x.AssessmentSessionId) && x.DifficultyLevelId == level.Id).ToList();
                                        if (questions != null && questions.Count > 0)
                                            item.AverageTime = questions.Sum(x => x.TimeToComplete) / questions.Count;
                                        else
                                            item.AverageTime = 0;
                                    }
                                    else
                                    {
                                        item.AverageTime = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }

        public async Task<TopicWiseTimeAnalysisDto> TimeSpentAnalysisByTopic(long topicId)
        {
            var currentUser = authService.GetCurrentUser();
            TopicWiseTimeAnalysisDto report = new TopicWiseTimeAnalysisDto();
            try
            {
                if (topicId > 0 && dbContext.Topics.Any(x => x.Id == topicId))
                {
                    var topic = dbContext.VwTopics.First(x => x.Id == topicId);
                    report.Title = "TWT";
                    report.Course = topic.Course;
                    report.Chapter = topic.Name;
                    report.Class = topic.Class;
                    report.Subject = topic.Subject;

                    var subTopics = dbContext.VwSubTopics.Any(x => x.TopicId == topicId) ? dbContext.VwSubTopics.Where(x => x.TopicId == topicId).ToList() : null;
                    if (subTopics != null && subTopics.Count > 0)
                    {
                        var subTopicIds = subTopics.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.SubTopicId != null && subTopicIds.Contains(session.SubTopicId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var subTopicQuestions = questionsQuery.ToList();

                        if (subTopicQuestions != null && subTopicQuestions.Count > 0)
                        {
                            report.AverageTimeSpentOnEachQuestion = subTopicQuestions.Sum(x => x.TimeToComplete) / subTopicQuestions.Count;
                            report.AverageTimeSpentOnEasyQuestions = subTopicQuestions.Where(x => x.DifficultyLevelId == 1000001).Sum(x => x.TimeToComplete) / subTopicQuestions.Count;
                            report.AverageTimeSpentOnMediumQuestions = subTopicQuestions.Where(x => x.DifficultyLevelId == 1000002).Sum(x => x.TimeToComplete) / subTopicQuestions.Count;
                            report.AverageTimeSpentOnDifficultQuestions = subTopicQuestions.Where(x => x.DifficultyLevelId == 1000003).Sum(x => x.TimeToComplete) / subTopicQuestions.Count;
                        }
                        else
                        {
                            report.AverageTimeSpentOnEachQuestion = 0;
                            report.AverageTimeSpentOnEasyQuestions = 0;
                            report.AverageTimeSpentOnMediumQuestions = 0;
                            report.AverageTimeSpentOnDifficultQuestions = 0;
                        }
                        report.Reports = new List<ItemWiseTimeAnalysisReportDto>();
                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        foreach (var subTopic in subTopics)
                        {
                            ItemWiseTimeAnalysisReportDto itemWiseReportDto = new ItemWiseTimeAnalysisReportDto();
                            itemWiseReportDto.Id = subTopic.Id;
                            itemWiseReportDto.Name = subTopic.Name;

                            var levels = dbContext.DifficultyLevels.ToList();
                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelTimeAnalysisReportDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelTimeAnalysisReportDto item = new AssessmentLevelTimeAnalysisReportDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (subTopicQuestions != null && subTopicQuestions.Count > 0)
                                    {
                                        var sessionIds = assessmentSessions.Where(x => x.SubTopicId == subTopic.Id && x.EndTime != null).ToList().Select(x => x.Id);
                                        var questions = subTopicQuestions.Where(x => sessionIds.Contains(x.AssessmentSessionId) && x.DifficultyLevelId == level.Id).ToList();
                                        if (questions != null && questions.Count > 0)
                                            item.AverageTime = questions.Sum(x => x.TimeToComplete) / questions.Count;
                                        else
                                            item.AverageTime = 0;
                                    }
                                    else
                                    {
                                        item.AverageTime = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }

        public async Task<SubjectWiseDifficultyLevelAnalysisDto> DifficultyLevelAnalysisBySubject(long subjectId)
        {
            var currentUser = authService.GetCurrentUser();
            SubjectWiseDifficultyLevelAnalysisDto report = new SubjectWiseDifficultyLevelAnalysisDto();
            try
            {
                if (subjectId > 0 && dbContext.Subjects.Any(x => x.Id == subjectId))
                {
                    var subject = dbContext.VwSubjects.First(x => x.Id == subjectId);
                    report.Title = "CWT";
                    report.Course = subject.Course;
                    report.Subject = subject.Name;
                    report.Class = subject.Class;

                    var chapters = dbContext.VwChapters.Any(x => x.SubjectId == subjectId) ? dbContext.VwChapters.Where(x => x.SubjectId == subjectId).ToList() : null;

                    if (chapters != null && chapters.Count > 0)
                    {
                        var chapterIds = chapters.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.ChapterId != null && chapterIds.Contains(session.ChapterId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var chapterQuestions =  questionsQuery.ToList();


                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        var levels = dbContext.DifficultyLevels.ToList();

                        if (levels != null && levels.Count > 0)
                        {
                            report.TotalQuestionAnalysis = new List<AssessmentLevelDifficultyLevelAnalysisDto>();
                            foreach (var level in levels)
                            {
                                AssessmentLevelDifficultyLevelAnalysisDto item = new AssessmentLevelDifficultyLevelAnalysisDto();
                                item.Id = level.Id;
                                item.Name = level.Name;
                                if (chapterQuestions != null && chapterQuestions.Count > 0)
                                {
                                    var questions = chapterQuestions.Where(x => x.DifficultyLevelId == level.Id).ToList();

                                    if (questions != null && questions.Count > 0)
                                    {
                                        item.TotalQuestions = questions.Count;
                                        item.TotalQuestionsAttempted = questions.Count(x => x.Result != null);
                                        item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                        item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                    }
                                    else
                                    {
                                        item.TotalQuestions = 0;
                                        item.TotalQuestionsAttempted = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                }
                                else
                                {
                                    item.TotalQuestions = 0;
                                    item.TotalQuestionsAttempted = 0;
                                    item.TotalCorrect = 0;
                                    item.TotalWrong = 0;
                                }
                                report.TotalQuestionAnalysis.Add(item);
                            }
                        }
                       
                        report.Reports = new List<ItemWiseDifficultyLevelAnalysisDto>();
                        foreach (var chapter in chapters)
                        {
                            ItemWiseDifficultyLevelAnalysisDto itemWiseReportDto = new ItemWiseDifficultyLevelAnalysisDto();
                            itemWiseReportDto.Id = chapter.Id;
                            itemWiseReportDto.Name = chapter.Name;

                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelDifficultyLevelAnalysisDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelDifficultyLevelAnalysisDto item = new AssessmentLevelDifficultyLevelAnalysisDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (chapterQuestions != null && chapterQuestions.Count > 0 && assessmentSessions.Any(x => x.ChapterId == chapter.Id && x.EndTime != null))
                                    {
                                        var sessionIds = assessmentSessions.Where(x => x.ChapterId == chapter.Id && x.EndTime != null).ToList().Select(x => x.Id);
                                        var questions = chapterQuestions.Where(x => sessionIds.Contains(x.AssessmentSessionId) && x.DifficultyLevelId == level.Id).ToList();

                                        if (questions != null && questions.Count > 0)
                                        {
                                            item.TotalQuestions = questions.Count;
                                            item.TotalQuestionsAttempted = questions.Count(x => x.Result != null);
                                            item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                            item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                        }
                                        else
                                        {
                                            item.TotalQuestions = 0;
                                            item.TotalQuestionsAttempted = 0;
                                            item.TotalCorrect = 0;
                                            item.TotalWrong = 0;
                                        }
                                    }
                                    else
                                    {
                                        item.TotalQuestions = 0;
                                        item.TotalQuestionsAttempted = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }

        public async Task<ChapterWiseDifficultyLevelAnalysisDto> DifficultyLevelAnalysisByChapter(long chapterId)
        {
            var currentUser = authService.GetCurrentUser();
            ChapterWiseDifficultyLevelAnalysisDto report = new ChapterWiseDifficultyLevelAnalysisDto();
            try
            {
                if (chapterId > 0 && dbContext.Chapters.Any(x => x.Id == chapterId))
                {
                    var chapter = dbContext.VwChapters.First(x => x.Id == chapterId);
                    report.Title = "TWT";
                    report.Course = chapter.Course;
                    report.Subject = chapter.Subject;
                    report.Class = chapter.Class;
                    report.Chapter = chapter.Name;

                    var topics = dbContext.VwTopics.Any(x => x.ChapterId == chapterId) ? dbContext.VwTopics.Where(x => x.ChapterId == chapterId).ToList() : null;

                    if (topics != null && topics.Count > 0)
                    {
                        var topicIds = topics.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.TopicId != null && topicIds.Contains(session.TopicId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var topicQuestions = questionsQuery.ToList();


                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        var levels = dbContext.AssessmentLevels.ToList();

                        if (levels != null && levels.Count > 0)
                        {
                            report.TotalQuestionAnalysis = new List<AssessmentLevelDifficultyLevelAnalysisDto>();
                            foreach (var level in levels)
                            {
                                AssessmentLevelDifficultyLevelAnalysisDto item = new AssessmentLevelDifficultyLevelAnalysisDto();
                                item.Id = level.Id;
                                item.Name = level.Name;
                                if (topicQuestions != null && topicQuestions.Count > 0)
                                {
                                    var questions = topicQuestions.Where(x => x.DifficultyLevelId == level.Id).ToList();

                                    if (questions != null && questions.Count > 0)
                                    {
                                        item.TotalQuestions = questions.Count;
                                        item.TotalQuestionsAttempted = questions.Count(x => x.Result != null);
                                        item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                        item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                    }
                                    else
                                    {
                                        item.TotalQuestions = 0;
                                        item.TotalQuestionsAttempted = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                }
                                else
                                {
                                    item.TotalQuestions = 0;
                                    item.TotalQuestionsAttempted = 0;
                                    item.TotalCorrect = 0;
                                    item.TotalWrong = 0;
                                }
                                report.TotalQuestionAnalysis.Add(item);
                            }
                        }

                        report.Reports = new List<ItemWiseDifficultyLevelAnalysisDto>();
                        foreach (var topic in topics)
                        {
                            ItemWiseDifficultyLevelAnalysisDto itemWiseReportDto = new ItemWiseDifficultyLevelAnalysisDto();
                            itemWiseReportDto.Id = topic.Id;
                            itemWiseReportDto.Name = topic.Name;

                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelDifficultyLevelAnalysisDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelDifficultyLevelAnalysisDto item = new AssessmentLevelDifficultyLevelAnalysisDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (topicQuestions != null && topicQuestions.Count > 0 && assessmentSessions.Any(x => x.TopicId == topic.Id && x.EndTime != null))
                                    {
                                        var sessionIds = assessmentSessions.Where(x => x.TopicId == topic.Id && x.EndTime != null).ToList().Select(x => x.Id);
                                        var questions = topicQuestions.Where(x => sessionIds.Contains(x.AssessmentSessionId) && x.DifficultyLevelId == level.Id).ToList();

                                        if (questions != null && questions.Count > 0)
                                        {
                                            item.TotalQuestions = questions.Count;
                                            item.TotalQuestionsAttempted = questions.Count(x => x.Result != null);
                                            item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                            item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                        }
                                        else
                                        {
                                            item.TotalQuestions = 0;
                                            item.TotalQuestionsAttempted = 0;
                                            item.TotalCorrect = 0;
                                            item.TotalWrong = 0;
                                        }
                                    }
                                    else
                                    {
                                        item.TotalQuestions = 0;
                                        item.TotalQuestionsAttempted = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }

        public async Task<TopicWiseDifficultyLevelAnalysisDto> DifficultyLevelAnalysisByTopic(long topicId)
        {
            var currentUser = authService.GetCurrentUser();
            TopicWiseDifficultyLevelAnalysisDto report = new TopicWiseDifficultyLevelAnalysisDto();
            try
            {
                if (topicId > 0 && dbContext.Topics.Any(x => x.Id == topicId))
                {
                    var topic = dbContext.VwTopics.First(x => x.Id == topicId);
                    report.Title = "CWT";
                    report.Course = topic.Course;
                    report.Subject = topic.Subject;
                    report.Chapter = topic.Chapter;
                    report.Class = topic.Class;
                    report.Topic = topic.Name;

                    var subTopics = dbContext.VwSubTopics.Any(x => x.TopicId == topicId) ? dbContext.VwSubTopics.Where(x => x.TopicId == topicId).ToList() : null;

                    if (subTopics != null && subTopics.Count > 0)
                    {
                        var subTopicIds = subTopics.Select(x => x.Id).ToList();
                        var questionsQuery = from question in dbContext.AssessmentSessionQuestions
                                             join session in dbContext.AssessmentSessions on question.AssessmentSessionId equals session.Id
                                             where session.SubTopicId != null && subTopicIds.Contains(session.SubTopicId.Value) && session.StudentId == currentUser.UserId
                                             select new AssessmentSessionQuestion
                                             {
                                                 Id = question.Id,
                                                 AssessmentSessionId = question.AssessmentSessionId,
                                                 TimeToComplete = question.TimeToComplete,
                                                 DifficultyLevelId = question.DifficultyLevelId,
                                                 Result = question.Result
                                             };
                        var subTopicQuestions = questionsQuery.ToList();


                        var assessmentSessions = dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId).ToList();
                        var levels = dbContext.AssessmentLevels.ToList();

                        if (levels != null && levels.Count > 0)
                        {
                            report.TotalQuestionAnalysis = new List<AssessmentLevelDifficultyLevelAnalysisDto>();
                            foreach (var level in levels)
                            {
                                AssessmentLevelDifficultyLevelAnalysisDto item = new AssessmentLevelDifficultyLevelAnalysisDto();
                                item.Id = level.Id;
                                item.Name = level.Name;
                                if (subTopicQuestions != null && subTopicQuestions.Count > 0)
                                {
                                    var questions = subTopicQuestions.Where(x => x.DifficultyLevelId == level.Id).ToList();

                                    if (questions != null && questions.Count > 0)
                                    {
                                        item.TotalQuestions = questions.Count;
                                        item.TotalQuestionsAttempted = questions.Count(x => x.Result != null);
                                        item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                        item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                    }
                                    else
                                    {
                                        item.TotalQuestions = 0;
                                        item.TotalQuestionsAttempted = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                }
                                else
                                {
                                    item.TotalQuestions = 0;
                                    item.TotalQuestionsAttempted = 0;
                                    item.TotalCorrect = 0;
                                    item.TotalWrong = 0;
                                }
                                report.TotalQuestionAnalysis.Add(item);
                            }
                        }

                        report.Reports = new List<ItemWiseDifficultyLevelAnalysisDto>();
                        foreach (var subTopic in subTopics)
                        {
                            ItemWiseDifficultyLevelAnalysisDto itemWiseReportDto = new ItemWiseDifficultyLevelAnalysisDto();
                            itemWiseReportDto.Id = subTopic.Id;
                            itemWiseReportDto.Name = subTopic.Name;

                            if (levels != null && levels.Count > 0)
                            {
                                itemWiseReportDto.LevelReports = new List<AssessmentLevelDifficultyLevelAnalysisDto>();
                                foreach (var level in levels)
                                {
                                    AssessmentLevelDifficultyLevelAnalysisDto item = new AssessmentLevelDifficultyLevelAnalysisDto();
                                    item.Id = level.Id;
                                    item.Name = level.Name;
                                    if (subTopicQuestions != null && subTopicQuestions.Count > 0 && assessmentSessions.Any(x => x.SubTopicId == subTopic.Id && x.EndTime != null))
                                    {
                                        var sessionIds = assessmentSessions.Where(x => x.SubTopicId == subTopic.Id && x.EndTime != null).ToList().Select(x => x.Id);
                                        var questions = subTopicQuestions.Where(x => sessionIds.Contains(x.AssessmentSessionId) && x.DifficultyLevelId == level.Id).ToList();

                                        if (questions != null && questions.Count > 0)
                                        {
                                            item.TotalQuestions = questions.Count;
                                            item.TotalQuestionsAttempted = questions.Count(x => x.Result != null);
                                            item.TotalCorrect = questions.Count(x => x.Result != null && x.Result == true);
                                            item.TotalWrong = questions.Count(x => x.Result != null && x.Result == false);
                                        }
                                        else
                                        {
                                            item.TotalQuestions = 0;
                                            item.TotalQuestionsAttempted = 0;
                                            item.TotalCorrect = 0;
                                            item.TotalWrong = 0;
                                        }
                                    }
                                    else
                                    {
                                        item.TotalQuestions = 0;
                                        item.TotalQuestionsAttempted = 0;
                                        item.TotalCorrect = 0;
                                        item.TotalWrong = 0;
                                    }
                                    itemWiseReportDto.LevelReports.Add(item);
                                }
                            }

                            report.Reports.Add(itemWiseReportDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return report;
        }








        //private List<ItemWiseAnalysisDto> GetDifficultyLevelAnalysis(List<AssessmentSessionQuestion> questions)
        //{
        //    List<ItemWiseAnalysisDto> difficultyLevelAnalysis = new List<ItemWiseAnalysisDto>();
        //    var difficultyLevels = dbContext.DifficultyLevels.ToList();

        //    var groupedDifficultyLevelQuestions = questions.GroupBy(q => q.DifficultyLevelId)
        //                            .Select(g => new
        //                            {
        //                                Level = g.Key,
        //                                Count = g.Count()
        //                            });

        //    if (groupedDifficultyLevelQuestions.Any())
        //    {
        //        difficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
        //        {
        //            Name = "Easy",
        //            QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Easy").Id)?.Count ?? 0

        //        });
        //        difficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
        //        {
        //            Name = "Medium",
        //            QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Medium").Id)?.Count ?? 0
        //        });

        //        difficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
        //        {
        //            Name = "Difficult",
        //            QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Difficult").Id)?.Count ?? 0
        //        });
        //    }

        //    return difficultyLevelAnalysis;
        //}

        //public async Task<List<SubjectWeightageDto>> GetSubjectsReport()
        //{
        //    var currentUser = authService.GetCurrentUser();
        //    List<SubjectWeightageDto> items = new List<SubjectWeightageDto>();

        //    var student = dbContext.Students.Where(x => x.Id == currentUser.UserId).FirstOrDefault();

        //    var course = dbContext.Courses.Where(x => x.Id == student.CourseId).FirstOrDefault();

        //    var classObject = dbContext.Classes.Where(x => x.Id == student.ClassId).FirstOrDefault();

        //    var subjects = dbContext.Subjects.Where(x => x.ClassId == classObject.Id && x.IsActive).ToList();

        //    if(subjects.Any())
        //    {
        //        foreach(var subject in subjects)
        //        {
        //            int totalQuestions = commonService.GetTotalQUestionsCountBySubjectId(subject.Id);
        //            int correctAnswers = commonService.GetCorrectAnswersCountBySubjectId(subject.Id);

        //            double percentage = ((double)totalQuestions / 100) * correctAnswers;
        //            items.Add(new SubjectWeightageDto() { Name = subject.Name, Percentage = Math.Round(percentage, 2), ColorCode = subject.ColorCode });
        //        }
        //    }

        //    return items;
        //}

        //public async Task<StudentReportDto> GetStudentReport()
        //{
        //    var currentUser = authService.GetCurrentUser();
        //    StudentReportDto report = new StudentReportDto();
        //    var student = dbContext.Students.Where(x => x.Id == currentUser.UserId).FirstOrDefault();

        //    if (student != null)
        //    {
        //        List<AssessmentSessionQuestion> questions = GetQuestionsByStudentId(student.Id);

        //        if (questions != null && questions.Count > 0)
        //        {
        //            Course course = GetCourse(student);
        //            var subjects = GetSubjects(questions);
        //            var chapters = GetChapters(questions);
        //            var topics = GetTopics(questions);

        //            report.StudentName = student.Name;
        //            report.CourseName = course.Name;
        //            report.TotalAttempted = questions.Count;
        //            report.CorrectAnswers = questions.Where(x => x.Result != null && x.Result.Value).Count();
        //            report.WrongAnswers = questions.Where(x => x.Result != null && !x.Result.Value).Count();

        //            report.SubjectsWeightage = GetSubjectWeightage(subjects);

        //            if (subjects != null && subjects.Any())
        //            {
        //                report.PracticedFromSubjects = new List<ListItemDto>();
        //                report.PracticedFromSubjects.AddRange(subjects.Select(y => new ListItemDto()
        //                {
        //                    Id = y.Id,
        //                    Name = y.Name
        //                }));
        //            }

        //            if (chapters != null && chapters.Any())
        //            {
        //                report.PracticedFromChapters = new List<ListItemDto>();
        //                report.PracticedFromChapters.AddRange(chapters.Select(y => new ListItemDto()
        //                {
        //                    Id = y.Id,
        //                    Name = y.Name
        //                }));
        //            }

        //            if (topics != null && topics.Any())
        //            {
        //                report.PracticedFromTopics = new List<ListItemDto>();
        //                report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
        //                {
        //                    Id = y.Id,
        //                    Name = y.Name
        //                }));
        //            }
        //            report.CorrectAnswersAnalysis = GetSubjectAnalysis(questions.Where(x => x.Result != null && x.Result.Value).ToList(), chapters, subjects, topics);
        //            report.WrongAnswersAnalysis = GetSubjectAnalysis(questions.Where(x => x.Result != null && !x.Result.Value).ToList(), chapters, subjects, topics);

        //        }
        //    }
        //    return report;
        //}

        //private List<ItemWiseWeightage> GetSubjectWeightage(List<Subject> subjects)
        //{
        //    List<ItemWiseWeightage> items = new List<ItemWiseWeightage>();
        //    foreach (var subject in subjects)
        //    {
        //        int totalQuestions = commonService.GetTotalQUestionsCountBySubjectId(subject.Id);
        //        int correctAnswers = commonService.GetCorrectAnswersCountBySubjectId(subject.Id);

        //        double percentage = ((double)totalQuestions / 100) * correctAnswers;
        //        items.Add(new ItemWiseWeightage() { Name = subject.Name, percentage = Math.Round(percentage, 2) });
        //    }
        //    return items;
        //}
        //public async Task<SubjectReportDto> GetSubjectReport(long id)
        //{
        //    var currentUser = authService.GetCurrentUser();
        //    SubjectReportDto report = new SubjectReportDto();

        //    var questions = GetQuestionsBySubjectId(id);

        //    if (questions != null && questions.Count > 0)
        //    {
        //        report.SubjectId = id;
        //        report.TotalAttempted = questions.Count;
        //        report.CorrectAnswers = questions.Where(x => x.Result != null && x.Result.Value).Count();
        //        report.WrongAnswers = questions.Where(x => x.Result != null && !x.Result.Value).Count();

        //        var chapters = GetChapters(questions);
        //        if (chapters != null && chapters.Any())
        //        {
        //            report.PracticedFromChapters = new List<ListItemDto>();
        //            report.PracticedFromChapters.AddRange(chapters.Select(y => new ListItemDto()
        //            {
        //                Id = y.Id,
        //                Name = y.Name
        //            }));
        //        }

        //        var topics = GetTopics(questions);

        //        if (topics != null && topics.Any())
        //        {
        //            report.PracticedFromTopics = new List<ListItemDto>();
        //            report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
        //            {
        //                Id = y.Id,
        //                Name = y.Name
        //            }));
        //        }

        //        report.CorrectAnswersAnalysis = GetChapterAnalysis(questions.Where(x => x.Result != null && x.Result.Value).ToList(), chapters, topics);
        //        report.WrongAnswersAnalysis = GetChapterAnalysis(questions.Where(x => x.Result != null && !x.Result.Value).ToList(), chapters, topics);
        //    }

        //    return report;
        //}

        //public async Task<ChapterReportDto> GetChapterReport(long id)
        //{
        //    var currentUser = authService.GetCurrentUser();
        //    ChapterReportDto report = new ChapterReportDto();
        //    var chapter = dbContext.Chapters.Where(x => x.Id == id).FirstOrDefault();

        //    if (chapter != null)
        //    {
        //        var allQuestions = dbContext.AssessmentSessionQuestions.Where(x => x.ChapterId == id && x.Result != null).ToList();
        //        var questions = RemoveDuplicateQuestions(allQuestions);
        //        report.ChapterId = chapter.Id;
        //        report.TotalAttempted = questions.Count;
        //        report.CorrectAnswers = questions.Where(x => x.Result.Value).Count();
        //        report.WrongAnswers = questions.Where(x => !x.Result.Value).Count();

        //        var topics = GetTopics(questions);
        //        if (topics != null && topics.Any())
        //        {
        //            report.PracticedFromTopics = new List<ListItemDto>();
        //            report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
        //            {
        //                Id = y.Id,
        //                Name = y.Name
        //            }));
        //        }

        //        report.CorrectAnswersAnalysis = GetTopicAnalysis(questions.Where(x => x.Result.Value).ToList(), topics);
        //        report.WrongAnswersAnalysis = GetTopicAnalysis(questions.Where(x => !x.Result.Value).ToList(), topics);
        //    }

        //    return report;
        //}

        //private SubjectAnalysisDto GetSubjectAnalysis(List<AssessmentSessionQuestion> questions, List<Chapter> chapters, List<Subject> subjects, List<Topic> topics)
        //{
        //    SubjectAnalysisDto answersAnalysis = new SubjectAnalysisDto();

        //    answersAnalysis.DifficultyLevelAnalysis = GetDifficultyLevelAnalysis(questions);

        //    List<Chapter> chapterIds = new List<Chapter>();
        //    var groupedChaperQuestions = questions.GroupBy(q => q.ChapterId)
        //                            .Select(g => new
        //                            {
        //                                Level = g.Key,
        //                                Count = g.Count()
        //                            });

        //    if (groupedChaperQuestions.Any())
        //    {
        //        answersAnalysis.ChapterWiseAnalysis = new List<ItemWiseAnalysisDto>();
        //        answersAnalysis.SubjectWiseAnalysis = new List<ItemWiseAnalysisDto>();
        //        foreach (var group in groupedChaperQuestions)
        //        {
        //            if (group.Level != null)
        //            {
        //                var chaptersData = chapters.First(x => x.Id == group.Level.Value);
        //                chapterIds.Add(chaptersData);
        //                ItemWiseAnalysisDto newItem = new ItemWiseAnalysisDto()
        //                {
        //                    Name = chaptersData.Name,
        //                    QuestionsCount = groupedChaperQuestions.FirstOrDefault(g => g.Level.Value == chaptersData.Id)?.Count ?? 0
        //                };
        //                answersAnalysis.ChapterWiseAnalysis.Add(newItem);

        //                var subject = subjects.Where(x => x.Id == chaptersData.SubjectId).FirstOrDefault();

        //                if(!answersAnalysis.SubjectWiseAnalysis.Any(x => x.Name == subject.Name))
        //                {
        //                    answersAnalysis.SubjectWiseAnalysis.Add(new ItemWiseAnalysisDto()
        //                    {
        //                        Name = subject.Name,
        //                        QuestionsCount = newItem.QuestionsCount
        //                    });
        //                }
        //                else
        //                {
        //                    var item = answersAnalysis.SubjectWiseAnalysis.Where(x => x.Name == subject.Name).First();
        //                    item.QuestionsCount += newItem.QuestionsCount;
        //                }
        //            }
        //        }
        //    }

        //    var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
        //                            .Select(g => new
        //                            {
        //                                Level = g.Key,
        //                                Count = g.Count()
        //                            });

        //    if (groupedTopicQuestions.Any())
        //    {
        //        answersAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
        //        foreach (var group in groupedTopicQuestions)
        //        {
        //            if (group.Level != null)
        //            {
        //                var topicsData = topics.First(x => x.Id == group.Level.Value);
        //                answersAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
        //                {
        //                    Name = topicsData.Name,
        //                    QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
        //                });
        //            }
        //        }
        //    }

        //    return answersAnalysis;
        //}

        //private ChapterAnalysisDto GetChapterAnalysis(List<AssessmentSessionQuestion> questions, List<Chapter> chapters, List<Topic> topics)
        //{
        //    ChapterAnalysisDto answersAnalysis = new ChapterAnalysisDto();

        //    answersAnalysis.DifficultyLevelAnalysis = GetDifficultyLevelAnalysis(questions);

        //    var groupedChaperQuestions = questions.GroupBy(q => q.ChapterId)
        //                            .Select(g => new
        //                            {
        //                                Level = g.Key,
        //                                Count = g.Count()
        //                            });

        //    if (groupedChaperQuestions.Any())
        //    {
        //        answersAnalysis.ChapterWiseAnalysis = new List<ItemWiseAnalysisDto>();
        //        foreach (var group in groupedChaperQuestions)
        //        {
        //            if (group.Level != null)
        //            {
        //                var chaptersData = chapters.First(x => x.Id == group.Level.Value);
        //                answersAnalysis.ChapterWiseAnalysis.Add(new ItemWiseAnalysisDto()
        //                {
        //                    Name = chaptersData.Name,
        //                    QuestionsCount = groupedChaperQuestions.FirstOrDefault(g => g.Level.Value == chaptersData.Id)?.Count ?? 0
        //                });
        //            }
        //        }
        //    }
        //    var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
        //                            .Select(g => new
        //                            {
        //                                Level = g.Key,
        //                                Count = g.Count()
        //                            });

        //    if (groupedTopicQuestions.Any())
        //    {
        //        answersAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
        //        foreach (var group in groupedTopicQuestions)
        //        {
        //            if (group.Level != null)
        //            {
        //                var topicsData = topics.First(x => x.Id == group.Level.Value);
        //                answersAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
        //                {
        //                    Name = topicsData.Name,
        //                    QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
        //                });
        //            }
        //        }
        //    }

        //    return answersAnalysis;
        //}

        //private TopicAnalysisDto GetTopicAnalysis(List<AssessmentSessionQuestion> questions, List<Topic> topics)
        //{
        //    TopicAnalysisDto topicAnalysis = new TopicAnalysisDto();
        //    topicAnalysis.DifficultyLevelAnalysis = GetDifficultyLevelAnalysis(questions);

        //    var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
        //                            .Select(g => new
        //                            {
        //                                Level = g.Key,
        //                                Count = g.Count()
        //                            });

        //    if (groupedTopicQuestions.Any())
        //    {
        //        topicAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
        //        foreach (var group in groupedTopicQuestions)
        //        {
        //            if (group.Level != null)
        //            {
        //                var topicsData = topics.First(x => x.Id == group.Level.Value);
        //                topicAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
        //                {
        //                    Name = topicsData.Name,
        //                    QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
        //                });
        //            }
        //        }
        //    }

        //    return topicAnalysis;
        //}

        //private Course GetCourse(Student student)
        //{
        //    return dbContext.Courses.First(x => x.Id == student.CourseId);
        //}
        //private List<AssessmentSessionQuestion> GetQuestionsByStudentId(long studentId)
        //{
        //    List<AssessmentSessionQuestion> questions = new List<AssessmentSessionQuestion>();

        //    if (dbContext.AssessmentSessions.Any(item => item.StudentId == studentId))
        //    {
        //        var sessions = dbContext.AssessmentSessions.Where(item => item.StudentId == studentId).ToList();

        //        foreach (var session in sessions)
        //        {
        //            questions.AddRange(dbContext.AssessmentSessionQuestions.Where(x => x.AssessmentSessionId == session.Id && x.Result != null).ToList());
        //        }
        //    }
        //    return RemoveDuplicateQuestions(questions);
        //}
        //private List<AssessmentSessionQuestion> GetQuestionsBySubjectId(long subjectId)
        //{
        //    List<AssessmentSessionQuestion> questions = new List<AssessmentSessionQuestion>();
        //    var subject = dbContext.Subjects.FirstOrDefault(x => x.Id == subjectId);
        //    if (subject != null)
        //    {
        //        var chapters = dbContext.Chapters.Where(item => item.SubjectId == subjectId).ToList();

        //        foreach (var chapter in chapters)
        //        {
        //            questions.AddRange(dbContext.AssessmentSessionQuestions.Where(x => x.ChapterId == chapter.Id && x.Result != null).ToList());
        //        }
        //    }
        //    return RemoveDuplicateQuestions(questions);
        //}
        //public List<AssessmentSessionQuestion> RemoveDuplicateQuestions(List<AssessmentSessionQuestion> objects)
        //{
        //    var distinctObjects = objects
        //        .GroupBy(obj => obj.QuestionId)
        //        .Select(group => group.OrderByDescending(obj => obj.Id).First())
        //        .ToList();

        //    return distinctObjects;
        //}
        //private List<Subject> GetSubjects(List<AssessmentSessionQuestion> questions)
        //{
        //    List<Subject> subjects = new List<Subject>();

        //    if (questions != null && questions.Count > 0)
        //    {
        //        var chapterIds = questions.Select(x=>x.ChapterId).Distinct().ToList();
        //        if(chapterIds != null && chapterIds.Count > 0)
        //        {
        //            var chapters = dbContext.Chapters.Where(x => chapterIds.Contains(x.Id)).ToList();
        //            foreach (var chapter in chapters)
        //            {
        //                var subject = dbContext.Subjects.Where(x => x.Id == chapter.SubjectId).FirstOrDefault();
        //                if (subject != null && !subjects.Any(x => x.Id == subject.Id))
        //                    subjects.Add(subject);
        //            }
        //        }
        //    }
        //    return subjects;
        //}
        //private List<Chapter> GetChapters(List<AssessmentSessionQuestion> questions)
        //{
        //    List<Chapter> chapters = new List<Chapter>();

        //    if (questions != null && questions.Count > 0)
        //    {
        //        var chapterIds = questions.Select(x => x.ChapterId).Distinct().ToList();
        //        if (chapterIds != null && chapterIds.Count > 0)
        //        {
        //            chapters.AddRange(dbContext.Chapters.Where(x => chapterIds.Contains(x.Id)).ToList());
        //        }
        //    }
        //    return chapters;
        //}
        //private List<Topic> GetTopics(List<AssessmentSessionQuestion> questions)
        //{
        //    List<Topic> topics = new List<Topic>();

        //    if (questions != null && questions.Count > 0)
        //    {
        //        var topicIds = questions.Select(x => x.TopicId).Distinct().ToList();
        //        if (topicIds != null && topicIds.Count > 0)
        //        {
        //            topics.AddRange(dbContext.Topics.Where(x => topicIds.Contains(x.Id)).ToList());
        //        }
        //    }
        //    return topics;
        //}


        //public async Task<TrackYourProgressReportDto> GetTimeSpentAnalysisReport(long id)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<TrackYourProgressReportDto> GetDifficultyLevelAnalysisReport(long id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

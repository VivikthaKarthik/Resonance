using AutoMapper;
using Azure.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Linq;

namespace ResoClassAPI.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public AssessmentService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<string> InsertQuestions(List<QuestionsDto> questions, string? chapter, string? topic, string? subTopic)
        {
            string response = string.Empty;
            var currentUser = authService.GetCurrentUser();
            try
            {
                if (questions != null && questions.Count > 0)
                {
                    long chapterId = 0;
                    long topicId = 0;
                    long subtopicId = 0;
                    List<QuestionBank> questionsList = new List<QuestionBank>();

                    if (!string.IsNullOrEmpty(chapter))
                    {
                        if (dbContext.Chapters.Any(x => x.Name.ToLower() == chapter.ToLower() && x.IsActive))
                            chapterId = dbContext.Chapters.Where(x => x.Name.ToLower() == chapter.ToLower() && x.IsActive).FirstOrDefault().Id;
                        else
                            response = "Invalid Chapter";
                    }

                    if (!string.IsNullOrEmpty(topic))
                    {
                        if (dbContext.Topics.Any(x => x.Name.ToLower() == topic.ToLower() && x.IsActive))
                            topicId = dbContext.Topics.Where(x => x.Name.ToLower() == chapter.ToLower() && x.IsActive).FirstOrDefault().Id;
                        else
                            response = "Invalid Topic";
                    }

                    if (!string.IsNullOrEmpty(subTopic))
                    {
                        if (dbContext.SubTopics.Any(x => x.Name.ToLower() == subTopic.ToLower() && x.IsActive))
                            subtopicId = dbContext.SubTopics.Where(x => x.Name.ToLower() == chapter.ToLower() && x.IsActive).FirstOrDefault().Id;
                        else
                            response = "Invalid SubTopic";
                    }

                    if (topicId > 0 && chapterId > 0 && !dbContext.Topics.Any(x => x.ChapterId == chapterId && x.Id == topicId))
                    {
                        response = "Topic and Chapter are not linked";
                    }

                    if (response == string.Empty)
                    {
                        questions.ForEach(question =>
                        {
                            if (question != null && !string.IsNullOrEmpty(question.Question) && !string.IsNullOrEmpty(question.FirstAnswer)
                            && !string.IsNullOrEmpty(question.SecondAnswer) && !string.IsNullOrEmpty(question.ThirdAnswer) && !string.IsNullOrEmpty(question.FourthAnswer)
                            && !string.IsNullOrEmpty(question.CorrectAnswer) && !string.IsNullOrEmpty(question.DifficultyLevel))
                            {
                                QuestionBank questionBank = new QuestionBank();
                                questionBank.Question = question.Question;
                                questionBank.FirstAnswer = question.FirstAnswer;
                                questionBank.SecondAnswer = question.SecondAnswer;
                                questionBank.ThirdAnswer = question.ThirdAnswer;
                                questionBank.FourthAnswer = question.FourthAnswer;
                                questionBank.CorrectAnswer = question.CorrectAnswer;

                                //string difficultyLevelText = question.DifficultyLevel.Remove(question.DifficultyLevel.Length - 7, 7).Remove(0, 6);
                                var difficultyLevel = dbContext.DifficultyLevels.Where(x => x.Name.ToLower() == question.DifficultyLevel.ToLower()).FirstOrDefault();
                                questionBank.DifficultyLevelId = difficultyLevel != null ? difficultyLevel.Id : 1000001;

                                if (chapterId > 0)
                                    questionBank.ChapterId = chapterId;

                                if (topicId > 0)
                                    questionBank.TopicId = topicId;

                                if (subtopicId > 0)
                                    questionBank.SubTopicId = subtopicId;

                                questionBank.IsActive = true;
                                questionBank.CreatedBy = currentUser.Name;
                                questionBank.CreatedOn = DateTime.Now;
                                questionBank.ModifiedBy = currentUser.Name;
                                questionBank.ModifiedOn = DateTime.Now;
                                questionsList.Add(questionBank);
                            }
                        });
                    }

                    if (questionsList.Count > 0)
                    {

                        dbContext.QuestionBanks.AddRange(questionsList);
                        response = "Success";
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public async Task<QuestionResponseDto> GetQuestions(QuestionRequestDto requestDto)
        {
            QuestionResponseDto response = new QuestionResponseDto();
            try
            {
                var config = await GetAssessmentConfig();
                if (config != null)
                {
                    response.TotalQuestions = config.MaximumQuestions;
                    response.MarksPerQuestion = config.MarksPerQuestion;
                    response.HasNegativeMarking = config.HasNegativeMarking;
                    response.NegativeMarksPerQuestion = config.NegativeMarksPerQuestion != null ? config.NegativeMarksPerQuestion.Value : 0;
                    var questions = await GetRandomQuestions(requestDto, config.MaximumQuestions);

                    if (questions != null && questions.Count > 0)
                    {
                        response.AssessmentId = await CreateNewSession(questions);
                        response.Questions = questions;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        private async Task<long> CreateNewSession(List<QuestionData> questions)
        {
            var currentUser = authService.GetCurrentUser();
            long newAssessmentId = 0;
            try
            {
                if (questions == null || questions.Count == 0)
                { return newAssessmentId; }

                AssessmentSession newSession = new AssessmentSession();
                newSession.AssessmentType = "Practice";
                newSession.StudentId = currentUser.UserId;

                dbContext.AssessmentSessions.Add(newSession);
                await dbContext.SaveChangesAsync();
                newAssessmentId = newSession.Id;

                List<AssessmentSessionQuestion> assessmentSessionQuestions = new List<AssessmentSessionQuestion>();
                foreach (var question in questions)
                {
                    AssessmentSessionQuestion assessmentSessionQuestion = new AssessmentSessionQuestion();
                    assessmentSessionQuestion.AssessmentSessionId = newAssessmentId;
                    assessmentSessionQuestion.QuestionId = question.Id;
                    assessmentSessionQuestions.Add(assessmentSessionQuestion);
                }
                await dbContext.AssessmentSessionQuestions.AddRangeAsync(assessmentSessionQuestions);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            return newAssessmentId;
        }
        private async Task<List<QuestionData>> GetRandomQuestions(QuestionRequestDto requestDto, int totalQuestions)
        {
            List<QuestionData> finalQuestions = new List<QuestionData>();
            try
            {
                List<QuestionBank> questions = new List<QuestionBank>();
                int totalIdCount = 0;

                if (requestDto.ChapterIds != null && requestDto.ChapterIds.Count > 0)
                {
                    totalIdCount += requestDto.ChapterIds.Where(x => x != 0).Count();
                    questions.AddRange(await GetQuestionsByChapters(requestDto.ChapterIds));
                }

                if (requestDto.TopicIds != null && requestDto.TopicIds.Count > 0)
                {
                    totalIdCount += requestDto.TopicIds.Where(x => x != 0).Count();
                    questions.AddRange(await GetQuestionsByTopics(requestDto.TopicIds));
                }

                if (requestDto.SubTopicIds != null && requestDto.SubTopicIds.Count > 0)
                {
                    totalIdCount += requestDto.SubTopicIds.Where(x => x != 0).Count();
                    questions.AddRange(await GetQuestionsBySubTopics(requestDto.SubTopicIds));
                }

                var numQuestionsPerId = totalQuestions / totalIdCount;

                var random = new Random();

                var selectedQuestions = questions
                    .Where(q => requestDto.ChapterIds.Contains(q.ChapterId.Value) ||
                                requestDto.TopicIds.Contains(q.TopicId.Value) ||
                                requestDto.SubTopicIds.Contains(q.SubTopicId.Value))
                    .OrderBy(q => random.Next())
                    .GroupBy(q => new { q.ChapterId, q.TopicId, q.SubTopicId })
                    .SelectMany(group => group.Take(numQuestionsPerId))
                    .ToList();

                if (selectedQuestions != null && selectedQuestions.ToList().Count > 0)
                {
                    if (selectedQuestions.Count < totalQuestions)
                    {
                        var additionalQuestions = questions
                            .Where(q => !selectedQuestions.Contains(q))
                            .OrderBy(q => random.Next())
                            .Take(totalQuestions - selectedQuestions.Count)
                            .ToList();

                        selectedQuestions.AddRange(additionalQuestions);
                    }
                    foreach (var question in selectedQuestions)
                    {
                        var finalQuestion = mapper.Map<QuestionData>(question);
                        if (question.DifficultyLevelId != null)
                            finalQuestion.DifficultyLevel = dbContext.DifficultyLevels.First(x => x.Id == question.DifficultyLevelId).Name;

                        finalQuestions.Add(finalQuestion);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return finalQuestions;
        }
        private async Task<AssessmentConfiguration> GetAssessmentConfig()
        {
            var currentUser = authService.GetCurrentUser();
            AssessmentConfiguration config = new AssessmentConfiguration();
            try
            {
                var query = from student in dbContext.Students
                            where student.Id == currentUser.UserId
                            join course in dbContext.Courses on student.CourseId equals course.Id
                            select new Course
                            {
                                Id = course.Id,
                                Name = course.Name
                            };

                var result = query.FirstOrDefault();

                var courseId = result?.Id;

                if (courseId > 0)
                {
                    config = dbContext.AssessmentConfigurations.Where(c => c.CourseId == courseId && c.IsActive).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }
            return config;
        }
        private async Task<List<QuestionBank>> GetQuestionsByChapters(List<long> ids)
        {
            List<QuestionBank> response = new List<QuestionBank>();
            try
            {
                var questions = dbContext.QuestionBanks.Where(x => x.ChapterId != null && ids.Contains(x.ChapterId.Value) && x.IsActive);

                if (questions != null && questions.ToList().Count > 0)
                {
                    response = questions.ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return response;
        }
        private async Task<List<QuestionBank>> GetQuestionsByTopics(List<long> ids)
        {
            List<QuestionBank> response = new List<QuestionBank>();

            try
            {
                var questions = dbContext.QuestionBanks.Where(x => x.ChapterId != null && ids.Contains(x.TopicId.Value) && x.IsActive);

                if (questions != null && questions.ToList().Count > 0)
                {
                    response = questions.ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return response;
        }
        private async Task<List<QuestionBank>> GetQuestionsBySubTopics(List<long> ids)
        {
            List<QuestionBank> response = new List<QuestionBank>();

            try
            {
                var questions = dbContext.QuestionBanks.Where(x => x.ChapterId != null && ids.Contains(x.SubTopicId.Value) && x.IsActive);

                if (questions != null && questions.ToList().Count > 0)
                {
                    response = questions.ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        public async Task<bool> StartAssessment(long assessmentId)
        {
            try
            {
                var assessment = dbContext.AssessmentSessions.Where(x => x.Id == assessmentId).FirstOrDefault();
                if (assessment != null)
                {
                    assessment.StartTime = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<bool> EndAssessment(long assessmentId)
        {
            try
            {
                var assessment = dbContext.AssessmentSessions.Where(x => x.Id == assessmentId).FirstOrDefault();
                if (assessment != null)
                {
                    assessment.EndTime = DateTime.Now;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<bool> UpdateQuestionStatus(UpdateAssessmentStatusDto request)
        {
            try
            {
                var assessment = dbContext.AssessmentSessionQuestions.Where(x => x.AssessmentSessionId == request.AssessmentId && x.QuestionId == request.QuestionId).FirstOrDefault();
                if (assessment != null)
                {
                    assessment.Result = request.Result;

                    if (!string.IsNullOrEmpty(request.SelectedAnswer))
                        assessment.SelectedAnswer = request.SelectedAnswer;

                    if (request.ChapterId > 0)
                        assessment.ChapterId = request.ChapterId;

                    if (request.TopicId > 0)
                        assessment.TopicId = request.TopicId;

                    if (request.SubTopicId > 0)
                        assessment.SubTopicId = request.SubTopicId;

                    if (!string.IsNullOrEmpty(request.DifficultyLevel))
                        assessment.DifficultyLevelId = dbContext.DifficultyLevels.First(x => x.Name.ToLower() == request.DifficultyLevel.ToLower()).Id;
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<AssessmentConfigurationDto> GetAssessmentConfiguration()
        {
            AssessmentConfigurationDto configuration = new AssessmentConfigurationDto();
            try
            {
                var config = await GetAssessmentConfig();

                if (config != null)
                    configuration = mapper.Map<AssessmentConfigurationDto>(config);
            }
            catch (Exception ex)
            {

            }

            return configuration;
        }

        public async Task<List<AssessmentSessionDto>> GetAssessmentSessions()
        {
            var currentUser = authService.GetCurrentUser();
            List<AssessmentSessionDto> sessions = new List<AssessmentSessionDto>();
            try
            {
                var sessionsList = await Task.FromResult(dbContext.AssessmentSessions.Where(x => x.StudentId == currentUser.UserId));

                if (sessionsList != null)
                {
                    foreach (var session in sessionsList)
                        sessions.Add(mapper.Map<AssessmentSessionDto>(session));
                }
            }
            catch (Exception ex)
            {

            }
            return sessions;
        }

        public async Task<AssessmentReportDto> GetAssessmentReport(long id)
        {
            var currentUser = authService.GetCurrentUser();
            AssessmentReportDto report = new AssessmentReportDto();
            var assessmentSession = dbContext.AssessmentSessions.Where(x => x.Id == id).FirstOrDefault();

            if (assessmentSession != null)
            {
                var questions = dbContext.AssessmentSessionQuestions.Where(x => x.AssessmentSessionId == id).ToList();
                report.AssessmentId = assessmentSession.Id;
                report.PracticedOn = assessmentSession.StartTime.Value;
                report.TotalAttempted = questions.Count;
                report.CorrectAnswers = questions.Where(x => x.Result != null && x.Result.Value).Count();
                report.WrongAnswers = questions.Where(x => x.Result == null || !x.Result.Value).Count();

                report.PracticedFromChapters = new List<ListItemDto>();
                var chapterIds = questions.Where(x => x.ChapterId != null).Select(x => x.ChapterId);

                if (chapterIds.Any())
                {
                    var ids = chapterIds.ToList();
                    var chaptersList = dbContext.Chapters.Where(x => ids.Contains(x.Id)).Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    });

                    if (chaptersList.Any())
                    {
                        report.PracticedFromChapters = chaptersList.ToList();
                    }
                }

                report.PracticedFromTopics = new List<ListItemDto>();
                var topicIds = questions.Where(x => x.TopicId != null).Select(x => x.TopicId);

                if (topicIds.Any())
                {
                    var ids = topicIds.ToList();
                    var topicsList = dbContext.Topics.Where(x => ids.Contains(x.Id)).Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    });

                    if (topicsList.Any())
                    {
                        report.PracticedFromTopics = topicsList.ToList();
                    }
                }

                report.CorrectAnswersAnalysis = GetAnalysis(questions.Where(x => x.Result != null && x.Result.Value).ToList());
                report.WrongAnswersAnalysis = GetAnalysis(questions.Where(x => x.Result == null || !x.Result.Value).ToList());
            }

            return report;
        }

        private AssessmentAnalysisDto GetAnalysis(List<AssessmentSessionQuestion> questions)
        {
            var difficultyLevels = dbContext.DifficultyLevels.ToList();
            AssessmentAnalysisDto answersAnalysis = new AssessmentAnalysisDto();

            var groupedDifficultyLevelQuestions = questions.GroupBy(q => q.DifficultyLevelId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });
            if (groupedDifficultyLevelQuestions.Any())
            {
                answersAnalysis.DifficultyLevelAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedDifficultyLevelQuestions)
                {
                    if (group.Level != null)
                    {
                        var difficultylevel = difficultyLevels.First(x => x.Id == group.Level.Value);
                        answersAnalysis.DifficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = difficultylevel.Name,
                            QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultylevel.Id)?.Count ?? 0
                        });
                    }
                }
            }
            //correctAnswersAnalysis.DifficultyLevelAnalysis = new List<ItemWiseAnalysisDto>
            //{
            //    new ItemWiseAnalysisDto()
            //    {
            //        Name = "Easy",
            //        QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Easy").Id)?.Count ?? 0

            //},
            //new ItemWiseAnalysisDto()
            //{
            //    Name = "Medium",
            //    QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Medium").Id)?.Count ?? 0
            //},new ItemWiseAnalysisDto()
            //{
            //    Name = "Difficult",
            //    QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Difficult").Id)?.Count ?? 0
            //},
            //};

            var chapters = dbContext.Chapters.ToList();
            var groupedChaperQuestions = questions.GroupBy(q => q.ChapterId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedChaperQuestions.Any())
            {
                answersAnalysis.ChapterWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedChaperQuestions)
                {
                    if (group.Level != null)
                    {
                        var chaptersData = chapters.First(x => x.Id == group.Level.Value);
                        answersAnalysis.ChapterWiseAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = chaptersData.Name,
                            QuestionsCount = groupedChaperQuestions.FirstOrDefault(g => g.Level.Value == chaptersData.Id)?.Count ?? 0
                        });
                    }
                }
            }
            var topics = dbContext.Topics.ToList();
            var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedTopicQuestions.Any())
            {
                answersAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedTopicQuestions)
                {
                    if (group.Level != null)
                    {
                        var topicsData = topics.First(x => x.Id == group.Level.Value);
                        answersAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = topicsData.Name,
                            QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
                        });
                    }
                }
            }

            return answersAnalysis;
        }
    }
}

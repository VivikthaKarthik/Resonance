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
                            topicId = dbContext.Chapters.Where(x => x.Name.ToLower() == chapter.ToLower() && x.IsActive).FirstOrDefault().Id;
                        else
                            response = "Invalid Topic";
                    }

                    if (!string.IsNullOrEmpty(subTopic))
                    {
                        if (dbContext.SubTopics.Any(x => x.Name.ToLower() == subTopic.ToLower() && x.IsActive))
                            subtopicId = dbContext.Chapters.Where(x => x.Name.ToLower() == chapter.ToLower() && x.IsActive).FirstOrDefault().Id;
                        else
                            response = "Invalid SubTopic";
                    }

                    if(topicId > 0 && chapterId > 0 && !dbContext.Topics.Any(x => x.ChapterId == chapterId && x.Id == topicId))
                    {
                        response = "Topic and Chapter are not linked";
                    }

                    if (response == string.Empty)
                    {
                        questions.ForEach(question =>
                        {
                            if (question != null && !string.IsNullOrEmpty(question.Question) && !string.IsNullOrEmpty(question.FirstAnswer)
                            && !string.IsNullOrEmpty(question.SecondAnswer) && !string.IsNullOrEmpty(question.ThirdAnswer) && !string.IsNullOrEmpty(question.FourthAnswer)
                            && !string.IsNullOrEmpty(question.CorrectAnswer))
                            {
                                QuestionBank questionBank = new QuestionBank();
                                questionBank.Question = question.Question;
                                questionBank.FirstAnswer = question.FirstAnswer;
                                questionBank.SecondAnswer = question.SecondAnswer;
                                questionBank.ThirdAnswer = question.ThirdAnswer;
                                questionBank.FourthAnswer = question.FourthAnswer;
                                questionBank.CorrectAnswer = question.CorrectAnswer;
                                questionBank.DifficultyLevelId = 1;
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

                    if(questionsList.Count > 0)
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
            var currentUser = authService.GetCurrentUser();
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

                    if(questions != null && questions.Count > 0)
                    {
                        AssessmentSession newSession = new AssessmentSession();
                        newSession.AssessmentType = "Practice";
                        newSession.StudentId = currentUser.UserId;

                        dbContext.AssessmentSessions.Add(newSession);
                        long newAssessmentId = await dbContext.SaveChangesAsync();
                        response.AssessmentId = await CreateNewSession(questions, newAssessmentId);
                        response.Questions = questions;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return response;
        }
        private async Task<long> CreateNewSession(List<QuestionData> questions, long assessmentId)
        {
            long newAssessmentId = 0;
            try
            {
                if (questions == null || questions.Count == 0)
                { return newAssessmentId; }

                if (dbContext.AssessmentSessions.Any(x => x.Id == assessmentId))
                {
                    foreach (var question in questions)
                    {
                        AssessmentSessionQuestion assessmentSessionQuestion = new AssessmentSessionQuestion();
                        assessmentSessionQuestion.AssessmentSessionId = assessmentId;
                        assessmentSessionQuestion.QuestionId = question.Id;
                        dbContext.AssessmentSessionQuestions.Add(assessmentSessionQuestion);
                    }
                    await dbContext.SaveChangesAsync();
                }

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
                    finalQuestions = mapper.Map<List<QuestionData>>(selectedQuestions);
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

    }
}

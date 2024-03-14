using AutoMapper;
using Azure.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;
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

        public async Task<string> InsertQuestions(List<QuestionsDto> questions, QuestionsUploadRequestDto request)
        {
            string response = string.Empty;
            var currentUser = authService.GetCurrentUser();
            try
            {
                if (questions != null && questions.Count > 0)
                {
                    List<QuestionBank> questionsList = new List<QuestionBank>();

                    if (request.CourseId > 0)
                    {
                        if (!dbContext.Courses.Any(x => x.Id == request.CourseId && x.IsActive))
                            response = "Invalid Course";
                    }

                    if (request.SubjectId > 0)
                    {
                        if (!dbContext.Subjects.Any(x => x.Id == request.SubjectId && x.CourseId == request.CourseId && x.IsActive))
                            response = "Invalid Subject";
                    }

                    if (request.ChapterId > 0)
                    {
                        if (dbContext.Chapters.Any(x => x.Id == request.ChapterId && x.SubjectId == request.SubjectId && x.IsActive))
                            response = "Invalid Chapter";
                    }

                    if (request.TopicId > 0)
                    {
                        if (!dbContext.Topics.Any(x => x.Id == request.TopicId && x.ChapterId == request.ChapterId && x.IsActive))
                            response = "Invalid Topic";
                    }

                    if (request.SubTopicId > 0)
                    {
                        if (dbContext.SubTopics.Any(x => x.Id == request.SubTopicId && x.TopicId == request.TopicId && x.IsActive))
                           response = "Invalid SubTopic";
                    }
                    var difficultyLevels = dbContext.DifficultyLevels.ToList();
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

                                var difficultyLevel = difficultyLevels.Where(x => x.Name.ToLower() == question.DifficultyLevel.ToLower()).FirstOrDefault();
                                questionBank.DifficultyLevelId = difficultyLevel != null ? difficultyLevel.Id : 1000001;

                                if (request.ChapterId > 0)
                                    questionBank.ChapterId = request.ChapterId;

                                if (request.TopicId > 0)
                                    questionBank.TopicId = request.TopicId;

                                if (request.SubTopicId > 0)
                                    questionBank.SubTopicId = request.SubTopicId;

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
                        questions = await ReplaceTags(questions, clientType.Mobile);
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

        private async Task<List<QuestionData>> ReplaceTags(List<QuestionData> questions, clientType clientType)
        {
            foreach(var question in questions)
            {
                if(clientType == clientType.WEB)
                {
                    question.Question = ReplaceWebText(question.Question);
                    question.FirstAnswer = ReplaceWebText(question.FirstAnswer);
                    question.SecondAnswer = ReplaceWebText(question.SecondAnswer);
                    question.ThirdAnswer = ReplaceWebText(question.ThirdAnswer);
                    question.FourthAnswer = ReplaceWebText(question.FourthAnswer);
                }
                else if(clientType == clientType.Mobile)
                {
                    question.Question = ReplaceMobileText(question.Question);
                    question.FirstAnswer = ReplaceMobileText(question.FirstAnswer);
                    question.SecondAnswer = ReplaceMobileText(question.SecondAnswer);
                    question.ThirdAnswer = ReplaceMobileText(question.ThirdAnswer);
                    question.FourthAnswer = ReplaceMobileText(question.FourthAnswer);
                }
            }
            return questions;
        }

        private async Task<List<QuestionBank>> ReplaceTags(List<QuestionBank> questions, clientType clientType)
        {
            foreach (var question in questions)
            {
                if (clientType == clientType.WEB)
                {
                    question.Question = ReplaceWebText(question.Question);
                    question.FirstAnswer = ReplaceWebText(question.FirstAnswer);
                    question.SecondAnswer = ReplaceWebText(question.SecondAnswer);
                    question.ThirdAnswer = ReplaceWebText(question.ThirdAnswer);
                    question.FourthAnswer = ReplaceWebText(question.FourthAnswer);
                }
                else if (clientType == clientType.Mobile)
                {
                    question.Question = ReplaceMobileText(question.Question);
                    question.FirstAnswer = ReplaceMobileText(question.FirstAnswer);
                    question.SecondAnswer = ReplaceMobileText(question.SecondAnswer);
                    question.ThirdAnswer = ReplaceMobileText(question.ThirdAnswer);
                    question.FourthAnswer = ReplaceMobileText(question.FourthAnswer);
                }
            }
            return questions;
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

        private string ReplaceWebText(string source)
        {
            string updated = source.
                Replace(QuestionAndAnswerTags.QuestionImageOpeningTag, "<img class='questionImage' src='").
                Replace(QuestionAndAnswerTags.ImageClosingTag, "' />").
                Replace(QuestionAndAnswerTags.QuestionTextOpeningTag, "<span class='questionText'>").
                Replace(QuestionAndAnswerTags.AnswerImageOpeningTag, "<img class='answerImage' src='").
                Replace(QuestionAndAnswerTags.AnswerTextOpeningTag, "<span>").
                Replace(QuestionAndAnswerTags.TextClosingTag, "</span>").
                Replace(QuestionAndAnswerTags.NewLineTag, "</ br>");
            return updated;
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
                    .Take(totalQuestions)
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

        public async Task<bool> DeleteQuestions(List<long> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    if (dbContext.QuestionBanks.Any(x => x.Id == id))
                    {
                        var question = dbContext.QuestionBanks.FirstOrDefault(x => x.Id == id);
                        dbContext.QuestionBanks.Remove(question);
                    }
                    else
                    {
                        return false;
                    }
                }
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<QuestionsDto>> GetQuestions(QuestionsUploadRequestDto requestDto)
        {
            List<QuestionsDto> response = new List<QuestionsDto>();
            try
            {
                List<QuestionBank> questions = new List<QuestionBank>();

                if (requestDto.SubTopicId > 0)
                {
                    if (requestDto.ChapterId > 0 && requestDto.TopicId > 0)
                    {
                        questions.AddRange(dbContext.QuestionBanks.Where(x =>
                        x.ChapterId != null && x.ChapterId.Value == requestDto.ChapterId &&
                        x.TopicId != null && x.TopicId.Value == requestDto.TopicId &&
                        x.SubTopicId != null && x.SubTopicId.Value == requestDto.SubTopicId &&
                        x.IsActive));
                    }
                    else if (requestDto.TopicId > 0)
                    {
                        questions.AddRange(dbContext.QuestionBanks.Where(x =>
                        x.TopicId != null && x.TopicId.Value == requestDto.TopicId &&
                        x.SubTopicId != null && x.SubTopicId.Value == requestDto.SubTopicId &&
                        x.IsActive));
                    }
                    else
                    {
                        questions.AddRange(dbContext.QuestionBanks.Where(x =>
                        x.SubTopicId != null && x.SubTopicId.Value == requestDto.SubTopicId &&
                        x.IsActive));
                    }
                }
                else if (requestDto.TopicId > 0)
                {
                    if (requestDto.ChapterId > 0)
                    {
                        questions.AddRange(dbContext.QuestionBanks.Where(x =>
                        x.ChapterId != null && x.ChapterId.Value == requestDto.ChapterId &&
                        x.TopicId != null && x.TopicId.Value == requestDto.TopicId &&
                        x.IsActive));
                    }
                    else
                    {
                        questions.AddRange(dbContext.QuestionBanks.Where(x =>
                        x.TopicId != null && x.TopicId.Value == requestDto.TopicId &&
                        x.IsActive));
                    }
                }
                else if (requestDto.ChapterId > 0)
                {
                    questions.AddRange(dbContext.QuestionBanks.Where(x => 
                    x.ChapterId != null && x.ChapterId.Value == requestDto.ChapterId 
                    && x.IsActive));
                }


                if(questions.Count > 0)
                {
                    questions = await ReplaceTags(questions, clientType.WEB);
                    response = mapper.Map<List<QuestionsDto>>(questions);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }


        public async Task<List<AssessmentSessionDto>> GetAssessmentsByStudentId(long id)
        {
            var currentUser = authService.GetCurrentUser();
            List<AssessmentSessionDto> sessions = new List<AssessmentSessionDto>();
            try
            {
                var sessionsList = await Task.FromResult(dbContext.AssessmentSessions.Where(x => x.StudentId == id));

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
    }
}

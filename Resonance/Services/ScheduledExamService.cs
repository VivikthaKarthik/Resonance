﻿using AutoMapper;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;
using System.Threading.Tasks;

namespace ResoClassAPI.Services
{
    public class ScheduledExamService : IScheduledExamService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public ScheduledExamService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<List<ScheduledExamResponseDto>> GetScheduledExams()
        {
            List<ScheduledExamResponseDto> list = new List<ScheduledExamResponseDto>();
            var currentUser = authService.GetCurrentUser();
            try
            {
                var examsList = await Task.FromResult(dbContext.ScheduledExams.Where(x => x.IsActive));
                var courses = await Task.FromResult(dbContext.Courses.Where(x => x.IsActive).ToList());
                var subjects = await Task.FromResult(dbContext.Subjects.Where(x => x.IsActive).ToList());
                if (examsList != null)
                {
                    foreach (var exam in examsList)
                    {
                        var item = mapper.Map<ScheduledExamResponseDto>(exam);

                        if (exam.CourseId > 0)
                            item.Course = courses.Where(x => x.Id == exam.CourseId).FirstOrDefault().Name;

                        if (exam.SubjectId > 0)
                            item.Subject = subjects.Where(x => x.Id == exam.SubjectId).FirstOrDefault().Name;

                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<ScheduledExamResponseDto>> GetCompletedExams(long courseId, long subjectId)
        {
            List<ScheduledExamResponseDto> list = new List<ScheduledExamResponseDto>();
            var currentUser = authService.GetCurrentUser();
            try
            {
                List<ScheduledExam> examsList = new List<ScheduledExam>();
                var course = await Task.FromResult(dbContext.Courses.Where(x => x.Id == courseId && x.IsActive).FirstOrDefault());
                var subject = await Task.FromResult(dbContext.Subjects.Where(x => x.Id == subjectId && x.IsActive).FirstOrDefault());

                if (course != null && subject != null)
                    examsList = await Task.FromResult(dbContext.ScheduledExams.Where(x => 
                    x.CourseId == courseId && 
                    x.SubjectId == subjectId && 
                    x.EndDate < DateTime.Now &&
                    x.IsActive).ToList());
                else if(course != null)
                    examsList = await Task.FromResult(dbContext.ScheduledExams.Where(x => 
                    x.CourseId == courseId &&
                    x.EndDate < DateTime.Now &&
                    x.IsActive).ToList());

                if (examsList != null)
                {
                    foreach (var exam in examsList)
                    {
                        var item = mapper.Map<ScheduledExamResponseDto>(exam);

                        if (course != null)
                            item.Course = course.Name;

                        if (subject != null)
                            item.Subject = subject.Name;

                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<ScheduledExamResponseDto>> GetExams()
        {
            List<ScheduledExamResponseDto> list = new List<ScheduledExamResponseDto>();
            var currentUser = authService.GetCurrentUser();
            try
            {
                var student = await Task.FromResult(dbContext.Students.Where(x => x.Id == currentUser.UserId).First());
                var examsList = await Task.FromResult(dbContext.ScheduledExams.Where(x => x.CourseId == student.CourseId && x.IsActive));
                var courses = await Task.FromResult(dbContext.Courses.Where(x => x.IsActive).ToList());
                var subjects = await Task.FromResult(dbContext.Subjects.Where(x => x.IsActive).ToList());
                if (examsList != null)
                {
                    foreach (var exam in examsList)
                    {
                        var item = mapper.Map<ScheduledExamResponseDto>(exam);

                        if (exam.CourseId > 0)
                            item.Course = courses.Where(x => x.Id == exam.CourseId).FirstOrDefault().Name;

                        if (exam.SubjectId > 0)
                            item.Subject = subjects.Where(x => x.Id == exam.SubjectId).FirstOrDefault().Name;

                        list.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<string> InsertQuestions(List<QuestionsDto> questions, ScheduledExamRequestDto request)
        {
            string response = string.Empty;
            var currentUser = authService.GetCurrentUser();
            try
            {
                if (questions != null && questions.Count > 0)
                {
                    List<ScheduledExamQuestion> questionsList = new List<ScheduledExamQuestion>();

                    if (request.CourseId > 0)
                    {
                        if (!dbContext.Courses.Any(x => x.Id == request.CourseId && x.IsActive))
                            response = "Invalid Course";
                    }

                  
                    if (request.SubjectId > 0)
                    {
                        if (!dbContext.Subjects.Any(x => x.Id == request.SubjectId && x.ClassId == request.CourseId && x.IsActive))
                            response = "Invalid Subject";
                    }

                    var difficultyLevels = dbContext.DifficultyLevels.ToList();
                    if (response == string.Empty)
                    {
                        long scheduledExamId = await CreateScheduleExam(request);

                        if (scheduledExamId > 0)
                        {
                            questions.ForEach(item =>
                            {
                                if (item != null && !string.IsNullOrEmpty(item.Question) && !string.IsNullOrEmpty(item.FirstAnswer)
                                && !string.IsNullOrEmpty(item.SecondAnswer) && !string.IsNullOrEmpty(item.ThirdAnswer) && !string.IsNullOrEmpty(item.FourthAnswer)
                                && !string.IsNullOrEmpty(item.CorrectAnswer) && !string.IsNullOrEmpty(item.DifficultyLevel))
                                {
                                    ScheduledExamQuestion question = new ScheduledExamQuestion();
                                    question.ScheduledExamId = scheduledExamId;
                                    question.Question = item.Question;
                                    question.FirstAnswer = item.FirstAnswer;
                                    question.SecondAnswer = item.SecondAnswer;
                                    question.ThirdAnswer = item.ThirdAnswer;
                                    question.FourthAnswer = item.FourthAnswer;
                                    question.CorrectAnswer = item.CorrectAnswer;

                                    var difficultyLevel = difficultyLevels.Where(x => x.Name.ToLower() == item.DifficultyLevel.ToLower()).FirstOrDefault();
                                    question.DifficultyLevelId = difficultyLevel != null ? difficultyLevel.Id : 1000001;

                                    question.IsActive = true;
                                    question.CreatedBy = currentUser.Name;
                                    question.CreatedOn = DateTime.Now;
                                    question.ModifiedBy = currentUser.Name;
                                    question.ModifiedOn = DateTime.Now;
                                    questionsList.Add(question);
                                }
                            });
                        }
                    }

                    if (questionsList.Count > 0)
                    {

                        dbContext.ScheduledExamQuestions.AddRange(questionsList);
                        response = "Success";
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private async Task<long> CreateScheduleExam(ScheduledExamRequestDto request)
        {
            var currentUser = authService.GetCurrentUser();
            long examId;
            try
            {
                ScheduledExam exam = new ScheduledExam();
                exam.Name = request.Name;
                exam.StartDate = request.StartDate;
                exam.EndDate = request.EndDate;
                exam.MarksPerQuestion = request.MarksPerQuestion;
                exam.NegativeMarksPerQuestion = request.NegativeMarksPerQuestion;
                exam.MaxAllowedTime = request.MaxAllowedTime;

                if (request.CourseId > 0)
                    exam.CourseId = request.CourseId;

                if (request.SubjectId > 0)
                    exam.SubjectId = request.SubjectId;

                exam.IsActive = true;
                exam.CreatedBy = currentUser.Name;
                exam.CreatedOn = DateTime.Now;
                exam.ModifiedBy = currentUser.Name;
                exam.ModifiedOn = DateTime.Now;
                dbContext.ScheduledExams.Add(exam);
                await dbContext.SaveChangesAsync();

                examId = exam.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return examId;
        }

        public async Task<List<QuestionsDto>> GetScheduledExamQuestions(long id)
        {
            List<QuestionsDto> response = new List<QuestionsDto>();
            try
            {
                List<ScheduledExamQuestion> questions = new List<ScheduledExamQuestion>();
                var difficultyLevels = dbContext.DifficultyLevels.ToList();
                if (id > 0)
                {
                        questions.AddRange(dbContext.ScheduledExamQuestions.Where(x =>
                        x.ScheduledExamId == id && x.IsActive));
                }


                if (questions.Count > 0)
                {
                    questions = await ReplaceTags(questions, clientType.WEB);
                    foreach (var question in questions)
                    {
                        var mappedItem = mapper.Map<QuestionsDto>(question);
                        if (question.DifficultyLevelId > 0)
                            mappedItem.DifficultyLevel = difficultyLevels.First(x => x.Id == question.DifficultyLevelId).Name;
                        response.Add(mappedItem);
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<List<ScheduledExamResultDto>> GetScheduledExamResults(long id)
        {
            List<ScheduledExamResultDto> response = new List<ScheduledExamResultDto>();
            try
            {
                if (id > 0 && dbContext.ScheduledExams.Any(x => x.Id == id))
                {
                    var exam = dbContext.ScheduledExams.First(x => x.Id == id);
                    var courses = await Task.FromResult(dbContext.Courses.Where(x => x.IsActive).ToList());
                    var subjects = await Task.FromResult(dbContext.Subjects.Where(x => x.IsActive).ToList());
                    var students = await Task.FromResult(dbContext.Students.Where(x => x.IsActive).ToList());

                    var sessions = dbContext.ScheduledExamSessions.Where(x => x.ScheduledExamId == exam.Id).ToList();

                    if(sessions != null && sessions.Count > 0)
                    {
                        foreach (var session in sessions)
                        {
                            ScheduledExamResultDto result = new ScheduledExamResultDto();
                            if (exam.CourseId > 0)
                                result.Course = courses.First(x => x.Id != exam.CourseId).Name;

                            if (exam.SubjectId > 0)
                                result.Subject = subjects.First(x => x.Id != exam.SubjectId).Name;

                            if (session.StudentId > 0)
                                result.StudentName = students.First(x => x.Id == session.StudentId).Name;

                            result.ExamName = exam.Name;
                            result.StartDate = exam.StartDate;

                            var questions = dbContext.ScheduledExamResults.Where(x => x.ScheduledExamSessionId == session.Id);

                            if (dbContext.ScheduledExamResults.Any(x => x.ScheduledExamSessionId == session.Id))
                            {
                                result.TotalQuestions = questions.Count();
                                result.QuestionsAttempted = questions.Count(x => x.Result != null);
                                result.CorrectAnswers = questions.Count(x=> x.Result != null && x.Result == true);
                                result.WrongAnswers = questions.Count(x => x.Result != null && x.Result == false);
                                result.TotalScore = GetScore(questions.ToList(), exam);
                            }
                            response.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private int GetScore(List<ScheduledExamResult> results, ScheduledExam exam)
        {
            int score = 0;
            var correctResults = results.Where(x => x.Result != null && x.Result == true);
            score = correctResults.Count() * exam.MarksPerQuestion;

            if (exam.NegativeMarksPerQuestion > 0)
            {
                var wrongResults = results.Where(x => x.Result != null && x.Result == false);
                int negativeScore = wrongResults.Count() * exam.NegativeMarksPerQuestion;
                score = score - negativeScore;
            }
            return score;
        }
        private async Task<List<ScheduledExamQuestion>> ReplaceTags(List<ScheduledExamQuestion> questions, clientType clientType)
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

        public async Task<bool> DeleteQuestions(List<long> ids)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var id in ids)
                {
                    if (dbContext.ScheduledExamQuestions.Any(x => x.Id == id))
                    {
                        if (dbContext.ScheduledExamResults.Any(x => x.QuestionId == id))
                        {
                            var result = dbContext.ScheduledExamResults.FirstOrDefault(x => x.QuestionId == id);
                            dbContext.ScheduledExamResults.Remove(result);
                        }
                        var question = dbContext.ScheduledExamQuestions.FirstOrDefault(x => x.Id == id);
                        if (question != null)
                        {
                            question.IsActive = false;
                            question.ModifiedBy = currentUser.Name;
                            question.ModifiedOn = DateTime.Now;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ScheduledExamQuestionsResponseDto> GetQuestions(long id)
        {
            ScheduledExamQuestionsResponseDto response = new ScheduledExamQuestionsResponseDto();
            try
            {
                if(id > 0)
                {
                    var difficultyLevels = dbContext.DifficultyLevels.ToList();
                    if (id > 0)
                    {
                        var exam = dbContext.ScheduledExams.FirstOrDefault(x => x.Id == id);
                        if (exam != null)
                        {
                            var maxAllowedTime = exam.StartDate;
                            if (exam.MaxAllowedTime > 0)
                                maxAllowedTime = maxAllowedTime.AddMinutes(exam.MaxAllowedTime);

                            if (DateTime.Now < exam.StartDate)
                                throw new Exception("Cannot start the exam before schduled Time");

                            if (DateTime.Now > maxAllowedTime)
                                throw new Exception("Cannot start the exam after schduled Time");

                            if (dbContext.ScheduledExamSessions.Any(x => x.ScheduledExamId == exam.Id && x.StudentId == authService.GetCurrentUser().UserId
                            && x.StartTime != null && x.EndTime != null))
                                throw new Exception("You have already completed the exam");

                            response.MarksPerQuestion = exam.MarksPerQuestion;
                            response.NegativeMarksPerQuestion = exam.NegativeMarksPerQuestion;
                            response.HasNegativeMarking = exam.NegativeMarksPerQuestion != 0;
                            response.StartDate = exam.StartDate;
                            response.EndDate = exam.EndDate;
                            response.MaxAllowedTime = exam.MaxAllowedTime;
                            response.CourseId = exam.CourseId;
                            response.SubjectId = exam.SubjectId;
                            response.Course = dbContext.Courses.FirstOrDefault(x => x.Id == exam.CourseId).Name;
                            response.Subject = dbContext.Subjects.FirstOrDefault(x => x.Id == exam.SubjectId).Name;

                            List<ScheduledExamQuestion> dbQuestions = new List<ScheduledExamQuestion>();
                            dbQuestions.AddRange(dbContext.ScheduledExamQuestions.Where(x =>
                            x.ScheduledExamId == id && x.IsActive));

                            if (dbQuestions.Count > 0)
                            {
                                response.Questions = new List<ScheduledExamQuestionData>();
                                dbQuestions = await ReplaceTags(dbQuestions, clientType.Mobile);
                                foreach (var question in dbQuestions)
                                {
                                    var mappedItem = mapper.Map<ScheduledExamQuestionData>(question);
                                    if (question.DifficultyLevelId > 0)
                                        mappedItem.DifficultyLevel = difficultyLevels.First(x => x.Id == question.DifficultyLevelId).Name;
                                    response.Questions.Add(mappedItem);
                                }
                                response.TotalQuestions = response.Questions.Count;
                                response.ScheduleExamSessionId = await CreateNewSession(response.Questions, id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private async Task<long> CreateNewSession(List<ScheduledExamQuestionData> questions, long examId)
        {
            var currentUser = authService.GetCurrentUser();
            long newSessionId = 0;
            try
            {
                if (dbContext.ScheduledExamSessions.Any(x => x.ScheduledExamId == examId && x.StudentId == currentUser.UserId))
                {
                    return dbContext.ScheduledExamSessions.First(x => x.ScheduledExamId == examId && x.StudentId == currentUser.UserId).Id;
                }
                else
                {
                    if (questions == null || questions.Count == 0)
                    { return newSessionId; }

                    ScheduledExamSession newSession = new ScheduledExamSession();
                    newSession.ScheduledExamId = examId;
                    newSession.StudentId = currentUser.UserId;
                    dbContext.ScheduledExamSessions.Add(newSession);
                    await dbContext.SaveChangesAsync();
                    newSessionId = newSession.Id;

                    List<ScheduledExamResult> ScheduledExamResults = new List<ScheduledExamResult>();
                    foreach (var question in questions)
                    {
                        ScheduledExamResult examResult = new ScheduledExamResult();
                        examResult.ScheduledExamSessionId = newSessionId;
                        examResult.QuestionId = question.Id;
                        ScheduledExamResults.Add(examResult);
                    }
                    await dbContext.ScheduledExamResults.AddRangeAsync(ScheduledExamResults);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return newSessionId;
        }

        public async Task<bool> StartAssessment(long examId)
        {
            try
            {
                var exam = dbContext.ScheduledExamSessions.Where(x => x.Id == examId).FirstOrDefault();
                if (exam != null)
                {
                    exam.StartTime = DateTime.Now;
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

        public async Task<bool> EndAssessment(long examId)
        {
            try
            {
                var exam = dbContext.ScheduledExamSessions.Where(x => x.Id == examId).FirstOrDefault();
                if (exam != null)
                {
                    exam.EndTime = DateTime.Now;
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

        public async Task<bool> UpdateQuestionStatus(UpdateScheduledExamQuestionStatusDto request)
        {
            try
            {
                var examResult = dbContext.ScheduledExamResults.Where(x => x.ScheduledExamSessionId == request.AssessmentId && x.QuestionId == request.QuestionId).FirstOrDefault();
                if (examResult != null)
                {
                    examResult.Result = request.Result;

                    if (!string.IsNullOrEmpty(request.SelectedAnswer))
                        examResult.SelectedAnswer = request.SelectedAnswer;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

    }
}

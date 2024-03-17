using AutoMapper;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;

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
                        if (!dbContext.Subjects.Any(x => x.Id == request.SubjectId && x.CourseId == request.CourseId && x.IsActive))
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


    }
}

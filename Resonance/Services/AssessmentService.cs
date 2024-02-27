using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;

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
                                questionBank.CreatedBy = "SYSTEM";
                                questionBank.CreatedOn = DateTime.Now;
                                questionBank.ModifiedBy = "SYSTEM";
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
    }
}

﻿using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<List<AssessmentSessionDto>> GetAssessmentSessions();
        Task<AssessmentConfigurationDto> GetAssessmentConfiguration();
        Task<string> InsertQuestions(List<QuestionsDto> questions, QuestionsUploadRequestDto request);
        Task<QuestionResponseDto> GetQuestions(QuestionRequestDto requestDto);
        Task<QuestionResponseDto> GetQuestionsByChapter(long id);
        Task<QuestionResponseDto> GetQuestionsByTopic(long id);
        Task<QuestionResponseDto> GetQuestionsBySubTopic(long id);
        Task<bool> StartAssessment(long assessmentId);
        Task<bool> EndAssessment(long assessmentId);
        Task<bool> UpdateQuestionStatus(UpdateAssessmentStatusDto request);
        Task<bool> DeleteQuestions(List<long> ids);                
        Task<List<QuestionsDto>> GetQuestions(QuestionsUploadRequestDto requestDto);
        Task<List<AssessmentSessionResponseDto>> GetAssessmentsByStudentId(long id);

    }
}

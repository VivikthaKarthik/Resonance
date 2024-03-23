using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectsViewDto>> GetAllSubjects();
        Task<SubjectsViewDto> GetSubject(long subjectId);
        Task<long> CreateSubject(SubjectDto newItem);
        Task<bool> UpdateSubject(SubjectDto updatedItem);
        Task<bool> DeleteSubject(long subjectId);
        Task<List<SubjectDto>> GetSubjectsWithClassId(long classId); 
        Task<bool> InsertSubjectsAndLinkToCourses(List<SubjectDto> subjects);
    }
}

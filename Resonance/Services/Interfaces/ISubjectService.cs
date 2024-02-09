using ResoClassAPI.DTOs;

namespace ResoClassAPI.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetAllSubjects();
        Task<SubjectDto> GetSubject(int subjectId);
        Task<long> CreateSubject(SubjectDto newItem);
        Task<bool> UpdateSubject(SubjectDto updatedItem);
        Task<bool> DeleteSubject(int subjectId);
    }
}

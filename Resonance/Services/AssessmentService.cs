using AutoMapper;
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


        public async Task<bool> InsertQuestions(List<QuestionsDto> questions)
        {
            try
            {
               
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

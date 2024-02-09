using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class ChapterService: IChapterService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        public ChapterService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
        }

        public async Task<List<ChapterDto>> GetAllChapters()
        {
            List<ChapterDto> dtoObjects = new List<ChapterDto>();
            var chapters =   await Task.FromResult(dbContext.Chapters.Where(item => item.IsActive == true).ToList());
            if (chapters != null && chapters.Count > 0)
            {
                
                foreach (var chapter in chapters)
                {
                    var dtoObject = mapper.Map<ChapterDto>(chapter);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }

       
        public async Task<long> CreateChapter(ChapterDto chapter)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Chapter newChapter = mapper.Map<Chapter>(chapter);
                newChapter.IsActive = true;
                newChapter.CreatedBy = newChapter.ModifiedBy = currentUser.Name;
                newChapter.CreatedOn = newChapter.ModifiedOn = DateTime.Now;
               
                dbContext.Chapters.Add(newChapter);
                await dbContext.SaveChangesAsync();
                return newChapter.Id;
            }
            return 0;
        }

        public async Task<bool> UpdateChapter(ChapterDto updatedChapter)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Chapters.FirstOrDefault(item => item.Id == updatedChapter.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedChapter.Name))
                    existingItem.Name = updatedChapter.Name;
                existingItem.SubjectId = updatedChapter.SubjectId;
                existingItem.CourseId = updatedChapter.CourseId;
                if (!string.IsNullOrEmpty(updatedChapter.Thumbnail))
                    existingItem.Thumbnail = updatedChapter.Thumbnail;
                existingItem.IsRecommended = updatedChapter.IsRecommended;
                existingItem.IsActive = true;
                if (!string.IsNullOrEmpty(updatedChapter.Name))
                    existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

       

        public async Task<bool> DeleteChapter(int chapterId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Users.FirstOrDefault(item => item.Id == chapterId);

            if (existingItem != null)
            {
                existingItem.IsActive = false;
                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async  Task<ChapterDto> GetChapter(int chapterId)
        {
            var chapter = await Task.FromResult(dbContext.Chapters.FirstOrDefault(item => item.Id == chapterId && item.IsActive == true));
            if (chapter != null)
            {
                var dtoObject = mapper.Map<ChapterDto>(chapter);
                //dtoObject.Role = dbContext.Roles.First(item => item.Id == user.RoleId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }
    }
}

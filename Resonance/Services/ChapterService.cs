using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ChapterResponseDto>> GetAllChapters()
        {
            List<ChapterResponseDto> dtoObjects = new List<ChapterResponseDto>();
            var chapters =   await Task.FromResult(dbContext.Chapters.Where(item => item.IsActive == true).ToList());
            if (chapters != null && chapters.Count > 0)
            {                
                foreach (var chapter in chapters)
                {
                    var dtoObject = mapper.Map<ChapterResponseDto>(chapter);
                    dtoObjects.Add(dtoObject);
                }
                return dtoObjects;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<long> CreateChapter(ChapterRequestDto chapter)
        {
            var currentUser = authService.GetCurrentUser();

            if (currentUser != null)
            {
                Chapter newChapter = mapper.Map<Chapter>(chapter);

                if (chapter.SubjectId > 0)
                {
                    if (dbContext.Subjects.Any(x => x.Id == chapter.SubjectId))
                        newChapter.SubjectId = chapter.SubjectId;
                    else
                        throw new Exception("Invalid SubjectId");
                }
                else
                    throw new Exception("SubjectId is missing");

                newChapter.IsActive = true;
                newChapter.CreatedBy = newChapter.ModifiedBy = currentUser.Name;
                newChapter.CreatedOn = newChapter.ModifiedOn = DateTime.Now;
               
                dbContext.Chapters.Add(newChapter);
                await dbContext.SaveChangesAsync();
                return newChapter.Id;
            }
            return 0;
        }

        public async Task<bool> UpdateChapter(ChapterRequestDto updatedChapter)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Chapters.FirstOrDefault(item => item.Id == updatedChapter.Id && item.IsActive == true);

            if (existingItem != null)
            {
                if (!string.IsNullOrEmpty(updatedChapter.Name))
                    existingItem.Name = updatedChapter.Name;

                if (!string.IsNullOrEmpty(updatedChapter.Thumbnail))
                    existingItem.Thumbnail = updatedChapter.Thumbnail;

                if (updatedChapter.SubjectId > 0)
                {
                    if (dbContext.Subjects.Any(x => x.Id == updatedChapter.SubjectId))
                        existingItem.SubjectId = updatedChapter.SubjectId;
                    else
                        throw new Exception("Invalid SubjectId");
                }

                if (updatedChapter.IsRecommended != null)
                    existingItem.IsRecommended = updatedChapter.IsRecommended.Value;

                existingItem.ModifiedBy = currentUser.Name;
                existingItem.ModifiedOn = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
             
        public async Task<bool> DeleteChapter(long chapterId)
        {
            var currentUser = authService.GetCurrentUser();
            var existingItem = dbContext.Users.FirstOrDefault(item => item.Id == chapterId && item.IsActive == true);

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

        public async  Task<ChapterResponseDto> GetChapter(long chapterId)
        {
            var chapter = await Task.FromResult(dbContext.Chapters.FirstOrDefault(item => item.Id == chapterId && item.IsActive == true));
            if (chapter != null)
            {
                var dtoObject = mapper.Map<ChapterResponseDto>(chapter);

                if (chapter.SubjectId > 0 && dbContext.Subjects.Any(x => x.Id == chapter.SubjectId))
                    dtoObject.SubjectName = dbContext.Subjects.FirstOrDefault(x => x.Id == chapter.SubjectId).Name;
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }
    }
}

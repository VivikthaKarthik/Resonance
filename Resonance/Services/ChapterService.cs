using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
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
            var chapters =   await Task.FromResult(dbContext.Chapters.Where(item => item.IsActive == true).OrderByDescending(x => x.CreatedOn).ToList());
            if (chapters != null && chapters.Count > 0)
            {                
                //foreach (var chapter in chapters)
                {
                    dtoObjects = mapper.Map<List<ChapterResponseDto>>(chapters);
                    //dtoObjects.Add(dtoObject);
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
                long subjectId = 0;
                Chapter newChapter = mapper.Map<Chapter>(chapter);

                if (chapter.SubjectId > 0)
                {
                    if (dbContext.Subjects.Any(x => x.Id == chapter.SubjectId))
                        subjectId = dbContext.Subjects.Where(x => x.Id == chapter.SubjectId).First().Id;
                    else
                        throw new Exception("Invalid SubjectId");
                }
                else
                    throw new Exception("SubjectId is missing");

                newChapter.SubjectId = chapter.SubjectId;
                newChapter.IsActive = true;
                newChapter.CreatedBy = newChapter.ModifiedBy = currentUser.Name;
                newChapter.CreatedOn = newChapter.ModifiedOn = DateTime.Now;
               
                dbContext.Chapters.Add(newChapter);
                await dbContext.SaveChangesAsync();

                //SubjectChapter item = new SubjectChapter();
                //item.ChapterId = chapter.Id;
                //item.SubjectId = subjectId;
                //item.IsActive = true;
                //item.CreatedBy = item.ModifiedBy = currentUser.Name;
                //item.CreatedOn = item.ModifiedOn = DateTime.Now;
                //dbContext.SubjectChapters.Add(item);
                //await dbContext.SaveChangesAsync();
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

                if (!string.IsNullOrEmpty(updatedChapter.Description))
                    existingItem.Description = updatedChapter.Description;

                if (!string.IsNullOrEmpty(updatedChapter.Thumbnail))
                    existingItem.Thumbnail = updatedChapter.Thumbnail;
                                
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
            var existingItem = dbContext.Chapters.FirstOrDefault(item => item.Id == chapterId && item.IsActive == true);

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

                if (dbContext.Subjects.Any(x => x.Id == chapter.SubjectId))
                {
                    dtoObject.SubjectName = dbContext.Subjects.FirstOrDefault(x => x.Id == chapter.SubjectId).Name;
                }
                return dtoObject;
            }
            else
                throw new Exception("Not Found");
        }

        public async Task<List<RecommendedChapterResponseDto>> GetRecommendedChapters()
        {
            var currentUser = authService.GetCurrentUser();
            List<RecommendedChapterResponseDto> returnList = new List<RecommendedChapterResponseDto>();

            long courseId = dbContext.Students.Where(x => x.Id == currentUser.UserId).FirstOrDefault().CourseId;
            if (courseId > 0)
            {
                var subjects = dbContext.Subjects.Where(x => x.CourseId == courseId).ToList();

                if (subjects != null && subjects.ToList().Count > 0)
                {
                    foreach (var subject in subjects.ToList())
                    {
                        RecommendedChapterResponseDto returnObj = new RecommendedChapterResponseDto();
                        returnObj.SubjectId = subject.Id;
                        returnObj.SubjectName = subject.Name;

                        var chapters = dbContext.Chapters.Where(x => x.SubjectId == subject.Id && x.IsRecommended && x.IsActive).ToList();

                        if (chapters != null && chapters.Count > 0)
                        {
                            foreach (var chapter in chapters)
                            {
                                if (chapter != null)
                                {
                                    RecommendedChapterDto recommendedChapterDto = new RecommendedChapterDto()
                                    {
                                        Id = chapter.Id,
                                        Name = chapter.Name,
                                        Description = !string.IsNullOrEmpty(chapter.Description) ? chapter.Description : string.Empty,
                                        Thumbnail = chapter.Thumbnail,
                                    };
                                    returnObj.RecommendedChapters.Add(recommendedChapterDto);
                                }
                            }

                            if (returnObj.RecommendedChapters != null && returnObj.RecommendedChapters.Count > 0)
                                returnList.Add(returnObj);
                        }

                    }
                    return returnList;
                }
            }
            throw new Exception("Not Found");
        }

        public async Task<List<ChapterResponseDto>> GetChaptersWithSubjectId(long subjectId)
        {
            var chapters = dbContext.Chapters.Where(x => x.SubjectId == subjectId && x.IsActive).ToList();

            if (chapters != null && chapters.Count > 0)
            {
                List<ChapterResponseDto> chaptersList = new List<ChapterResponseDto>();
                foreach(var chapter in chapters)
                {
                    chaptersList.Add(new ChapterResponseDto
                    {
                        Id = chapter.Id,
                        Name = chapter.Name,
                        Thumbnail = chapter.Thumbnail,
                        Description = !string.IsNullOrEmpty(chapter.Description) ? chapter.Description : string.Empty,
                        SubjectId = chapter.SubjectId,
                        SubjectName = dbContext.Subjects.Where(x => x.Id == chapter.SubjectId).First().Name,
                        IsRecommended = chapter.IsRecommended,
                    });

                }

                return chaptersList;
            }
            else
                throw new Exception("Not Found");
        }


        public async Task<bool> InsertChaptersAndLinkToSubjects(List<ChapterExcelRequestDto> chapters)
        {
            try
            {
                var currentUser = authService.GetCurrentUser();
                foreach (var chapterDto in chapters)
                {
                    // Get the course ID based on the course name
                    long courseId = dbContext.Courses.Where(c => c.Name == chapterDto.Course && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (courseId == 0)
                    {
                        throw new Exception($"Course '{chapterDto.Course}' not found in the database.");
                    }

                    long subjectId = dbContext.Subjects.Where(c => c.Name == chapterDto.Subject && c.CourseId == courseId && c.IsActive).Select(c => c.Id).FirstOrDefault();

                    if (subjectId == 0)
                    {
                        throw new Exception($"Course '{chapterDto.Subject}' not found in the database.");
                    }

                    // Insert the subject if it doesn't exist
                    Chapter existingChapter = dbContext.Chapters.FirstOrDefault(s => s.Name == chapterDto.Name && s.IsActive);

                    if (existingChapter == null)
                    {
                        existingChapter = new Chapter
                        {
                            Name = chapterDto.Name,
                            Description = !string.IsNullOrEmpty(chapterDto.Description) ? chapterDto.Description : string.Empty,
                            Thumbnail = chapterDto.Thumbnail,
                            SubjectId = subjectId,
                            IsActive = true,
                            CreatedBy = currentUser.Name,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser.Name,
                            ModifiedOn = DateTime.Now
                        };
                        dbContext.Chapters.Add(existingChapter);

                        await dbContext.SaveChangesAsync();
                    }

                    //var linkedItem = new SubjectChapter
                    //{
                    //    ChapterId = existingChapter.Id,
                    //    SubjectId = subjectId,
                    //    IsActive = true,
                    //    CreatedBy = currentUser.Name,
                    //    CreatedOn = DateTime.Now,
                    //    ModifiedBy = currentUser.Name,
                    //    ModifiedOn = DateTime.Now
                    //};
                    //dbContext.SubjectChapters.Add(linkedItem);
                    //await dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

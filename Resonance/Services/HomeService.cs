using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;

namespace ResoClassAPI.Services
{
    public class HomeService : IHomeService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly ICommonService commonService;
        public HomeService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper, ICommonService commonService)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
            this.commonService = commonService;
        }
        public async Task<SearchResponseDto> SearchItems(string text)
        {
            var currentUser = authService.GetCurrentUser();
            SearchResponseDto items = new SearchResponseDto();

            if (!string.IsNullOrEmpty(text))
            {
                var student = dbContext.Students.Where(x => x.Id == currentUser.UserId).FirstOrDefault();

                var chapters = from chapter in dbContext.Chapters 
                               where chapter.Name.ToLower().Contains(text)
                            join subject in dbContext.Subjects on chapter.SubjectId equals subject.Id
                            where subject.ClassId == student.ClassId
                               select new ListItemDto
                            {
                                Id = chapter.Id,
                                Name = chapter.Name
                            };
                items.Chapters = chapters.ToList();

                var topics = from topic in dbContext.Topics
                             where topic.Name.ToLower().Contains(text)
                             join chapter in dbContext.Chapters on topic.ChapterId equals chapter.Id
                             join subject in dbContext.Subjects on chapter.SubjectId equals subject.Id
                               where subject.ClassId == student.ClassId
                             select new ListItemDto
                               {
                                   Id = topic.Id,
                                   Name = topic.Name
                               };
                items.Topics = topics.ToList();

                var subTopics = from subTopic in dbContext.SubTopics
                                where subTopic.Name.ToLower().Contains(text)
                                join topic in dbContext.Topics on subTopic.TopicId equals topic.Id
                                join chapter in dbContext.Chapters on topic.ChapterId equals chapter.Id
                                join subject in dbContext.Subjects on chapter.SubjectId equals subject.Id
                                where subject.ClassId == student.ClassId
                                select new ListItemDto
                                {
                                    Id = topic.Id,
                                    Name = topic.Name
                                };
                items.SubTopics = subTopics.ToList();

                //var chapters = await Task.FromResult(dbContext.Chapters.Where(item => item.Name.ToLower().Contains(text) && item.IsActive == true).OrderBy(x => x.Name).ToList());
                //var topics = await Task.FromResult(dbContext.Topics.Where(item => item.Name.ToLower().Contains(text) && item.IsActive == true).OrderBy(x => x.Name).ToList());
                //var subTopics = await Task.FromResult(dbContext.SubTopics.Where(item => item.Name.ToLower().Contains(text) && item.IsActive == true).OrderBy(x => x.Name).ToList());

                //if (chapters != null && chapters.Count > 0)
                //{
                //    items.Chapters = new List<SearchItem>();
                //    foreach (var chapter in chapters)
                //    {
                //        SearchItem item = new SearchItem()
                //        {
                //            Id = chapter.Id,
                //            Name = chapter.Name
                //        };
                //        items.Chapters.Add(item);
                //    }
                //}
                //if (topics != null && topics.Count > 0)
                //{
                //    items.Topics = new List<SearchItem>();
                //    foreach (var topic in topics)
                //    {
                //        SearchItem item = new SearchItem()
                //        {
                //            Id = topic.Id,
                //            Name = topic.Name
                //        };
                //        items.Topics.Add(item);
                //    }
                //}
                //if (subTopics != null && subTopics.Count > 0)
                //{
                //    items.SubTopics = new List<SearchItem>();
                //    foreach (var subTopic in subTopics)
                //    {
                //        SearchItem item = new SearchItem()
                //        {
                //            Id = subTopic.Id,
                //            Name = subTopic.Name
                //        };
                //        items.SubTopics.Add(item);
                //    }
                //}
            }
            return items;
        }

        public async Task<List<ListItemDto>> GetListItems(string tableName, string? parentName, long? parentId)
        {
            return await commonService.GetListItems(tableName, parentName, parentId);
        }
    }
}

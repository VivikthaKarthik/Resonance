using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities;
using System.Collections.Generic;

namespace ResoClassAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly ResoClassContext dbContext;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly ICommonService commonService;
        public ReportService(ResoClassContext _dbContext, IAuthService _authService, IMapper _mapper, ICommonService _commonService)
        {
            dbContext = _dbContext;
            authService = _authService;
            mapper = _mapper;
            commonService = _commonService;
        }

        public async Task<List<SubjectWeightageDto>> GetSubjectsReport()
        {
            var currentUser = authService.GetCurrentUser();
            List<SubjectWeightageDto> items = new List<SubjectWeightageDto>();

            var student = dbContext.Students.Where(x => x.Id == currentUser.UserId).FirstOrDefault();

            var course = dbContext.Courses.Where(x => x.Id == student.CourseId).FirstOrDefault();

            var classObject = dbContext.Classes.Where(x => x.Id == student.ClassId).FirstOrDefault();

            var subjects = dbContext.Subjects.Where(x => x.ClassId == classObject.Id && x.IsActive).ToList();

            if(subjects.Any())
            {
                foreach(var subject in subjects)
                {
                    int totalQuestions = commonService.GetTotalQUestionsCountBySubjectId(subject.Id);
                    int correctAnswers = commonService.GetCorrectAnswersCountBySubjectId(subject.Id);

                    double percentage = ((double)totalQuestions / 100) * correctAnswers;
                    items.Add(new SubjectWeightageDto() { Name = subject.Name, Percentage = Math.Round(percentage, 2), ColorCode = subject.ColorCode });
                }
            }

            return items;
        }

        public async Task<StudentReportDto> GetStudentReport()
        {
            var currentUser = authService.GetCurrentUser();
            StudentReportDto report = new StudentReportDto();
            var student = dbContext.Students.Where(x => x.Id == currentUser.UserId).FirstOrDefault();

            if (student != null)
            {
                List<AssessmentSessionQuestion> questions = GetQuestionsByStudentId(student.Id);

                if (questions != null && questions.Count > 0)
                {
                    Course course = GetCourse(student);
                    var subjects = GetSubjects(questions);
                    var chapters = GetChapters(questions);
                    var topics = GetTopics(questions);

                    report.StudentName = student.Name;
                    report.CourseName = course.Name;
                    report.TotalAttempted = questions.Count;
                    report.CorrectAnswers = questions.Where(x => x.Result != null && x.Result.Value).Count();
                    report.WrongAnswers = questions.Where(x => x.Result != null && !x.Result.Value).Count();

                    report.SubjectsWeightage = GetSubjectWeightage(subjects);

                    if (subjects != null && subjects.Any())
                    {
                        report.PracticedFromSubjects = new List<ListItemDto>();
                        report.PracticedFromSubjects.AddRange(subjects.Select(y => new ListItemDto()
                        {
                            Id = y.Id,
                            Name = y.Name
                        }));
                    }

                    if (chapters != null && chapters.Any())
                    {
                        report.PracticedFromChapters = new List<ListItemDto>();
                        report.PracticedFromChapters.AddRange(chapters.Select(y => new ListItemDto()
                        {
                            Id = y.Id,
                            Name = y.Name
                        }));
                    }

                    if (topics != null && topics.Any())
                    {
                        report.PracticedFromTopics = new List<ListItemDto>();
                        report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
                        {
                            Id = y.Id,
                            Name = y.Name
                        }));
                    }
                    report.CorrectAnswersAnalysis = GetSubjectAnalysis(questions.Where(x => x.Result != null && x.Result.Value).ToList(), chapters, subjects, topics);
                    report.WrongAnswersAnalysis = GetSubjectAnalysis(questions.Where(x => x.Result != null && !x.Result.Value).ToList(), chapters, subjects, topics);

                }
            }
            return report;
        }

        private List<ItemWiseWeightage> GetSubjectWeightage(List<Subject> subjects)
        {
            List<ItemWiseWeightage> items = new List<ItemWiseWeightage>();
            foreach (var subject in subjects)
            {
                int totalQuestions = commonService.GetTotalQUestionsCountBySubjectId(subject.Id);
                int correctAnswers = commonService.GetCorrectAnswersCountBySubjectId(subject.Id);

                double percentage = ((double)totalQuestions / 100) * correctAnswers;
                items.Add(new ItemWiseWeightage() { Name = subject.Name, percentage = Math.Round(percentage, 2) });
            }
            return items;
        }
        public async Task<SubjectReportDto> GetSubjectReport(long id)
        {
            var currentUser = authService.GetCurrentUser();
            SubjectReportDto report = new SubjectReportDto();

            var questions = GetQuestionsBySubjectId(id);

            if (questions != null && questions.Count > 0)
            {
                report.SubjectId = id;
                report.TotalAttempted = questions.Count;
                report.CorrectAnswers = questions.Where(x => x.Result != null && x.Result.Value).Count();
                report.WrongAnswers = questions.Where(x => x.Result != null && !x.Result.Value).Count();

                var chapters = GetChapters(questions);
                if (chapters != null && chapters.Any())
                {
                    report.PracticedFromChapters = new List<ListItemDto>();
                    report.PracticedFromChapters.AddRange(chapters.Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    }));
                }

                var topics = GetTopics(questions);

                if (topics != null && topics.Any())
                {
                    report.PracticedFromTopics = new List<ListItemDto>();
                    report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    }));
                }

                report.CorrectAnswersAnalysis = GetChapterAnalysis(questions.Where(x => x.Result != null && x.Result.Value).ToList(), chapters, topics);
                report.WrongAnswersAnalysis = GetChapterAnalysis(questions.Where(x => x.Result != null && !x.Result.Value).ToList(), chapters, topics);
            }

            return report;
        }

        public async Task<ChapterReportDto> GetChapterReport(long id)
        {
            var currentUser = authService.GetCurrentUser();
            ChapterReportDto report = new ChapterReportDto();
            var chapter = dbContext.Chapters.Where(x => x.Id == id).FirstOrDefault();

            if (chapter != null)
            {
                var allQuestions = dbContext.AssessmentSessionQuestions.Where(x => x.ChapterId == id && x.Result != null).ToList();
                var questions = RemoveDuplicateQuestions(allQuestions);
                report.ChapterId = chapter.Id;
                report.TotalAttempted = questions.Count;
                report.CorrectAnswers = questions.Where(x => x.Result.Value).Count();
                report.WrongAnswers = questions.Where(x => !x.Result.Value).Count();

                var topics = GetTopics(questions);
                if (topics != null && topics.Any())
                {
                    report.PracticedFromTopics = new List<ListItemDto>();
                    report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    }));
                }

                report.CorrectAnswersAnalysis = GetTopicAnalysis(questions.Where(x => x.Result.Value).ToList(), topics);
                report.WrongAnswersAnalysis = GetTopicAnalysis(questions.Where(x => !x.Result.Value).ToList(), topics);
            }

            return report;
        }

        public async Task<AssessmentReportDto> GetAssessmentReport(long id)
        {
            var currentUser = authService.GetCurrentUser();
            AssessmentReportDto report = new AssessmentReportDto();
            var assessmentSession = dbContext.AssessmentSessions.Where(x => x.Id == id).FirstOrDefault();

            if (assessmentSession != null)
            {
                var allQuestions = dbContext.AssessmentSessionQuestions.Where(x => x.AssessmentSessionId == id && x.Result != null).ToList();
                var questions = RemoveDuplicateQuestions(allQuestions);
                report.AssessmentId = assessmentSession.Id;
                report.AssessmentName = assessmentSession.Name;
                report.PracticedOn = assessmentSession.StartTime.Value;
                report.TotalAttempted = questions.Count;
                report.CorrectAnswers = questions.Where(x => x.Result.Value).Count();
                report.WrongAnswers = questions.Where(x => !x.Result.Value).Count();

                var chapters = GetChapters(questions);
                if (chapters != null && chapters.Any())
                {
                    report.PracticedFromChapters = new List<ListItemDto>();
                    report.PracticedFromChapters.AddRange(chapters.Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    }));
                }

                var topics = GetTopics(questions);
                if (topics != null && topics.Any())
                {
                    report.PracticedFromTopics = new List<ListItemDto>();
                    report.PracticedFromTopics.AddRange(topics.Select(y => new ListItemDto()
                    {
                        Id = y.Id,
                        Name = y.Name
                    }));
                }

                report.CorrectAnswersAnalysis = GetChapterAnalysis(questions.Where(x => x.Result.Value).ToList(), chapters, topics);
                report.WrongAnswersAnalysis = GetChapterAnalysis(questions.Where(x => !x.Result.Value).ToList(), chapters, topics);

                report.Keys = new List<QuestionKeyDto>();
                foreach(var item in questions)
                {
                    var question = dbContext.QuestionBanks.First(x => x.Id == item.QuestionId);
                    question.Question = ReplaceMobileText(question.Question);
                    question.FirstAnswer = ReplaceMobileText(question.FirstAnswer);
                    question.SecondAnswer = ReplaceMobileText(question.SecondAnswer);
                    question.ThirdAnswer = ReplaceMobileText(question.ThirdAnswer);
                    question.FourthAnswer = ReplaceMobileText(question.FourthAnswer);
                    if (question != null)
                    {
                        var key = mapper.Map<QuestionKeyDto>(question);
                        if (key != null)
                        {
                            key.SelectedAnswer = item.SelectedAnswer;
                            report.Keys.Add(key);
                        }

                    }
                }


            }

            return report;
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

        private SubjectAnalysisDto GetSubjectAnalysis(List<AssessmentSessionQuestion> questions, List<Chapter> chapters, List<Subject> subjects, List<Topic> topics)
        {
            SubjectAnalysisDto answersAnalysis = new SubjectAnalysisDto();

            answersAnalysis.DifficultyLevelAnalysis = GetDifficultyLevelAnalysis(questions);

            List<Chapter> chapterIds = new List<Chapter>();
            var groupedChaperQuestions = questions.GroupBy(q => q.ChapterId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedChaperQuestions.Any())
            {
                answersAnalysis.ChapterWiseAnalysis = new List<ItemWiseAnalysisDto>();
                answersAnalysis.SubjectWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedChaperQuestions)
                {
                    if (group.Level != null)
                    {
                        var chaptersData = chapters.First(x => x.Id == group.Level.Value);
                        chapterIds.Add(chaptersData);
                        ItemWiseAnalysisDto newItem = new ItemWiseAnalysisDto()
                        {
                            Name = chaptersData.Name,
                            QuestionsCount = groupedChaperQuestions.FirstOrDefault(g => g.Level.Value == chaptersData.Id)?.Count ?? 0
                        };
                        answersAnalysis.ChapterWiseAnalysis.Add(newItem);

                        var subject = subjects.Where(x => x.Id == chaptersData.SubjectId).FirstOrDefault();

                        if(!answersAnalysis.SubjectWiseAnalysis.Any(x => x.Name == subject.Name))
                        {
                            answersAnalysis.SubjectWiseAnalysis.Add(new ItemWiseAnalysisDto()
                            {
                                Name = subject.Name,
                                QuestionsCount = newItem.QuestionsCount
                            });
                        }
                        else
                        {
                            var item = answersAnalysis.SubjectWiseAnalysis.Where(x => x.Name == subject.Name).First();
                            item.QuestionsCount += newItem.QuestionsCount;
                        }
                    }
                }
            }

            var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedTopicQuestions.Any())
            {
                answersAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedTopicQuestions)
                {
                    if (group.Level != null)
                    {
                        var topicsData = topics.First(x => x.Id == group.Level.Value);
                        answersAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = topicsData.Name,
                            QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
                        });
                    }
                }
            }

            return answersAnalysis;
        }

        private ChapterAnalysisDto GetChapterAnalysis(List<AssessmentSessionQuestion> questions, List<Chapter> chapters, List<Topic> topics)
        {
            ChapterAnalysisDto answersAnalysis = new ChapterAnalysisDto();

            answersAnalysis.DifficultyLevelAnalysis = GetDifficultyLevelAnalysis(questions);

            var groupedChaperQuestions = questions.GroupBy(q => q.ChapterId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedChaperQuestions.Any())
            {
                answersAnalysis.ChapterWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedChaperQuestions)
                {
                    if (group.Level != null)
                    {
                        var chaptersData = chapters.First(x => x.Id == group.Level.Value);
                        answersAnalysis.ChapterWiseAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = chaptersData.Name,
                            QuestionsCount = groupedChaperQuestions.FirstOrDefault(g => g.Level.Value == chaptersData.Id)?.Count ?? 0
                        });
                    }
                }
            }
            var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedTopicQuestions.Any())
            {
                answersAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedTopicQuestions)
                {
                    if (group.Level != null)
                    {
                        var topicsData = topics.First(x => x.Id == group.Level.Value);
                        answersAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = topicsData.Name,
                            QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
                        });
                    }
                }
            }

            return answersAnalysis;
        }

        private TopicAnalysisDto GetTopicAnalysis(List<AssessmentSessionQuestion> questions, List<Topic> topics)
        {
            TopicAnalysisDto topicAnalysis = new TopicAnalysisDto();
            topicAnalysis.DifficultyLevelAnalysis = GetDifficultyLevelAnalysis(questions);

            var groupedTopicQuestions = questions.GroupBy(q => q.TopicId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });

            if (groupedTopicQuestions.Any())
            {
                topicAnalysis.TopicWiseAnalysis = new List<ItemWiseAnalysisDto>();
                foreach (var group in groupedTopicQuestions)
                {
                    if (group.Level != null)
                    {
                        var topicsData = topics.First(x => x.Id == group.Level.Value);
                        topicAnalysis.TopicWiseAnalysis.Add(new ItemWiseAnalysisDto()
                        {
                            Name = topicsData.Name,
                            QuestionsCount = groupedTopicQuestions.FirstOrDefault(g => g.Level.Value == topicsData.Id)?.Count ?? 0
                        });
                    }
                }
            }

            return topicAnalysis;
        }

        private Course GetCourse(Student student)
        {
            return dbContext.Courses.First(x => x.Id == student.CourseId);
        }
        private List<AssessmentSessionQuestion> GetQuestionsByStudentId(long studentId)
        {
            List<AssessmentSessionQuestion> questions = new List<AssessmentSessionQuestion>();

            if (dbContext.AssessmentSessions.Any(item => item.StudentId == studentId))
            {
                var sessions = dbContext.AssessmentSessions.Where(item => item.StudentId == studentId).ToList();

                foreach (var session in sessions)
                {
                    questions.AddRange(dbContext.AssessmentSessionQuestions.Where(x => x.AssessmentSessionId == session.Id && x.Result != null).ToList());
                }
            }
            return RemoveDuplicateQuestions(questions);
        }
        private List<AssessmentSessionQuestion> GetQuestionsBySubjectId(long subjectId)
        {
            List<AssessmentSessionQuestion> questions = new List<AssessmentSessionQuestion>();
            var subject = dbContext.Subjects.FirstOrDefault(x => x.Id == subjectId);
            if (subject != null)
            {
                var chapters = dbContext.Chapters.Where(item => item.SubjectId == subjectId).ToList();

                foreach (var chapter in chapters)
                {
                    questions.AddRange(dbContext.AssessmentSessionQuestions.Where(x => x.ChapterId == chapter.Id && x.Result != null).ToList());
                }
            }
            return RemoveDuplicateQuestions(questions);
        }
        public List<AssessmentSessionQuestion> RemoveDuplicateQuestions(List<AssessmentSessionQuestion> objects)
        {
            var distinctObjects = objects
                .GroupBy(obj => obj.QuestionId)
                .Select(group => group.OrderByDescending(obj => obj.Id).First())
                .ToList();

            return distinctObjects;
        }
        private List<Subject> GetSubjects(List<AssessmentSessionQuestion> questions)
        {
            List<Subject> subjects = new List<Subject>();

            if (questions != null && questions.Count > 0)
            {
                var chapterIds = questions.Select(x=>x.ChapterId).Distinct().ToList();
                if(chapterIds != null && chapterIds.Count > 0)
                {
                    var chapters = dbContext.Chapters.Where(x => chapterIds.Contains(x.Id)).ToList();
                    foreach (var chapter in chapters)
                    {
                        var subject = dbContext.Subjects.Where(x => x.Id == chapter.SubjectId).FirstOrDefault();
                        if (subject != null && !subjects.Any(x => x.Id == subject.Id))
                            subjects.Add(subject);
                    }
                }
            }
            return subjects;
        }
        private List<Chapter> GetChapters(List<AssessmentSessionQuestion> questions)
        {
            List<Chapter> chapters = new List<Chapter>();

            if (questions != null && questions.Count > 0)
            {
                var chapterIds = questions.Select(x => x.ChapterId).Distinct().ToList();
                if (chapterIds != null && chapterIds.Count > 0)
                {
                    chapters.AddRange(dbContext.Chapters.Where(x => chapterIds.Contains(x.Id)).ToList());
                }
            }
            return chapters;
        }
        private List<Topic> GetTopics(List<AssessmentSessionQuestion> questions)
        {
            List<Topic> topics = new List<Topic>();

            if (questions != null && questions.Count > 0)
            {
                var topicIds = questions.Select(x => x.TopicId).Distinct().ToList();
                if (topicIds != null && topicIds.Count > 0)
                {
                    topics.AddRange(dbContext.Topics.Where(x => topicIds.Contains(x.Id)).ToList());
                }
            }
            return topics;
        }
        private List<ItemWiseAnalysisDto> GetDifficultyLevelAnalysis(List<AssessmentSessionQuestion> questions)
        {
            List<ItemWiseAnalysisDto> difficultyLevelAnalysis = new List<ItemWiseAnalysisDto>();
            var difficultyLevels = dbContext.DifficultyLevels.ToList();

            var groupedDifficultyLevelQuestions = questions.GroupBy(q => q.DifficultyLevelId)
                                    .Select(g => new
                                    {
                                        Level = g.Key,
                                        Count = g.Count()
                                    });
            //if (groupedDifficultyLevelQuestions.Any())
            //{
            //    answersAnalysis.DifficultyLevelAnalysis = new List<ItemWiseAnalysisDto>();
            //    foreach (var group in groupedDifficultyLevelQuestions)
            //    {
            //        if (group.Level != null)
            //        {
            //            var difficultylevel = difficultyLevels.First(x => x.Id == group.Level.Value);
            //            answersAnalysis.DifficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
            //            {
            //                Name = difficultylevel.Name,
            //                QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultylevel.Id)?.Count ?? 0
            //            });
            //        }
            //    }
            //}

            if (groupedDifficultyLevelQuestions.Any())
            {
                difficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
                {
                    Name = "Easy",
                    QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Easy").Id)?.Count ?? 0

                });
                difficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
                {
                    Name = "Medium",
                    QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Medium").Id)?.Count ?? 0
                });

                difficultyLevelAnalysis.Add(new ItemWiseAnalysisDto()
                {
                    Name = "Difficult",
                    QuestionsCount = groupedDifficultyLevelQuestions.FirstOrDefault(g => g.Level.Value == difficultyLevels.First(x => x.Name == "Difficult").Id)?.Count ?? 0
                });
            }

            return difficultyLevelAnalysis;
        }

        public async Task<TrackYourProgressReportDto> GetTrackYourProgressReport(long id)
        {
            TrackYourProgressReportDto report = new TrackYourProgressReportDto();

            report.ChapterWiseTest = new ChapterWiseTestDto();
            report.ChapterWiseTest.Title = "CWT";
            report.ChapterWiseTest.Course = "NEET";
            report.ChapterWiseTest.Class = "Class 11";
            report.ChapterWiseTest.Subject = "Botany";
            report.ChapterWiseTest.TotalQuestions = 400;
            report.ChapterWiseTest.TotalQuestionsAttempted = 350;
            report.ChapterWiseTest.TotalCorrect = 280;
            report.ChapterWiseTest.TotalWrong = 120;
            report.ChapterWiseTest.CorrectAnswersPercentage = 70;
            report.ChapterWiseTest.WrongAnswersPercentage = 30;

            report.ChapterWiseTest.ChapterReports = new List<ItemWiseReportDto>();
            ItemWiseReportDto chapterWiseReport1 = new ItemWiseReportDto();
            chapterWiseReport1.Name = "Chapter 1";
            chapterWiseReport1.LevelReports = new List<AssessmnetLevelReportDto>()
            {
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 14,
                    TotalWrong = 6,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 2",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 13,
                    TotalWrong = 7,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 3",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 19,
                    TotalWrong = 1,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 1",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 2",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                }
            };

            report.ChapterWiseTest.ChapterReports.Add(chapterWiseReport1);

            ItemWiseReportDto chapterWiseReport2 = new ItemWiseReportDto();
            chapterWiseReport2.Name = "Chapter 2";
            chapterWiseReport2.LevelReports = new List<AssessmnetLevelReportDto>()
            {
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 14,
                    TotalWrong = 6,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 2",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 13,
                    TotalWrong = 7,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 3",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 19,
                    TotalWrong = 1,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 16,
                    TotalWrong = 4,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 2",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                }
            };

            report.ChapterWiseTest.ChapterReports.Add(chapterWiseReport2);

            report.TopicWiseTest = new TopicWiseTestDto();
            report.TopicWiseTest.Title = "TWT";
            report.TopicWiseTest.Course = "NEET";
            report.TopicWiseTest.Class = "Class 11";
            report.TopicWiseTest.Subject = "Botany";
            report.TopicWiseTest.Chapter = "Chapter 1";
            report.TopicWiseTest.TotalQuestions = 400;
            report.TopicWiseTest.TotalQuestionsAttempted = 350;
            report.TopicWiseTest.TotalCorrect = 280;
            report.TopicWiseTest.TotalWrong = 120;
            report.TopicWiseTest.CorrectAnswersPercentage = 70;
            report.TopicWiseTest.WrongAnswersPercentage = 30;

            report.TopicWiseTest.ChapterReports = new List<ItemWiseReportDto>();
            ItemWiseReportDto topicWiseReport1 = new ItemWiseReportDto();
            topicWiseReport1.Name = "Topic 1";
            topicWiseReport1.LevelReports = new List<AssessmnetLevelReportDto>()
            {
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 14,
                    TotalWrong = 6,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 2",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 13,
                    TotalWrong = 7,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 3",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 19,
                    TotalWrong = 1,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 1",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 2",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                }
            };

            report.ChapterWiseTest.ChapterReports.Add(chapterWiseReport1);

            ItemWiseReportDto topicWiseReport2 = new ItemWiseReportDto();
            topicWiseReport2.Name = "Topic 2";
            chapterWiseReport2.LevelReports = new List<AssessmnetLevelReportDto>()
            {
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 14,
                    TotalWrong = 6,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 2",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 13,
                    TotalWrong = 7,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 3",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 19,
                    TotalWrong = 1,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 16,
                    TotalWrong = 4,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 2",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                }
            };

            report.QuickPracticeTest = new QuickPracticeTestDto();
            report.QuickPracticeTest.Title = "QPT";
            report.QuickPracticeTest.Course = "NEET";
            report.QuickPracticeTest.Class = "Class 11";
            report.QuickPracticeTest.Subject = "Botany";
            report.QuickPracticeTest.Chapter = "Chapter 1";
            report.QuickPracticeTest.Topic = "Topic 1";
            report.QuickPracticeTest.TotalQuestions = 400;
            report.QuickPracticeTest.TotalQuestionsAttempted = 350;
            report.QuickPracticeTest.TotalCorrect = 280;
            report.QuickPracticeTest.TotalWrong = 120;
            report.QuickPracticeTest.CorrectAnswersPercentage = 70;
            report.QuickPracticeTest.WrongAnswersPercentage = 30;

            report.QuickPracticeTest.ChapterReports = new List<ItemWiseReportDto>();
            ItemWiseReportDto quickPracticeTestReport1 = new ItemWiseReportDto();
            quickPracticeTestReport1.Name = "Sub-Topic 1";
            quickPracticeTestReport1.LevelReports = new List<AssessmnetLevelReportDto>()
            {
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 14,
                    TotalWrong = 6,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 2",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 13,
                    TotalWrong = 7,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 3",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 19,
                    TotalWrong = 1,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 1",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 2",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                }
            };

            report.QuickPracticeTest.ChapterReports.Add(chapterWiseReport1);

            ItemWiseReportDto quickPracticeTestReport2 = new ItemWiseReportDto();
            quickPracticeTestReport2.Name = "Sub-Topic 2";
            quickPracticeTestReport2.LevelReports = new List<AssessmnetLevelReportDto>()
            {
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 14,
                    TotalWrong = 6,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 2",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 13,
                    TotalWrong = 7,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "Level 3",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 19,
                    TotalWrong = 1,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 1",
                    Status = "Completed",
                    TotalQuestions = 20,
                    TotalCorrect = 16,
                    TotalWrong = 4,
                },
                new AssessmnetLevelReportDto()
                {
                    Name = "PYQ 2",
                    Status = "Pending",
                    TotalQuestions = 0,
                    TotalCorrect = 0,
                    TotalWrong = 0,
                }
            };

            return report;
        }

        public async Task<TrackYourProgressReportDto> GetTimeSpentAnalysisReport(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<TrackYourProgressReportDto> GetDifficultyLevelAnalysisReport(long id)
        {
            throw new NotImplementedException();
        }
    }
}

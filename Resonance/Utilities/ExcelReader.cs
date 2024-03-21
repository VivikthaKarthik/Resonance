using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;
using System.Data;

namespace ResoClassAPI.Utilities
{
    public class ExcelReader : IExcelReader
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IAuthService authService;
        private readonly ICommonService commonService;

        public ExcelReader(IWebHostEnvironment _hostingEnvironment, IAuthService _authService
            ,ICommonService _commonService)
        {
            hostingEnvironment = _hostingEnvironment;
            authService = _authService;
            commonService = _commonService;
        }

        public async Task<bool> BulkUpload(IFormFile file, string tableName)
        {
            bool isUploaded = false;
            string filePath = string.Empty;
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var foreignKeyColumns = await commonService.GetForeignKeyColumns(tableName);
                    DataTable dataTable = ReadExcelToDataTable(stream, tableName, foreignKeyColumns);
                    if (dataTable != null && dataTable.Rows.Count > 0)
                        isUploaded = await commonService.SaveDataToDatabase(tableName, dataTable, foreignKeyColumns);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return isUploaded;
        }

        private DataTable ReadExcelToDataTable(Stream stream, string tableName, List<string> foreignKeyColumns)
        {
            var currentUser = authService.GetCurrentUser();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dataTable = new DataTable();

                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    if (foreignKeyColumns != null && foreignKeyColumns.Contains(firstRowCell.Text.Trim() + "Id"))
                        dataTable.Columns.Add(firstRowCell.Text.Trim() + "Id");
                    else
                        dataTable.Columns.Add(firstRowCell.Text.Trim());
                }

                dataTable.Columns.Add("IsActive");
                dataTable.Columns.Add("CreatedBy");
                dataTable.Columns.Add("CreatedOn");
                dataTable.Columns.Add("ModifiedBy");
                dataTable.Columns.Add("ModifiedOn");

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    DataRow newRow = dataTable.Rows.Add();

                    foreach (var cell in row)
                    {
                        string columnName = dataTable.Columns[cell.Start.Column - 1].ColumnName;
                        string cellValue = cell.Text.Trim();

                        if (tableName == "Chapter" && columnName == "IsRecommended")
                        {
                            // Convert Excel boolean value to database-compatible format
                            bool isActive = cellValue.Equals(cellValue, StringComparison.OrdinalIgnoreCase);
                            newRow["IsRecommended"] = isActive;
                        }
                        else
                        {
                            newRow[cell.Start.Column - 1] = cellValue;
                        }
                    }

                    newRow["IsActive"] = true;
                    newRow["CreatedBy"] = currentUser.Name;
                    newRow["CreatedOn"] = DateTime.Now.ToString();
                    newRow["ModifiedBy"] = currentUser.Name;
                    newRow["ModifiedOn"] = DateTime.Now.ToString();
                }

                return dataTable;
            }
        }

        public async Task<List<CourseDto>> ReadCoursesFromExcel(Stream stream)
        {
            List<CourseDto> courses = new List<CourseDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string courseName = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 2].Text.Trim();

                    if (!string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(courseName))
                        courses.Add(new CourseDto { Name = courseName, Thumbnail = thumbnail });
                }
            }

            return courses;
        }

        public async Task<List<ClassDto>> ReadClassesFromExcel(Stream stream)
        {
            List<ClassDto> classes = new List<ClassDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string className = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string courseName = worksheet.Cells[rowNumber, 3].Text.Trim();

                    if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(courseName))
                        classes.Add(new ClassDto { Name = className, Thumbnail = thumbnail, Course = courseName });
                }
            }

            return classes;
        }

        public async Task<List<SubjectDto>> ReadSubjectsFromExcel(Stream stream)
        {
            List<SubjectDto> subjects = new List<SubjectDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string subjectName = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string className = worksheet.Cells[rowNumber, 3].Text.Trim();
                    string courseName = worksheet.Cells[rowNumber, 4].Text.Trim();
                    string colorCode = worksheet.Cells[rowNumber, 5].Text.Trim();

                    if (!string.IsNullOrEmpty(subjectName) && !string.IsNullOrEmpty(courseName))
                        subjects.Add(new SubjectDto { Name = subjectName, Thumbnail = thumbnail, ClassName = className, CourseName = courseName, ColorCode = colorCode });
                }
            }

            return subjects;
        }

        public async Task<List<StudentProfileDto>> ReadStudentsFromExcel(Stream stream)
        {
            List<StudentProfileDto> students = new List<StudentProfileDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string AdmissionId = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string AdmissionDate = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string Name = worksheet.Cells[rowNumber, 3].Text.Trim();
                    string FatherName = worksheet.Cells[rowNumber, 4].Text.Trim();
                    string MotherName = worksheet.Cells[rowNumber, 5].Text.Trim();
                    string DateOfBirth = worksheet.Cells[rowNumber, 6].Text.Trim();
                    string AddressLine1 = worksheet.Cells[rowNumber, 7].Text.Trim();
                    string AddressLine2 = worksheet.Cells[rowNumber, 8].Text.Trim();
                    string Landmark = worksheet.Cells[rowNumber, 9].Text.Trim();
                    string City = worksheet.Cells[rowNumber, 10].Text.Trim();
                    string State = worksheet.Cells[rowNumber, 11].Text.Trim();
                    string PinCode = worksheet.Cells[rowNumber, 12].Text.Trim();
                    string Gender = worksheet.Cells[rowNumber, 13].Text.Trim();
                    string Class = worksheet.Cells[rowNumber, 14].Text.Trim();
                    string Course = worksheet.Cells[rowNumber, 15].Text.Trim();
                    string MobileNumber = worksheet.Cells[rowNumber, 16].Text.Trim();
                    string AlternateMobileNumber = worksheet.Cells[rowNumber, 17].Text.Trim();
                    string EmailAddress = worksheet.Cells[rowNumber, 18].Text.Trim();
                    string ProfilePicture = worksheet.Cells[rowNumber, 19].Text.Trim();

                    if (!string.IsNullOrEmpty(AdmissionId))
                    {
                        var student = new StudentProfileDto();
                        student.AdmissionId = AdmissionId;
                        if (!string.IsNullOrEmpty(AdmissionDate))
                            student.AdmissionDate = Convert.ToDateTime(AdmissionDate);
                        student.Name = Name;
                        student.FatherName = FatherName;
                        student.MotherName = MotherName;
                        if (!string.IsNullOrEmpty(DateOfBirth))
                            student.DateOfBirth = Convert.ToDateTime(DateOfBirth);
                        student.AddressLine1 = AddressLine1;
                        student.AddressLine2 = AddressLine2;
                        student.Landmark = Landmark;
                        student.City = City;
                        student.State = State;
                        student.PinCode = PinCode;
                        student.Gender = Gender;
                        student.CourseName = Course;
                        student.ClassName = Class;
                        student.MobileNumber = MobileNumber;
                        student.AlternateMobileNumber = AlternateMobileNumber;
                        student.EmailAddress = EmailAddress;
                        student.ProfilePicture = ProfilePicture;

                        students.Add(student);
                    }
                }
            }

            return students;
        }

        public async Task<List<ChapterExcelRequestDto>> ReadChaptersFromExcel(Stream stream)
        {
            List<ChapterExcelRequestDto> chapters = new List<ChapterExcelRequestDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string chapterName = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string subject = worksheet.Cells[rowNumber, 3].Text.Trim();
                    string Class = worksheet.Cells[rowNumber, 4].Text.Trim();
                    string course = worksheet.Cells[rowNumber, 5].Text.Trim();
                    string isRecommended = worksheet.Cells[rowNumber, 6].Text.Trim();
                    string description = worksheet.Cells[rowNumber, 7].Text.Trim();

                    if (!string.IsNullOrEmpty(chapterName) && !string.IsNullOrEmpty(course))
                        chapters.Add(new ChapterExcelRequestDto { Name = chapterName, Description = description, Thumbnail = thumbnail, Class = Class, Course = course, Subject = subject, IsRecommended = (isRecommended == "1" || isRecommended.ToLower() == "true") });
                }
            }

            return chapters;
        }

        public async Task<List<TopicExcelRequestDto>> ReadTopicsFromExcel(Stream stream)
        {
            List<TopicExcelRequestDto> topics = new List<TopicExcelRequestDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string name = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string chapter = worksheet.Cells[rowNumber, 3].Text.Trim();
                    string subject = worksheet.Cells[rowNumber, 4].Text.Trim();
                    string Class = worksheet.Cells[rowNumber, 5].Text.Trim();
                    string course = worksheet.Cells[rowNumber, 6].Text.Trim();
                    string description = worksheet.Cells[rowNumber, 7].Text.Trim();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(course))
                        topics.Add(new TopicExcelRequestDto { Name = name, Description = description, Thumbnail = thumbnail, Chapter = chapter, Class = Class, Course = course, Subject = subject });
                }
            }

            return topics;
        }

        public async Task<List<SubTopicExcelRequestDto>> ReadSubTopicsFromExcel(Stream stream)
        {
            List<SubTopicExcelRequestDto> topics = new List<SubTopicExcelRequestDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string name = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string description = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 3].Text.Trim();
                    string sourceUrl = worksheet.Cells[rowNumber, 4].Text.Trim();
                    string topic = worksheet.Cells[rowNumber, 5].Text.Trim();
                    string chapter = worksheet.Cells[rowNumber, 6].Text.Trim();
                    string subject = worksheet.Cells[rowNumber, 7].Text.Trim();
                    string Class = worksheet.Cells[rowNumber, 8].Text.Trim();
                    string course = worksheet.Cells[rowNumber, 9].Text.Trim();
                    string homeDisplay = worksheet.Cells[rowNumber, 10].Text.Trim();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(course))
                        topics.Add(new SubTopicExcelRequestDto { Name = name, SourceUrl = sourceUrl, Topic = topic, 
                            Description = description, Thumbnail = thumbnail, Chapter = chapter, Class = Class, 
                            Course = course, Subject = subject, HomeDisplay = (homeDisplay == "1" || homeDisplay.ToLower() == "true") });
                }
            }

            return topics;
        }

        public async Task<List<VideoExcelRequestDto>> ReadVideosFromExcel(Stream stream)
        {
            List<VideoExcelRequestDto> videos = new List<VideoExcelRequestDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    string title = worksheet.Cells[rowNumber, 1].Text.Trim();
                    string description = worksheet.Cells[rowNumber, 2].Text.Trim();
                    string thumbnail = worksheet.Cells[rowNumber, 3].Text.Trim();
                    string sourceUrl = worksheet.Cells[rowNumber, 4].Text.Trim();
                    string subTopic = worksheet.Cells[rowNumber, 5].Text.Trim();
                    string topic = worksheet.Cells[rowNumber, 6].Text.Trim();
                    string chapter = worksheet.Cells[rowNumber, 7].Text.Trim();
                    string homeDisplay = worksheet.Cells[rowNumber, 8].Text.Trim();
                    string subject = worksheet.Cells[rowNumber, 9].Text.Trim();
                    string course = worksheet.Cells[rowNumber, 10].Text.Trim();

                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(sourceUrl))
                    {
                        videos.Add(new VideoExcelRequestDto
                        {
                            Title = title,
                            Description = description,
                            Thumbnail = thumbnail,
                            SourceUrl = sourceUrl,
                            SubTopic = subTopic,
                            Topic = topic,
                            Chapter = chapter,
                            HomeDisplay = !string.IsNullOrEmpty(homeDisplay) && homeDisplay.ToLower() == "true",
                            Course = course,
                            Subject = subject
                        });
                    }
                }
            }

            return videos;
        }

    }
}

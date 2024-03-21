CREATE OR ALTER VIEW [dbo].[vwClasses]
AS

Select Cl.ID, Cl.[Name], Co.[Name] AS Course from [Class] Cl
Inner Join [Course] Co On Co.Id = Cl.CourseId
Where Cl.IsActive = 1
GO

CREATE OR ALTER VIEW [dbo].[vwSubjects]
AS

Select S.ID, S.[Name], Cl.[Name] AS Class, Co.[Name] AS Course, S.ColorCode from [Subject] S
Inner Join [Class] Cl On Cl.Id = S.ClassId
Inner Join [Course] Co On Co.Id = Cl.CourseId
Where S.IsActive = 1
GO

CREATE OR ALTER VIEW vwChapters
AS

Select Ch.ID, Ch.[Name], S.[Name] AS [Subject], Cl.[Name] AS Class, Co.[Name] AS Course, Ch.Thumbnail, Ch.[Description], Ch.IsRecommended from Chapter Ch
Inner Join [Subject] S On S.ID = Ch.SubjectId
Inner Join [Class] Cl On Cl.Id = S.ClassId
Inner Join [Course] Co On Co.Id = Cl.CourseId
Where Ch.IsActive = 1
GO

CREATE OR ALTER VIEW vwTopics
AS

Select T.ID, T.[Name], Ch.[Name] AS Chapter, S.[Name] AS [Subject], Cl.[Name] AS Class, Co.[Name] AS Course, T.[Description], T.Thumbnail from Topic T
Inner Join Chapter Ch On Ch.ID = T.ChapterId
Inner Join [Subject] S On S.ID = Ch.SubjectId
Inner Join [Class] Cl On Cl.Id = S.ClassId
Inner Join [Course] Co On Co.Id = Cl.CourseId
Where T.IsActive = 1
GO


CREATE OR ALTER VIEW vwSubTopics
AS

Select ST.ID, ST.[Name], ST.[Description],ST.Thumbnail, ST.SourceUrl,ST.HomeDisplay, T.[Name] AS Topic, Ch.[Name] AS Chapter, S.[Name] AS [Subject], Cl.[Name] AS Class, Co.[Name] AS Course from SubTopic ST
Left Join Topic T On T.ID = ST.TopicId
Inner Join Chapter Ch On Ch.ID = T.ChapterId
Inner Join [Subject] S On S.ID = Ch.SubjectId
Inner Join [Class] Cl On Cl.Id = S.ClassId
Inner Join [Course] Co On Co.Id = Cl.CourseId
Where ST.IsActive = 1

UNION

Select ST.ID, ST.[Name], ST.[Description],ST.Thumbnail, ST.SourceUrl,ST.HomeDisplay, '' AS Topic, Ch.[Name] AS Chapter, S.[Name] AS [Subject], Cl.[Name] AS Class, Co.[Name] AS Course from SubTopic ST
Inner Join Chapter Ch On Ch.ID = ST.ChapterId
Inner Join [Subject] S On S.ID = Ch.SubjectId
Inner Join [Class] Cl On Cl.Id = S.ClassId
Inner Join [Course] Co On Co.Id = Cl.CourseId
Where ST.IsActive = 1 AND ST.TopicId IS NULL

GO
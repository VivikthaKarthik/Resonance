
CREATE TABLE [dbo].[AssessmentConfiguration](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[CourseId] [bigint] NOT NULL,
	[MaximumQuestions] [int] NOT NULL,
	[MarksPerQuestion] [int] NOT NULL,
	[HasNegativeMarking] [bit] NOT NULL,
	[NegativeMarksPerQuestion] [int] NULL,
	[PassMarkks] [int] NOT NULL,
	[TimeDuration] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](250) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](250) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_AssessmentConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssessmentConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentConfiguration_Course] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Course] ([Id])
GO

ALTER TABLE [dbo].[AssessmentConfiguration] CHECK CONSTRAINT [FK_AssessmentConfiguration_Course]
GO



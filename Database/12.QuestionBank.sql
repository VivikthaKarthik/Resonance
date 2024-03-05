
CREATE TABLE [dbo].[QuestionBank](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[Question] [nvarchar](max) NOT NULL,
	[FirstAnswer] [nvarchar](max) NOT NULL,
	[SecondAnswer] [nvarchar](max) NOT NULL,
	[ThirdAnswer] [nvarchar](max) NOT NULL,
	[FourthAnswer] [nvarchar](max) NOT NULL,
	[CorrectAnswer] [nvarchar](max) NOT NULL,
	[Explanation] [nvarchar](max) NULL,
	[DifficultyLevelId] [bigint] NULL,
	[SubTopicId] [bigint] NULL,
	[TopicId] [bigint] NULL,
	[ChapterId] [bigint] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_QuestionBank] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuestionBank]  WITH CHECK ADD  CONSTRAINT [FK_QuestionBank_Chapter] FOREIGN KEY([ChapterId])
REFERENCES [dbo].[Chapter] ([Id])
GO

ALTER TABLE [dbo].[QuestionBank] CHECK CONSTRAINT [FK_QuestionBank_Chapter]
GO

ALTER TABLE [dbo].[QuestionBank]  WITH CHECK ADD  CONSTRAINT [FK_QuestionBank_SubTopic] FOREIGN KEY([SubTopicId])
REFERENCES [dbo].[SubTopic] ([Id])
GO

ALTER TABLE [dbo].[QuestionBank] CHECK CONSTRAINT [FK_QuestionBank_SubTopic]
GO

ALTER TABLE [dbo].[QuestionBank]  WITH CHECK ADD  CONSTRAINT [FK_QuestionBank_Topic] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topic] ([Id])
GO

ALTER TABLE [dbo].[QuestionBank] CHECK CONSTRAINT [FK_QuestionBank_Topic]
GO



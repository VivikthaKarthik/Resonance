
CREATE TABLE [dbo].[MultipleChoiceQuestion](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[FirstChoiceId] [bigint] NOT NULL,
	[SecondChoiceId] [bigint] NOT NULL,
	[ThirdChoiceId] [bigint] NOT NULL,
	[FourthChoiceId] [bigint] NOT NULL,
	[CorrectChoiceId] [bigint] NOT NULL,
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
 CONSTRAINT [PK_MultipleChoiceQuestion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Chapter] FOREIGN KEY([ChapterId])
REFERENCES [dbo].[Chapter] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Chapter]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Choice] FOREIGN KEY([FirstChoiceId])
REFERENCES [dbo].[Choice] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Choice]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Choice1] FOREIGN KEY([SecondChoiceId])
REFERENCES [dbo].[Choice] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Choice1]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Choice2] FOREIGN KEY([ThirdChoiceId])
REFERENCES [dbo].[Choice] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Choice2]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Choice3] FOREIGN KEY([FourthChoiceId])
REFERENCES [dbo].[Choice] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Choice3]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Choice4] FOREIGN KEY([CorrectChoiceId])
REFERENCES [dbo].[Choice] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Choice4]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Question] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Question] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Question]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_SubTopic] FOREIGN KEY([SubTopicId])
REFERENCES [dbo].[SubTopic] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_SubTopic]
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion]  WITH CHECK ADD  CONSTRAINT [FK_MultipleChoiceQuestion_Topic] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topic] ([Id])
GO

ALTER TABLE [dbo].[MultipleChoiceQuestion] CHECK CONSTRAINT [FK_MultipleChoiceQuestion_Topic]
GO



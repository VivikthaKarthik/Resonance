
CREATE TABLE [dbo].[AssessmentSession_Questions](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[AssessmentSessionId] [bigint] NOT NULL,
	[QuestionId] [bigint] NOT NULL,
	[Result] [bit] NULL,
 CONSTRAINT [PK_AssessmentSession_Questions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssessmentSession_Questions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSession_Questions_AssessmentSession1] FOREIGN KEY([AssessmentSessionId])
REFERENCES [dbo].[AssessmentSession] ([Id])
GO

ALTER TABLE [dbo].[AssessmentSession_Questions] CHECK CONSTRAINT [FK_AssessmentSession_Questions_AssessmentSession1]
GO

ALTER TABLE [dbo].[AssessmentSession_Questions]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSession_Questions_QuestionBank] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[QuestionBank] ([Id])
GO

ALTER TABLE [dbo].[AssessmentSession_Questions] CHECK CONSTRAINT [FK_AssessmentSession_Questions_QuestionBank]
GO



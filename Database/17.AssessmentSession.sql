
CREATE TABLE [dbo].[AssessmentSession](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[StudentId] [bigint] NOT NULL,
	[AssessmentType] [nvarchar](50) NOT NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Result] [bit] NULL,
 CONSTRAINT [PK_AssessmentSession] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssessmentSession]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentSession_AssessmentSession] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([Id])
GO

ALTER TABLE [dbo].[AssessmentSession] CHECK CONSTRAINT [FK_AssessmentSession_AssessmentSession]
GO



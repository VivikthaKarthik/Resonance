
CREATE TABLE [dbo].[Video](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[ThumbNail] [nvarchar](max) NOT NULL,
	[SourceUrl] [nvarchar](max) NOT NULL,
	[SubTopicId] [bigint] NULL,
	[TopicId] [bigint] NULL,
	[ChapterId] [bigint] NULL,
	[HomeDisplay] [nvarchar](5) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Video]  WITH NOCHECK ADD  CONSTRAINT [FK_Video_Chapter] FOREIGN KEY([ChapterId])
REFERENCES [dbo].[Chapter] ([Id])
GO

ALTER TABLE [dbo].[Video] CHECK CONSTRAINT [FK_Video_Chapter]
GO

ALTER TABLE [dbo].[Video]  WITH NOCHECK ADD  CONSTRAINT [FK_Video_SubTopic] FOREIGN KEY([SubTopicId])
REFERENCES [dbo].[SubTopic] ([Id])
GO

ALTER TABLE [dbo].[Video] CHECK CONSTRAINT [FK_Video_SubTopic]
GO

ALTER TABLE [dbo].[Video]  WITH NOCHECK ADD  CONSTRAINT [FK_Video_Topic] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topic] ([Id])
GO

ALTER TABLE [dbo].[Video] CHECK CONSTRAINT [FK_Video_Topic]
GO



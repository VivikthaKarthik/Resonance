
CREATE TABLE [dbo].[SubTopic](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Thumbnail] [nvarchar](max) NOT NULL,
	[SourceUrl] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[HomeDisplay] [bit] NOT NULL,
	[TopicId] [bigint] NULL,
	[ChapterId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_SubTopic] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SubTopic]  WITH CHECK ADD  CONSTRAINT [FK_SubTopic_Topic] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topic] ([Id])
GO

ALTER TABLE [dbo].[SubTopic] CHECK CONSTRAINT [FK_SubTopic_Topic]
GO



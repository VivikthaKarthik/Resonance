
CREATE TABLE [dbo].[Subject](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CourseId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Subject]  WITH CHECK ADD  CONSTRAINT [FK_Subject_Course] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Course] ([Id])
GO

ALTER TABLE [dbo].[Subject] CHECK CONSTRAINT [FK_Subject_Course]
GO



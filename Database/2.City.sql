
CREATE TABLE [dbo].[City](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[StateId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](250) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](250) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_City] FOREIGN KEY([Id])
REFERENCES [dbo].[City] ([Id])
GO

ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_City]
GO

ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([Id])
GO

ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_State]
GO




CREATE TABLE [dbo].[User](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[LastLoginDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_ResoUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[User] ADD  CONSTRAINT [df_ResoUser]  DEFAULT ('Resonance@123') FOR [Password]
GO

ALTER TABLE [dbo].[User]  WITH NOCHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO

ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO



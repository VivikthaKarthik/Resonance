
CREATE TABLE [dbo].[Student](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[AdmissionId] [nvarchar](128) NOT NULL,
	[AdmissionDate] [datetime] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[FatherName] [nvarchar](128) NOT NULL,
	[MotherName] [nvarchar](128) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[AddressLine1] [nvarchar](max) NOT NULL,
	[AddressLine2] [nvarchar](max) NULL,
	[Landmark] [nvarchar](max) NULL,
	[CityId] [bigint] NOT NULL,
	[StateId] [bigint] NOT NULL,
	[PinCode] [nvarchar](10) NOT NULL,
	[BranchId] [nchar](10) NULL,
	[Gender] [nvarchar](10) NOT NULL,
	[CourseId] [bigint] NOT NULL,
	[MobileNumber] [nvarchar](15) NOT NULL,
	[AlternateMobileNumber] [nvarchar](15) NULL,
	[EmailAddress] [nvarchar](80) NOT NULL,
	[DeviceId] [nvarchar](max) NULL,
	[FirebaseId] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Longitude] [nvarchar](50) NULL,
	[Latitude] [nvarchar](50) NULL,
	[LastLoginDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](128) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[IsPasswordChangeRequired] [bit] NULL,
	[ProfilePicture] [nvarchar](max) NULL,
 CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF__Student__IsPassw__308E3499]  DEFAULT ((1)) FOR [IsPasswordChangeRequired]
GO

ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Student_City] FOREIGN KEY([CityId])
REFERENCES [dbo].[City] ([Id])
GO

ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK_Student_City]
GO

ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Student_State] FOREIGN KEY([StateId])
REFERENCES [dbo].[State] ([Id])
GO

ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK_Student_State]
GO

ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Student_Student] FOREIGN KEY([Id])
REFERENCES [dbo].[Student] ([Id])
GO

ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK_Student_Student]
GO



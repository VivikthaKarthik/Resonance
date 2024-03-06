
CREATE TABLE [dbo].[Audit](
	[Id] [bigint] IDENTITY(1000001,1) NOT NULL,
	[TableName] [nvarchar](128) NOT NULL,
	[RecordId] [bigint] NOT NULL,
	[ColumnName] [nvarchar](128) NOT NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



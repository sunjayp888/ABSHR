USE [HR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Overtime')
BEGIN

	CREATE TABLE [dbo].Overtime(
		[OvertimeId] [int] IDENTITY(1,1) NOT NULL,
		[OrganisationId] [int] NOT NULL FOREIGN KEY REFERENCES Organisation(OrganisationId),
		[OvertimeStateId] [int] NOT NULL FOREIGN KEY REFERENCES OvertimeState(OvertimeStateId) DEFAULT 1,
		[OvertimePreferenceId] [int] NOT NULL FOREIGN KEY REFERENCES OvertimePreference(OvertimePreferenceId),
		[PersonnelId] [int] NOT NULL FOREIGN KEY REFERENCES Personnel(PersonnelId),
		[Name] [nvarchar](100) NOT NULL,
		[Date] [date] NOT NULL,
		[Hours] [float] NOT NULL,
		[Reason] [nvarchar](255) NOT NULL,
		[Comment] [nvarchar](255) NULL,
		[CreatedBy] [nvarchar](50) NULL,
		[CreatedDateUtc] [datetime] NULL DEFAULT GETUTCDATE(),
		[UpdatedBy] [nvarchar](50) NULL,
		[UpdatedDateUtc] [datetime] NULL,
	 CONSTRAINT [PK_Overtime] PRIMARY KEY CLUSTERED 
	(
		[OvertimeId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
END
USE [HR]
GO

/****** Object:  Table [dbo].[CountryAbsenceType]    Script Date: 03/11/2016 19:16:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CountryAbsenceType](
	[CountryAbsenceTypeId] [int] IDENTITY(1,1) NOT NULL,
	[CountryId] [int] NOT NULL,
	[AbsenceTypeId] [int] NOT NULL,
 CONSTRAINT [PK_CountryAbsenceType] PRIMARY KEY CLUSTERED 
(
	[CountryAbsenceTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CountryAbsenceType]  WITH CHECK ADD  CONSTRAINT [FK_CountryAbsenceType_AbsenceType] FOREIGN KEY([AbsenceTypeId])
REFERENCES [dbo].[AbsenceType] ([AbsenceTypeId])
GO

ALTER TABLE [dbo].[CountryAbsenceType] CHECK CONSTRAINT [FK_CountryAbsenceType_AbsenceType]
GO

ALTER TABLE [dbo].[CountryAbsenceType]  WITH CHECK ADD  CONSTRAINT [FK_CountryAbsenceType_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([CountryId])
GO

ALTER TABLE [dbo].[CountryAbsenceType] CHECK CONSTRAINT [FK_CountryAbsenceType_Country]
GO



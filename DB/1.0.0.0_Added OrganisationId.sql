USE HR
GO

ALTER TABLE [Country] ADD [OrganisationId] int
GO

update [Country] set [OrganisationId] = 1
GO

ALTER TABLE [Country] Alter column [OrganisationId] int NOT NULL
GO

ALTER TABLE [dbo].[Country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisation] ([OrganisationId])
GO

ALTER TABLE [dbo].[Country] CHECK CONSTRAINT [FK_Country_Organisation]
GO


ALTER TABLE [CountryAbsenceType] ADD [OrganisationId] int
GO

update [CountryAbsenceType] set [OrganisationId] = 1
GO

ALTER TABLE [CountryAbsenceType] Alter column [OrganisationId] int NOT NULL
GO

ALTER TABLE [dbo].[CountryAbsenceType]  WITH CHECK ADD  CONSTRAINT [FK_CountryAbsenceType_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisation] ([OrganisationId])
GO

ALTER TABLE [dbo].[CountryAbsenceType] CHECK CONSTRAINT [FK_CountryAbsenceType_Organisation]
GO


ALTER TABLE [CountryPublicHoliday] ADD [OrganisationId] int
GO

update [CountryPublicHoliday] set [OrganisationId] = 1
GO

ALTER TABLE [CountryPublicHoliday] Alter column [OrganisationId] int NOT NULL
GO

ALTER TABLE [dbo].[CountryPublicHoliday]  WITH CHECK ADD  CONSTRAINT [FK_CountryPublicHoliday_Organisation] FOREIGN KEY([OrganisationId])
REFERENCES [dbo].[Organisation] ([OrganisationId])
GO

ALTER TABLE [dbo].[CountryPublicHoliday] CHECK CONSTRAINT [FK_CountryPublicHoliday_Organisation]
GO
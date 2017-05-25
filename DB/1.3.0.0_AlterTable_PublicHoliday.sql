USE [HR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PublicHoliday' AND COLUMN_NAME = 'OrganisationId')
BEGIN
	ALTER TABLE PublicHoliday
	Add OrganisationId Int
	EXECUTE('UPDATE ph SET OrganisationId = c.OrganisationId FROM PublicHoliday ph INNER JOIN CountryPublicHoliday cph ON ph.PublicHolidayId = cph.CountryPublicHolidayId INNER JOIN Country c on cph.CountryId = c.CountryId')
END

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PublicHoliday' AND COLUMN_NAME = 'PublicHolidayPolicyId')
BEGIN
	ALTER TABLE PublicHoliday
	Add PublicHolidayPolicyId Int
	EXECUTE('UPDATE PublicHoliday SET PublicHolidayPolicyId = 0')
END
USE [HR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PersonnelAbsenceEntitlement' AND COLUMN_NAME = 'DivisionCountryAbsencePeriodId')
BEGIN
	ALTER TABLE PersonnelAbsenceEntitlement
	DROP CONSTRAINT  FK_PersonnelAbsenceEntitlement_DivisionCountryAbsencePeriod

	ALTER TABLE PersonnelAbsenceEntitlement
	DROP COLUMN DivisionCountryAbsencePeriodId
END
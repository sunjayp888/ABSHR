USE [HR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'PublicHolidayPolicyId')
BEGIN
	ALTER TABLE Employment
	ADD  PublicHolidayPolicyId INT
	EXECUTE('UPDATE Employment SET PublicHolidayPolicyId = 0')
END

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'AbsencePolicyId')
BEGIN
	ALTER TABLE Employment
	ADD AbsencePolicyId INT
	EXECUTE('UPDATE Employment SET AbsencePolicyId = 0')
END

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'CompanyId')
BEGIN
	ALTER TABLE Employment
	ADD CompanyId INT
	EXECUTE('UPDATE e SET e.CompanyId = d.CompanyId FROM Employment e INNER JOIN Division d ON e.DivisionId = d.DivisionId')
END

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'EmploymentTypeId')
BEGIN
	ALTER TABLE Employment
	ADD EmploymentTypeId INT FOREIGN KEY REFERENCES EmploymentType(EmploymentTypeId)
	EXECUTE('UPDATE e SET e.EmploymentTypeId = et.EmploymentTypeId FROM Employment e INNER JOIN EmploymentType et ON e.OrganisationId = et.OrganisationId')
END

IF EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'JobTitle')
BEGIN
	ALTER TABLE Employment
	Drop Column JobTitle
END

IF EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'JobDesription')
BEGIN
	ALTER TABLE Employment
	Drop Column JobDesription
END

IF EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'DivisionId')
BEGIN
	IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'FK_Employment_Division' and type='F')
	BEGIN
		ALTER table Employment drop constraint  FK_Employment_Division
	END
	ALTER TABLE Employment
	Drop Column DivisionID
END

IF EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'DepartmentId')
BEGIN
	IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'FK_Employment_Department' and type='F')
	BEGIN
		ALTER table Employment drop constraint  FK_Employment_Department
	END
	ALTER TABLE Employment
	Drop Column DepartmentId
END

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'JobTitleId')
BEGIN
	ALTER TABLE Employment
	ADD JobTitleId INT
END

IF NOT EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employment' AND COLUMN_NAME = 'JobGradeId')
BEGIN
	ALTER TABLE Employment
	ADD JobGradeId INT
END
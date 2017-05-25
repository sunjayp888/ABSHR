USE [HR]

GO
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Personnel' AND COLUMN_NAME = 'CurrentEmploymentId')
BEGIN
	ALTER TABLE [dbo].[Personnel] ADD CurrentEmploymentId 
	int CONSTRAINT FK_Personnel_EmploymentId FOREIGN KEY(CurrentEmploymentId) REFERENCES [dbo].[Employment](EmploymentId)
END

GO
UPDATE Personnel
SET
	CurrentEmploymentId = (SELECT TOP 1 EmploymentId FROM Employment WHERE Employment.PersonnelId = Personnel.PersonnelId AND EndDate IS NULL AND TerminationDate IS NULL ORDER BY StartDate DESC)
WHERE CurrentEmploymentId IS NULL

GO
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PersonnelAbsenceEntitlement' AND COLUMN_NAME = 'EmploymentId')
BEGIN
	ALTER TABLE [dbo].[PersonnelAbsenceEntitlement] ADD EmploymentId 
	int CONSTRAINT FK_PersonnelAbsenceEntitlement_EmploymentId 
	FOREIGN KEY(EmploymentId) REFERENCES [dbo].[Employment](EmploymentId)
END

GO
UPDATE PersonnelAbsenceEntitlement
SET
	EmploymentId = (SELECT CurrentEmploymentId FROM Personnel WHERE Personnel.PersonnelId = PersonnelAbsenceEntitlement.PersonnelId)
WHERE
	EmploymentId IS NULL

GO
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PersonnelAbsenceEntitlement' AND COLUMN_NAME = 'EmploymentId' AND IS_NULLABLE = 'NO')
BEGIN
	ALTER TABLE [PersonnelAbsenceEntitlement] ALTER COLUMN EmploymentId INT NOT NULL
END
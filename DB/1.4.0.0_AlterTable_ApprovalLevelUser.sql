USE [HR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ApprovalLevelUser' AND COLUMN_NAME = 'AspNetUserId' AND DATA_TYPE = 'int')
BEGIN
	ALTER TABLE ApprovalLevelUser
	ALTER COLUMN AspNetUserId nvarchar(128)
END

--Will not work because we have to remove then return the foreign keys
--IF NOT EXISTS (SELECT o.name, c.name FROM sys.objects o inner join sys.columns c on o.object_id = c.object_id WHERE o.name ='ApprovalLevelUser' AND c.name = 'ApprovalLevelUserId' AND c.is_identity = 1)
--BEGIN
--	ALTER TABLE ApprovalLevelUser
--	DROP COLUMN ApprovalLevelUserId
	
--	ALTER TABLE ApprovalLevelUser
--	ADD ApprovalLevelUserId Int Identity(1, 1)
	
--	ALTER TABLE ApprovalLevelUser
--	ADD PRIMARY KEY (ApprovalLevelUserId)
--END
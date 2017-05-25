USE [HR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(SELECT 1  FROM ApprovalState)
BEGIN
	EXECUTE('INSERT INTO ApprovalState (Name) VALUES (''Requested''),(''In Approval''),(''Approved''),(''Declines'')')
END
USE [HR]
GO

/****** Object:  View [dbo].[DivisionDetails]    Script Date: 10/10/2016 20:10:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[DivisionDetails]
AS 
SELECT
	O.OrganisationId,
	O.Name [OrganisationName],
	C.CompanyId [CompanyId],
	C.name [CompanyName],
	D.DivisionId,
	D.Name [DivisionName],
	D.HolidayEntitlement,
	D.MaximumHolidayCarryOver
FROM
	Organisation O
INNER JOIN
	Company C
		ON O.OrganisationId = C.OrganisationId
INNER JOIN
	 Division D
		ON C.CompanyId = D.CompanyId


GO



USE HR

GO
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AbsenceType' AND COLUMN_NAME = 'Colour')
BEGIN
	ALTER TABLE AbsenceType
	DROP COLUMN Colour
END

GO
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AbsenceType' AND COLUMN_NAME = 'ColourId')
BEGIN
	ALTER TABLE AbsenceType
	ADD ColourId int FOREIGN KEY REFERENCES Colour(ColourId)
END


-----------------   Once you have data in the ColourId columns change column to not null
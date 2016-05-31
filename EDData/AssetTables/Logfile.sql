CREATE TABLE [EDDATA].[Logfile]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProfileID] INT NOT NULL, 
    [LogfileName] NVARCHAR(50) NOT NULL, 
	[LogFileDate] DateTime NOT NULL,
    [LastParsed] DATETIME NOT NULL, 
    [LinesParsed] INT NOT NULL,
	CONSTRAINT FK_Logfiles_Asset FOREIGN KEY (Id) REFERENCES [EDDATA].[Asset] (Id),
	CONSTRAINT FK_Logfiles_Profile FOREIGN KEY (ProfileID) REFERENCES [EDDATA].[Profile] (Id)
)

/*
Journeys group together consecutive history items to identify specific trips
*/
CREATE TABLE [dbo].[Journey]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProfileID] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [HistoryStartID] INT NULL, 
    [HistoryEndID] INT NULL,
	CONSTRAINT FK_Journey_Asset FOREIGN KEY (Id) REFERENCES [EDDATA].Asset (Id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_Journey_Profile FOREIGN KEY (ProfileID) REFERENCES [EDDATA].[Profile] (Id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_Journey_Start FOREIGN KEY (HistoryStartID) REFERENCES [EDDATA].[History] (Id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_Journey_End FOREIGN KEY (HistoryEndID) REFERENCES [EDDATA].[History] (Id) ON DELETE CASCADE ON UPDATE CASCADE     
)

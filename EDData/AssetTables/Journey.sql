/*
Journeys group together consecutive history items to identify specific trips
*/
CREATE TABLE [EDData].[Journey]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProfileID] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [HistoryStartID] INT NULL, 
    [HistoryEndID] INT NULL,
	CONSTRAINT FK_Journey_Asset FOREIGN KEY (Id) REFERENCES [EDDATA].Asset (Id),
	CONSTRAINT FK_Journey_Profile FOREIGN KEY (ProfileID) REFERENCES [EDDATA].[Profile] (Id),
	CONSTRAINT FK_Journey_Start FOREIGN KEY (HistoryStartID) REFERENCES [EDDATA].[History] (Id),
	CONSTRAINT FK_Journey_End FOREIGN KEY (HistoryEndID) REFERENCES [EDDATA].[History] (Id)    
)

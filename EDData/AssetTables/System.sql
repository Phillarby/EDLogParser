﻿CREATE TABLE [EDDATA].[System]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [X] FLOAT NULL, 
    [Y] NCHAR(10) NULL, 
    [Z] NCHAR(10) NULL, 
    [Added] DATETIME NULL,
	[Validated] DATETIME NULL,
	CONSTRAINT FK_System_Asset FOREIGN KEY (Id) REFERENCES [EDDATA].[Asset] (Id)
)
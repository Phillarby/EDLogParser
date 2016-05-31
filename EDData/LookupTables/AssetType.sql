CREATE TABLE Lookup.[AssetType]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [SortOrder] INT NOT NULL, 
    [Active] BIT NOT NULL
)

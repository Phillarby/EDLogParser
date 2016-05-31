/*
- An asset is any consequential data item in the system.  Application logs, Lookup vaues etc are not assets.
- The asset table is a central repository with metadata and organisation information for all assets.  
- The IDs in this table are provide globally unique identifiers and are common across all asset types
- Parent identifier provides a parent-child tree organisation of assets.  I'm using this as it is a 
  simpler organisation than having a graph of multiple relationships - We'll see if this will suffice
*/
CREATE TABLE [EDDATA].[Asset]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[ItemTypeID] INT NOT NULL,
	[Description] NVARCHAR(250),
	[Created] DateTime, -- Galactic time
    [Deleted] BIT NULL, 
    [ParentID] INT NULL
)

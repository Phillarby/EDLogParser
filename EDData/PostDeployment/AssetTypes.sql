
--Reference Data for AddressType 
MERGE INTO [Lookup].[AssetType] AS Target 
USING ( VALUES 
	(1,	'Profile',	1,	1),
	(2, 'LogFile',	2,	1), 
	(3, 'System',	3,	1),
	(4, 'History',	4,	1),  
	(5, 'Journey',	5,	1) 
) 
AS Source (Id, Name, SortOrder, Active) 
ON Target.Id = Source.Id 

--update matched rows 
WHEN MATCHED THEN 
UPDATE SET Name = Source.Name 

--insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (Id, Name, SortOrder, Active) 
VALUES (Id, Name, SortOrder, Active) 

--delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
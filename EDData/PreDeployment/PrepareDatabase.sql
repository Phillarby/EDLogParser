DELETE FROM [EDData].[Profile]
DELETE FROM [EDData].[Asset]
DBCC CHECKIDENT ('[EDData].[Asset]', RESEED, 0);
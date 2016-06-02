CREATE PROCEDURE [EDData].[AddLogFile]
	@Id int OUTPUT,
	@ProfileID INT, 
    @Filename VARCHAR(50), 
	@FileDate DateTime, 
    @LastParse DateTime, 
    @LinesParsed INT
    
AS
	BEGIN TRY
		BEGIN TRAN
		
			DECLARE @LogFileAssetType INT,
					@Created DateTime
			SELECT  @LogFileAssetType = 2, 
					@Created = GetDate()
		
			--Create a new Asset to associated with the new log file
			EXEC EDData.AddAsset 
				@Id = @Id OUTPUT, 
				@AssetTypeID = @LogFileAssetType,  
				@Description = @Filename, 
				@ParentID = @ProfileID

			--Create logfile Record
			INSERT INTO [EDDATA].[LogFile] (
				[Id],
				[ProfileID], 
				[LogFileDate],
				[LastParsed],
				[LinesParsed]
			) VALUES (
				@Id,
				@ProfileID, 
				@FileName, 
				@FileDate,
				@LastParse,
				@LinesParsed
			)

		COMMIT TRAN
	END TRY

	BEGIN CATCH
	IF @@TRANCOUNT > 0 
		ROLLBACK
	END CATCH
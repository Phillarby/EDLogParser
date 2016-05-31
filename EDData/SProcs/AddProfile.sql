CREATE PROCEDURE [EDDATA].[AddProfile]
	@Id int OUTPUT,
	@SecurityToken VARCHAR(36) OUTPUT, 
    @TokenExpiry DATETIME OUTPUT, 
	@Commander NVARCHAR(150), 
    @Email NVARCHAR(150), 
    @Password NVARCHAR(256)
    
AS
	BEGIN TRY
		BEGIN TRAN
		
			DECLARE @ProfileAssetType INT,
					@Created DateTime
			SELECT  @ProfileAssetType = 1, 
			        @SecurityToken = NewID(),
					@Created = GetDate(),
					@TokenExpiry = DATEADD(d, 2, @Created)
		
			--Create a new Asset to associated with the new profile
			EXEC EDData.AddAsset 
				@Id = @Id OUTPUT, 
				@AssetTypeID = @ProfileAssetType,  
				@Description = @Commander, 
				@ParentID = Null

			--Create Profile Record
			INSERT INTO [EDDATA].[Profile] (
				[Id],
				[Commander], 
				[Email], 
				[Password], 
				[SecurityToken], 
				[TokenExpiry]
			) VALUES (
				@Id,
				@Commander,
				@Email,
				@Password,
				@SecurityToken,
				@TokenExpiry
			)

		COMMIT TRAN
	END TRY

	BEGIN CATCH
	IF @@TRANCOUNT > 0 
		ROLLBACK
	END CATCH

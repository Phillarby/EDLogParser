CREATE PROCEDURE [EDDATA].[AddProfile]
	@Id int OUTPUT,
	@SecurityToken NVARCHAR OUTPUT, 
    @TokenExpiry DATETIME OUTPUT, 
	@Commander NVARCHAR(150), 
    @Email NVARCHAR(150), 
    @Password NVARCHAR(256)
    
AS
	BEGIN TRY
		BEGIN TRAN
		
			DECLARE @ProfileAssetType INT;
			SELECT  @ProfileAssetType = 1, 
			        @SecurityToken = NewID(),
					@TokenExpiry = DATEADD(d, 2, GETDATE());
		
			--Create a new Asset to associated with the new profile
			EXEC AddAsset @Id OUTPUT, @ProfileAssetType,  @Commander, GetDate, Null

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
				@SecurityToken,
				@TokenExpiry
			)

		COMMIT
	END TRY

	BEGIN CATCH
	IF @@TRANCOUNT > 0 
		ROLLBACK
	END CATCH
RETURN 0

SELECT NEWID
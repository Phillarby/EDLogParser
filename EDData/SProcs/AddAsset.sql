CREATE PROCEDURE [EDData].[AddAsset]
	@ID INT OUTPUT,
	@AssetTypeID INT,
	@Description NVARCHAR(250) = NULL,
	@ParentID INT = NULL
AS
	INSERT INTO [EDDATA].Asset (
		[ItemTypeID],
		[Description],
		[Created],
		[Deleted],
		[ParentID]
	) VALUES (
		@AssetTypeID,
		@Description,
		GETDATE(),
		0, --Assume assets will not be created as deleted
		@ParentID)

SELECT @ID = SCOPE_IDENTITY() --Get the ID of the created asset item


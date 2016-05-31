CREATE PROCEDURE [dbo].[AddAsset]
	@ID INT OUTPUT,
	@AssetTypeID INT,
	@Description NVARCHAR(250) = NULL,
	@Created DateTime, -- Galactic time
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
		@Created,
		0, --Assume assets will not be created as deleted
		@ParentID)

SELECT @ID = SCOPE_IDENTITY() --Get the ID of the created asset item


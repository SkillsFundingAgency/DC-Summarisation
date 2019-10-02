CREATE PROCEDURE [dbo].[usp_GetClosedCollectionEvents]
           @closedCollectionsSince DATETIME        
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			[Id]
			,[CollectionType]
			,[CollectionReturnCode]
			,[DateTime]
		FROM [dbo].[CollectionReturn]
		WHERE [DateTime] > COALESCE(@closedCollectionsSince, CAST('1753-1-1' AS DATETIME))
	END TRY
	BEGIN CATCH
		DECLARE   @ErrorMessage		NVARCHAR(4000)
				, @ErrorSeverity	INT 
				, @ErrorState		INT
				, @ErrorNumber		INT
						
		SELECT	  @ErrorNumber		= ERROR_NUMBER()
				, @ErrorMessage		= 'Error in :' + ISNULL(OBJECT_NAME(@@PROCID),'') + ' - Error was :' + ERROR_MESSAGE()
				, @ErrorSeverity	= ERROR_SEVERITY()
				, @ErrorState		= ERROR_STATE();
	
		RAISERROR (
					  @ErrorMessage		-- Message text.
					, @ErrorSeverity	-- Severity.
					, @ErrorState		-- State.
				  );
			  
		RETURN @ErrorNumber;
	END CATCH
END
GO
GRANT EXECUTE ON [dbo].[usp_GetClosedCollectionEvents] TO [DataViewing];
GO
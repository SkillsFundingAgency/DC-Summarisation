CREATE PROCEDURE [dbo].[usp_GetSummarisedActuals]
	@CollectionType VARCHAR(20),
	@CollectionReturnCode VARCHAR(10),
	@PageSize INT = 1,
	@PageNumber INT = 1000
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			NEWID() as Id
			,cr.[CollectionReturnCode]
			,sa.[OrganisationId]
			,sa.[PeriodTypeCode]
			,sa.[Period]
			,sa.[FundingStreamPeriodCode]
			,cr.[CollectionType]
			,sa.[ContractAllocationNumber]
			,sa.[UoPCode]
			,sa.[DeliverableCode]
			,sa.[ActualVolume]
			,sa.[ActualValue]
		FROM 
			[dbo].[SummarisedActuals] sa
		LEFT JOIN
			[dbo].CollectionReturn cr
		ON
			sa.CollectionReturnId = cr.Id	
		WHERE 
			CollectionReturnCode = @CollectionReturnCode
		AND 
			CollectionType = @CollectionType
		ORDER BY 
			sa.Id
		OFFSET @PageSize * (@PageNumber -1) ROWS
		FETCH NEXT @PageSize ROWS ONLY
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
GRANT EXECUTE ON [dbo].[usp_GetSummarisedActuals] TO [DataViewing];
GO
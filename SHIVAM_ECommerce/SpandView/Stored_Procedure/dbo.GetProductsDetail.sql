
GO

/****** Object:  StoredProcedure [dbo].[GetProductsDetail]    Script Date: 9/26/2018 3:19:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetProductsDetail]
	 @SupplierId nvarchar(10)
AS
BEGIN
	--QUERY
DECLARE @DynamicPivotQuery AS NVARCHAR(MAX);
DECLARE @ColumnNamesInPivot AS NVARCHAR(MAX);

--Get distinct values of PIVOT Column 
SELECT   @ColumnNamesInPivot = ISNULL(@ColumnNamesInPivot + ',', '')
        + QUOTENAME([AttributeName])
FROM    ( SELECT    DISTINCT
                    [AttributeName]
          FROM      ProductValues_view where SupplierID=@SupplierId
        ) AS P



SELECT  @DynamicPivotQuery = N'Select productName,Cost,Quantity,SupplierName,'
        + @ColumnNamesInPivot + ' 
            FROM    ( SELECT * 
          FROM      ProductValues_view where SupplierID='+@SupplierId+'
        ) AS SourceTable PIVOT( MAX(AttributeValue) FOR [AttributeName] IN ('
        + @ColumnNamesInPivot + ') ) AS PVTTable'

EXEC sp_executesql @DynamicPivotQuery;
END

GO



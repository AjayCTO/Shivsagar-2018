Create FUNCTION [dbo].[ExecuteStringAsQuery]()
RETURNS Varchar(8000)
AS
BEGIN
DECLARE @SQLQuery AS NVARCHAR(500);
DECLARE @RESULT AS NVARCHAR(500);
--QUERY
DECLARE @DynamicPivotQuery AS NVARCHAR(MAX);
DECLARE @ColumnNamesInPivot AS NVARCHAR(MAX);
SET @DynamicPivotQuery='select * from Products';
SET @RESULT=@DynamicPivotQuery;
return @RESULT
END

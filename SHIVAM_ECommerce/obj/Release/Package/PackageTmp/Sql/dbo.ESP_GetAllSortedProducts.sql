CREATE Proc [dbo].[SP_GetAllSortedProducts]
@DisplayLength as Int,
@DisplayStart as Int,
@SortCol as Nvarchar(100),
@SortDir as Nvarchar(10),
@SearchText Nvarchar(255) =NULL,
@SupplierId as nvarchar(10)
As
Begin
DECLARE @FirstRec int,@LastRec int;
DECLARE @DynamicPivotQuery AS NVARCHAR(MAX);
DECLARE @ColumnNamesInPivot AS NVARCHAR(MAX);
DECLARE @WhereClause as NVARCHAR(MAX);
DECLARE @AttributeWhereClause as NVARCHAR(MAX);

Set @FirstRec=@DisplayStart;
Set @LastRec=@DisplayStart + @DisplayLength;
SET @WhereClause='';
SET @AttributeWhereClause='';

--Get distinct values of PIVOT Column 
SELECT   @ColumnNamesInPivot = ISNULL(@ColumnNamesInPivot + ',', '')
        + QUOTENAME([AttributeName])
FROM    ( SELECT    DISTINCT
                    [AttributeName]
          FROM      ProductValues_view where SupplierID=@SupplierId
        ) AS P;
 
 
 IF @SearchText IS NOT NULL
BEGIN
  SET @WhereClause=' Or productName like ''%'+@SearchText +'%''
                               or Cost like ''%'+ @SearchText+'%'' 
                               or Quantity like ''%'+ @SearchText+'%''
                               or SupplierName like ''%'+ @SearchText+'%''';
                               
                               
DECLARE @valueList varchar(8000)
DECLARE @pos INT
DECLARE @len INT
DECLARE @value varchar(8000)
DECLARE @Query varchar(8000)
SET @valueList = @ColumnNamesInPivot;

--the value list string must end with a comma ','
--so, if the last comma it's not there, the following IF will add a trailing comma to the value list
IF @valueList NOT LIKE '%,'
BEGIN
    set @valueList = @valueList + ',';
END


set @pos = 0;
set @len = 0;
set @AttributeWhereClause='('''+@SearchText +''' IS NULL'+ @WhereClause;

WHILE CHARINDEX(',', @valueList, @pos+1)>0
BEGIN
    set @len = CHARINDEX(',', @valueList, @pos+1) - @pos
    set @value = SUBSTRING(@valueList, @pos, @len)
    --SELECT @pos, @len, @value /*this is here for debugging*/
        
    
    SET @AttributeWhereClause=@AttributeWhereClause + ' Or ' + @value +' like ''%'+@SearchText +'%''';

    set @pos = CHARINDEX(',', @valueList, @pos+@len) +1
END
SET @AttributeWhereClause=@AttributeWhereClause + ') AND';
--PRINT @WhereClause                            
                               
END

set @Query='FROM(SELECT * FROM ProductValues_view '+CASE WHEN @SupplierId <>-1 THEN 'where SupplierID='+@SupplierId  ELSE '' END +')'

print @SupplierId
print @Query


SELECT  @DynamicPivotQuery = N'Select * from (Select Row_Number() over (order by '+@SortCol+' '+@SortDir+') as RowNum,Count(*) over() as TotalCount,ProductId,ProductQuantityID,ImagePath,productName,Cost,Quantity,SupplierName,ImageName,'+ @ColumnNamesInPivot + @Query+ 'AS SourceTable PIVOT( MAX(AttributeValue) FOR [AttributeName] IN ('+ @ColumnNamesInPivot + ') ) AS PVTTable ) as PP where '+@AttributeWhereClause + '(RowNum > '+convert(nvarchar,@FirstRec)+' AND RowNum<='+convert(nvarchar,@LastRec)+')';
print @DynamicPivotQuery;
EXEC sp_executesql @DynamicPivotQuery;
    
End






GO

/****** Object:  StoredProcedure [dbo].[SP_GetAllSortedProductsFrontFace]    Script Date: 9/26/2018 3:23:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [dbo].[SP_GetAllSortedProductsFrontFace]
@DisplayLength as Int,
@DisplayStart as Int,
@SortCol as Nvarchar(100),
@SortDir as Nvarchar(10),
@SearchText Nvarchar(255) =NULL,
@FilterText NVARCHAR(255)=Null,
@Categories Nvarchar(255)=null,
@LowPrice Nvarchar(255)=null,
@HighPrice Nvarchar(255)=null,
@IsFeatured Nvarchar(1)=null,
@ProductIds NVARCHAR(MAX)=null
As
Begin
DECLARE @FirstRec int,@LastRec int;
DECLARE @DynamicPivotQuery AS NVARCHAR(MAX);
DECLARE @ColumnNamesInPivot AS NVARCHAR(MAX);
DECLARE @WhereClause as NVARCHAR(MAX);
DECLARE @AttributeWhereClause as NVARCHAR(MAX);
DECLARE @catText as NVARCHAR(MAX);
DECLARE @PriceText as NVARCHAR(MAX);
DECLARE @FeaturedText as NVARCHAR(MAX);
DECLARE @ProductIDsText as NVARCHAR(MAX);

Set @FirstRec=@DisplayStart;
Set @LastRec=@DisplayStart + @DisplayLength;
SET @WhereClause='';
SET @AttributeWhereClause='';
SET @catText='';
SET @PriceText='';
SET @FeaturedText='';
SET @ProductIDsText='';

SET @ProductIds = ISNULL(@ProductIds, '')
--Get distinct values of PIVOT Column 
SELECT   @ColumnNamesInPivot = ISNULL(@ColumnNamesInPivot + ',', '')
        + QUOTENAME([AttributeName])
FROM    ( SELECT    DISTINCT
                    [AttributeName]
          FROM      ProductValues_view  where [AttributeName] is not null
        ) AS P;
                   
                   IF @Categories IS NOT NULL and @Categories!=''
BEGIN
  SET @catText=' CateogryID IN ('+@Categories +') AND ';  
  end
  print @catText
  
                   IF @IsFeatured IS NOT NULL and @IsFeatured!='0'
BEGIN
  SET @FeaturedText=' IsFeatured=1 AND ';  
  end
  print @FeaturedText
  
  IF @ProductIds<>''
  BEGIN
 SET @ProductIDsText=' ProductId IN('+@ProductIds+') AND'
 END  
   IF @LowPrice IS NOT NULL and @HighPrice!='' and  @HighPrice IS NOT NULL and @LowPrice!=''
BEGIN
  SET @PriceText=' Cost BETWEEN ('+@LowPrice+') AND ('+@HighPrice+')AND ';
  end
  print @PriceText
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
DECLARE @SupplierId as nvarchar(10)
SET @valueList = @ColumnNamesInPivot;
SET @SupplierId=-1;
--the value list string must end with a comma ','
--so, if the last comma it's not there, the following IF will add a trailing comma to the value list
IF @valueList NOT LIKE '%,'
BEGIN
    set @valueList = @valueList + ',';
END


set @pos = 0;
set @len = 0;
set @AttributeWhereClause='('''+@SearchText +''' IS NULL'+ @WhereClause;

print 'Attribute where clause'
print @AttributeWhereClause

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
print @ColumnNamesInPivot
print @FilterText
--SELECT  @DynamicPivotQuery = N'Select * from (Select Row_Number() over (order by '+@SortCol+' '+@SortDir+') as RowNum,Count(*) over() as TotalCount,ImagePath,productName,Cost,Quantity,SupplierName,ImageName,'+ @ColumnNamesInPivot + @Query+ 'AS SourceTable PIVOT( MAX(AttributeValue) FOR [AttributeName] IN ('+ @ColumnNamesInPivot + ') ) AS PVTTable ) as PP where '+@AttributeWhereClause + '(RowNum > '+convert(nvarchar,@FirstRec)+' AND RowNum<='+convert(nvarchar,@LastRec)+')';
--print @DynamicPivotQuery;

SELECT  @DynamicPivotQuery = N'Select * from (Select Row_Number() over (order by '+@SortCol+' '+@SortDir+') as RowNum,Count(*) over() as TotalCount,ImagePath,productName,Cost,Quantity,SupplierName,ImageName,Id as ProductId,CateogryID,IsFeatured,SupplierID,Description,'
        + @ColumnNamesInPivot + ' 
            FROM    ( SELECT * 
          FROM      ProductValues_view  
        ) AS SourceTable PIVOT( MAX(AttributeValue) FOR [AttributeName] IN ('
        + @ColumnNamesInPivot + ') ) AS PVTTable ) as PP where'+@AttributeWhereClause +' ProductId is not null AND '+@FilterText+ @catText+@PriceText+@FeaturedText+ @ProductIDsText+' (RowNum > '+convert(nvarchar,@FirstRec)+' AND RowNum<='+convert(nvarchar,@LastRec)+')';
	
	print @DynamicPivotQuery
EXEC sp_executesql @DynamicPivotQuery;
    
End

GO



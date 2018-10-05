CREATE PROCEDURE [dbo].[InsertProduct]
      @Name nvarchar(MAX),
	  @ProductCode nvarchar(MAX),
	  @Description nvarchar(MAX),
	  --@Quantity int,
	  @categoryName nvarchar(MAX),
	  --@cost float,
	  @UnitOfMeasure nvarchar(MAX),
	  @ManuFacturer nvarchar(MAX),
	  @SupplierID int,
	   @ProductID INT OUTPUT ,
 @categoryID INT = NULL ,
 @ManufacturerID INT = NULL,
 @UnitOfMeasureID INT = NULL 
 
AS
BEGIN
    SET NOCOUNT ON;

    

	SET @categoryID = (select TOP 1 Id 
                      from dbo.Categories c 
                      where LOWER(c.CategoryName) = LOWER(@categoryName))
					SET @categoryID=@categoryID

					  if @categoryID IS NULL
					  Begin
					   INSERT dbo.Categories(CategoryName,IsActive,CreatedDate,UpdatedDate,Sort,[Description],Notes,SupplierCategoryType,Discriminator) values(@categoryName,1,GETDATE(),GETDATE(),78,'','','','Category');
						SET @categoryID = SCOPE_IDENTITY();
						   End
Else
    Begin
	print @categoryID
	End
	SET @ManufacturerID = (select TOP 1 Id 
                      from dbo.Manufacturers c 
                      where LOWER(c.Name) = LOWER(@ManuFacturer))
					SET @ManufacturerID=@ManufacturerID
	
						  if @ManufacturerID IS NULL
					  Begin
					   INSERT dbo.Manufacturers(Code,Name,SupplierID,CreatedDate,UpdatedDate,Sort,[Description],Notes) values('',@ManuFacturer,@SupplierID,GETDATE(),GETDATE(),78,'','');
						SET @ManufacturerID = SCOPE_IDENTITY();
						   End
Else
    Begin
	print @ManufacturerID
	End
	
	SET @UnitOfMeasureID = (select TOP 1 Id 
                      from dbo.UnitOfMeasures c 
                      where LOWER(c.UnitOfMeasuresName) = LOWER(@UnitOfMeasure))
					SET @UnitOfMeasureID=@UnitOfMeasureID
	
						  if @UnitOfMeasureID IS NULL
					  Begin
					   INSERT dbo.UnitOfMeasures(UnitOfMeasuresName) values(@UnitOfMeasure);
						SET @UnitOfMeasureID = SCOPE_IDENTITY();
						   End
Else
    Begin
	print @UnitOfMeasureID
	End
	
							SET @ProductID = (select TOP 1 Id 
                      from dbo.Products c 
                      where LOWER(c.ProductName) = LOWER(@Name))
					SET @ProductID=@ProductID
	
						  if @ProductID IS NULL
						 Begin
						INSERT INTO [Products]
           ([ProductName]
           ,[ProductCode]
           ,[Ranking]
           ,[SKU]
           ,[IDSKU]
           ,[SupplierID]
           ,[ManuFacturerID]
           ,[CateogryID]
           ,[UnitOfMeasuresId]
           ,[CreatedDate]
           ,[UpdatedDate]
           ,[Sort]
           ,[Description]
           ,[Notes])
     VALUES
           (@Name,@ProductCode,'','','',@SupplierID,@ManufacturerID,@categoryID,@UnitOfMeasureID,GETDATE(),GETDATE(),'234',@Description,'')
           SET @ProductID = SCOPE_IDENTITY();
           End
Else
    Begin
	print @ProductID
	End
						
		return @ProductID			
END





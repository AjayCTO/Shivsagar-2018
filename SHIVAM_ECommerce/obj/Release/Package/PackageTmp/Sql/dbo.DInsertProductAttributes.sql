CREATE PROCEDURE [dbo].[InsertProductAttributes]
      @AttributeValues nvarchar(MAX),
	  @ProductPrice decimal,
	  @ProductQuantity decimal,
	  @ProductId int,
	  @UnitInStock decimal,
	  @UnitWeight decimal,
	  @HighQuantityThreshold decimal,
	  @LowQuantityThreshold decimal,
	  @ProductStatus nvarchar(MAX),
      @StatusID INT = NULL 
 
 
AS
BEGIN
    SET NOCOUNT ON;
    
    	SET @StatusID = (select TOP 1 Id 
                      from dbo.ProductStatus c 
                      where LOWER(c.Name) = LOWER(@ProductStatus))
					SET @StatusID=@StatusID

					  if @StatusID IS NULL
					  Begin
					   INSERT dbo.ProductStatus(Name,IsActive,CreatedDate,UpdatedDate,Sort,[Description],Notes) values(@ProductStatus,1,GETDATE(),GETDATE(),78,'','');
						SET @StatusID = SCOPE_IDENTITY();
						   End
Else
    Begin
	print @StatusID
	End

    INSERT INTO [ProductAttributeWithQuantities]
           ([AttributeValues]
           ,[IsAvailable]
           ,[Discount]
           ,[UnitPrice]
           ,[UnitWeight]
           ,[UnitInStock]
           ,[UnitsInOrder]
           ,[ProductQuantity]
           ,[ProductPrice]
           ,[Weight]
           ,[ProductId]
           ,[IsFeatured]
           ,[lowQuantityThreshold]
           ,[highQuantityThreshold]
           ,[StatusId]
           ,[IsActive]
           )
     VALUES
           (@AttributeValues
           ,1
           ,'0'
           ,@ProductPrice
           ,@UnitWeight
           ,@UnitInStock
           ,'0'
           ,@ProductQuantity
           ,@ProductPrice
           ,@UnitWeight
           ,@ProductId,0,@LowQuantityThreshold,@HighQuantityThreshold,@StatusID,1)
						
						

						
			
END





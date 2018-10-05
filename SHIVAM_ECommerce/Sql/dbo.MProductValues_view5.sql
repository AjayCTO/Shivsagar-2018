CREATE VIEW [dbo].[ProductValues_view]
AS
SELECT     dbo.ProductAttribute_view.AttributeValue, dbo.ProductAttribute_view.IsFeatured, dbo.ProductAttribute_view.Quantity, dbo.ProductAttribute_view.Id AS ProductQuantityID, 
                      dbo.ProductAttribute_view.Cost, dbo.ProductAttributes.AttributeName, dbo.ProductAttribute_view.Id, dbo.ProductAttributesRelations.SupplierID, dbo.ProductAttribute_view.productId, 
                      dbo.Products.ProductName,dbo.Products.Description, dbo.Suppliers.FirstName + ' ' + dbo.Suppliers.LastName AS SupplierName,
                          (SELECT     TOP (1) ImageName
                            FROM          dbo.ProductImages
                            WHERE      (dbo.ProductAttribute_view.Id = ProductQuantityId)) AS ImageName,
                          (SELECT     TOP (1) ImagePath
                            FROM          dbo.ProductImages AS ProductImages_2
                            WHERE      (dbo.ProductAttribute_view.Id = ProductQuantityId)) AS ImagePath, dbo.Products.CateogryID
FROM         dbo.Products INNER JOIN
                      dbo.ProductAttribute_view ON dbo.Products.Id = dbo.ProductAttribute_view.productId INNER JOIN
                      dbo.Suppliers ON dbo.Products.SupplierID = dbo.Suppliers.Id RIGHT OUTER JOIN
                      dbo.ProductAttributesRelations INNER JOIN
                      dbo.ProductAttributes ON dbo.ProductAttributesRelations.ProductAttributesId = dbo.ProductAttributes.Id ON 
                      dbo.ProductAttribute_view.AttributeName = dbo.ProductAttributesRelations.ProductAttributesId AND dbo.Products.SupplierID = dbo.ProductAttributesRelations.SupplierID



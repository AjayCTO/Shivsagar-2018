CREATE VIEW [dbo].[GetAttributes]
AS
SELECT DISTINCT TOP (100) PERCENT dbo.ProductAttribute_view.AttributeValue, dbo.ProductAttributes.AttributeName, dbo.ProductAttributes.Id
FROM         dbo.ProductAttributes INNER JOIN
                      dbo.ProductAttribute_view ON dbo.ProductAttributes.Id = CONVERT(numeric, dbo.ProductAttribute_view.AttributeName)
ORDER BY dbo.ProductAttributes.AttributeName



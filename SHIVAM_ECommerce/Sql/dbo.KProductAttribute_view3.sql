CREATE VIEW [dbo].[ProductAttribute_view]
AS
SELECT     (CASE WHEN S.Item LIKE '%@@%' THEN LEFT(S.Item, Charindex('@@', S.Item) - 1) ELSE S.Item END) AS AttributeName, (CASE WHEN S.Item LIKE '%@@%' THEN RIGHT(S.Item, Charindex('@@', 
                      Reverse(S.Item)) - 1) END) AS AttributeValue, productId, ProductQuantity AS Quantity, ProductPrice AS Cost, Id,IsFeatured
FROM         (SELECT     S.Item, p.productId, p.ProductQuantity, p.ProductPrice, p.Id,p.IsFeatured
                       FROM          [ProductAttributeWithQuantities] AS P CROSS apply dbo.SplitString(LEFT(P.AttributeValues, len(P.AttributeValues) - 2), '##') AS S) AS S
WHERE     S.Item <> ''




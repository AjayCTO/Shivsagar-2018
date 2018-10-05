
GO

/****** Object:  View [dbo].[ProductValues_view]    Script Date: 9/26/2018 3:27:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/
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

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[18] 4[15] 2[52] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Products"
            Begin Extent = 
               Top = 0
               Left = 692
               Bottom = 234
               Right = 868
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Suppliers"
            Begin Extent = 
               Top = 36
               Left = 1139
               Bottom = 156
               Right = 1309
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "ProductAttributesRelations"
            Begin Extent = 
               Top = 144
               Left = 390
               Bottom = 281
               Right = 574
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ProductAttributes"
            Begin Extent = 
               Top = 241
               Left = 17
               Bottom = 401
               Right = 202
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ProductAttribute_view"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 163
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2520
         Alias = 2355
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProductValues_view'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProductValues_view'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProductValues_view'
GO



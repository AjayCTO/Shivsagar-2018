
GO

/****** Object:  View [dbo].[GetAllUsersemail]    Script Date: 9/26/2018 3:25:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create VIEW [dbo].[GetAllUsersemail]
AS
SELECT A.[id],Split.a.value('.', 'VARCHAR(100)') AS String
 FROM  (SELECT *,  
         CAST ('<M>' + REPLACE([ReceiverId], ',', '</M><M>') + '</M>' AS XML) AS String  
     FROM  EmailSenders) AS A CROSS APPLY String.nodes ('/M') AS Split(a);

GO



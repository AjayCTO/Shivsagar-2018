create VIEW [dbo].[GetAllUsersemail]
AS
SELECT A.[id],Split.a.value('.', 'VARCHAR(100)') AS String
 FROM  (SELECT *,  
         CAST ('<M>' + REPLACE([ReceiverId], ',', '</M><M>') + '</M>' AS XML) AS String  
     FROM  EmailSenders) AS A CROSS APPLY String.nodes ('/M') AS Split(a);





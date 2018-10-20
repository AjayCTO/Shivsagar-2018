create VIEW [dbo].[GetAllEmails]
AS
SELECT     dbo.GetAllUsersemail.id, dbo.GetAllUsersemail.String AS userId, dbo.EmailSenders.SenderId, dbo.EmailSenders.Subject, dbo.EmailSenders.ContentMsg, dbo.EmailSenders.IsAttachment, 
                      dbo.EmailSenders.Attachment, dbo.EmailSenders.IsRead, dbo.EmailSenders.SendingDate, dbo.EmailSenders.ReceiverId
FROM         dbo.GetAllUsersemail INNER JOIN
                      dbo.EmailSenders ON dbo.GetAllUsersemail.id = dbo.EmailSenders.Id





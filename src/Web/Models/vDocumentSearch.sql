SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO


CREATE VIEW [dbo].[vDocumentSearch]
WITH SCHEMABINDING
AS
SELECT dbo.[Documents].Abstract, dbo.DocumentContents.[Content], dbo.[Documents].Title, dbo.[Documents].Id
FROM  dbo.[Documents] INNER JOIN
         dbo.DocumentContents ON dbo.[Documents].Id = dbo.DocumentContents.DocumentId


GO

/****** Object:  Index [IX_vDocumentSearch_Id]    Script Date: 5/11/2016 8:05:55 PM ******/
CREATE UNIQUE CLUSTERED INDEX [IX_vDocumentSearch_Id] ON [dbo].[vDocumentSearch]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



/****** Object:  FullTextCatalog [DocumentCatalog]    Script Date: 3/20/2016 8:50:29 PM ******/
CREATE FULLTEXT CATALOG [DocumentCatalog]WITH ACCENT_SENSITIVITY = ON

GO

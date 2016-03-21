SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vDocumentSearch]
WITH SCHEMABINDING
AS
SELECT dbo.[Document].Abstract, dbo.DocumentContent.[Content], dbo.[Document].Title, dbo.[Document].Id
FROM  dbo.[Document] INNER JOIN
         dbo.DocumentContent ON dbo.[Document].Id = dbo.DocumentContent.DocumentId


GO

/****** Object:  FullTextCatalog [DocumentCatalog]    Script Date: 3/20/2016 8:50:29 PM ******/
CREATE FULLTEXT CATALOG [DocumentCatalog]WITH ACCENT_SENSITIVITY = ON

GO
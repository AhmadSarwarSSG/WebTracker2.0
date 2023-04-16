CREATE VIEW [dbo].[SummedAction]
	AS SELECT AC.[FlowId], US.WebsiteId, CONCAT(AC.[Page],'[',STRING_AGG(CONCAT(AC.[Type],'(', AC.Content, ')'),'>'), ']') AS [Flowsummed]
	FROM [Actions] AS AC 
	Inner Join [Flows] AS FS
	ON AC.FlowId=FS.FlowId
	Inner Join [Users] AS US
	ON US.UserId=FS.UserId
	Group by US.WebsiteId, AC.FlowId, Ac.Page

CREATE VIEW [dbo].[SummedFlow]
	AS SELECT FD.[FlowId], US.WebsiteId, STRING_AGG(CONCAT(FD.[Page],''),'>') AS [Flowsummed]
	FROM [FlowDatas] AS FD 
	Inner Join [Flows] AS FS
	ON FD.FlowId=FS.FlowId
	Inner Join [Users] AS US
	ON US.UserId=FS.UserId
	Group by FD.FlowId, US.WebsiteId

CREATE VIEW [dbo].[ActionSummaryTable]
	AS SELECT [Flowsummed], [WebsiteId], COUNT([Flowsummed]) AS Count FROM [SummedAction]
	Group by Flowsummed, WebsiteId

CREATE VIEW [dbo].[SummaryTable]
	AS SELECT [Flowsummed], [WebsiteId], COUNT([Flowsummed]) AS Count FROM [SummedFlow]
	Group by Flowsummed, WebsiteId
async function testSQL(){
	let createResult = await SQL.execute("DB", `
		CREATE TABLE dbo.Products (
			ProductID int PRIMARY KEY NOT NULL, 
			ProductName varchar(25) NOT NULL, 
			Price money NULL, 
			ProductDescription text NULL
		)
	`);
	Test.assert(() => typeof createResult == typeof [], "SQL.execute create Table");
	
	let insertResult = await SQL.execute("DB", `
		INSERT dbo.Products (
			ProductID, 
			ProductName, 
			Price, 
			ProductDescription
		) VALUES (
			1, 
			'Clamp', 
			12.48, 
			'Workbench clamp'
		)
	`);
	Test.assert(() => typeof insertResult == typeof [], "SQL.execute Insert Data");
	
	let selectResult = await SQL.execute("DB", `
		SELECT 
			ProductID, 
			ProductName, 
			Price, 
			ProductDescription 
		FROM dbo.Products
	`);
	Test.assert(() => selectResult[0].ProductName == "Clamp", "SQL.execute Select Data");
	
	let dropResult = await SQL.execute("DB", "DROP TABLE IF EXISTS dbo.Products");
	Test.assert(() => typeof dropResult == typeof [], "SQL.execute drop table");
}
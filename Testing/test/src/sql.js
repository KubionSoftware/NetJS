// SQL execute
assert(() => typeof SQL.execute("DB", "CREATE TABLE dbo.Products (ProductID int PRIMARY KEY NOT NULL, " + 
"ProductName varchar(25) NOT NULL, Price money NULL, ProductDescription text NULL)") == typeof [], "SQL.execute create Table");

assert(() => typeof SQL.execute("DB", "INSERT dbo.Products (ProductID, ProductName, Price, ProductDescription)" + 
"VALUES (1, 'Clamp', 12.48, 'Workbench clamp')") == typeof [], "SQL.execute Insert Data");

assert(() => SQL.execute("DB", "SELECT ProductID, ProductName, Price, ProductDescription " +
"FROM dbo.Products")[0].ProductName == "Clamp", "SQL.execute Select Data");

assert(() => typeof SQL.execute("DB", "DROP TABLE IF EXISTS dbo.Products") == typeof [], "SQL.execute drop table");
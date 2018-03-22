# SQL
SQL class contains basic methods for communicating with SQL databases configured in the config.
_This class can make SELECT, UPDATE, INSERT and custom query's._
_Configuration is needed to make a DB connection._




## Configuration
To enable acces to an SQL database, the connection must be defined in a file in the root of the project named 'connections.json'.
Configuration structure:
```json
{"Data": {"type": "sql", "connectionString": "Server=example.com;Database=ExampleName;UserId=sa;Password=test"}}
```



## Index
**Functions:**
* [get(connectionName, table)](#get(connectionName,-table))

* [set(connectionName, table, id, info)](#set(connectionName,-table,-id,-info))

* [insert(connectionName, table, info)](#insert(connectionName,-table,-info))

* [execute(connectionName, query)](#execute(connectionName,-query))

**[Examples](#examples)**:
* [Query's](#Query's)




## Functions
### get(connectionName, table)
SQL.get takes a connectionName and a tablename and executes a SELECT * FROM [table] query which result will be returned.
Variable | Description
 ------------------------- | -------------------------
connectionName | Name of a configured connection
table | Tablename for SELECT statement
**Returns:**
Response from database, in json.
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in application scope._
* _**Exception**: Thrown when the type of the DB result can't be identified._




**Example:**
```javascript
var results = SQL.get("NetDB", "users");
```




### set(connectionName, table, id, info)
SQL.set takes a connectionName, tablename, id and a object and executes a UPDATE statement and returns a boolean (only true).
Variable | Description
 ------------------------- | -------------------------
connectionName | Name of a configured connection
table | Tablename for UPDATE statement
id | ID of row for the UPDATE statement
info | An object with information to be updated to
**Returns:**
A boolean (always true).
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in application scope._




**Example:**
```javascript
var results = SQL.set("NETDB", "users", 1, {name:"NetJS rules!"});
```




### insert(connectionName, table, info)
SQL.insert takes a connectionName, table and a object which will be inserted into the database. The last inserted id will be returned.
Variable | Description
 ------------------------- | -------------------------
connectionName | Name of a configured connection
table | Tablename for INSERT statement
info | An object with information to be updated to
**Returns:**
The ID of the last inserted row.
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in application scope._




**Example:**
```javascript
var id = SQL.insert("NETDB", "users", {name:"Hello World!"});
```




### execute(connectionName, query)
SQL.execute takes a connectionName and a query, executes the query and returns the result if the query is a SELECT statement.
Variable | Description
 ------------------------- | -------------------------
connectionName | Name of a configured connection
query | The query to be executed
**Returns:**
the result if the query is a SELECT statement.
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in application scope._
* _**Error**: Thrown when an error has been found while executing the query._




**Example:**
```javascript
var id = SQL.execute("NETDB", "SELECT * FROM users;");
```




## Examples
### Query's
This example expects to have the following connection in the configuration:
```json
{"Data": {"type": "sql", "connectionString": "Server=example.com;Database=ExampleName;UserId=sa;Password=test"}}
```
We can INSERT a new user and UPDATE his values:
```javascript
var db = "ExampleName";
var user = {name: "Hello World!", mail: "HelloWorld@example.com"};
var id = SQL.insert(db, "users", user);
user.name = "NewExample"
// updating our db user based on the id of insert
SQL.set(db, "users", id, user);
```
Now let's check if everything went fine:
```javascript
console.log(SQL.get(db, "users")); //Prints all users
```
And to set the db back, we delete the row with a custom query:
```javascript
var query = "DELETE FROM users WHERE id = " + id.toString() + ";";
SQL.execute(db, query);
```









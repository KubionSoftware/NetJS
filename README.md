![NetJS](images/logo.png?raw=true "Title")

NetJS is a javascript engine implemented in .NET
It allows for backend development in javascript while still using the .NET framework and running your applications in IIS.

The engine implements all standard javascript features.
Next to the standard it provides serverside features like session management, IO, logging, SQL and many more.

## Table of contents

* [Include and import](#include-import)
* [Session](#session)
* [IO](#io)
* [Log](#log)
* [HTTP](#http)
* [SQL](#sql)
* [Installation](#installation)

Every scope has a buffer for text output, wheneven an expression outputs a value it is converted to a string and added to the buffer.
This allows you easily output HTML or text without having to store it in a javascript variable for return.

<a name="include-import"/>

## Include and import
You can include another javascript or html file. This runs the code in the file and returns the value.

```javascript
include("logic.js", {loggedIn: true});

include("article.html", {title: "My article", body: "Lorem impsum..."});
```

You can also import javascript files. This will run the code in that file in the current scope.
This can be used to import functions or variables.

```javascript
import("date.js");
formatDate(new Date());
```

<a name="session"/>

## Session
Sessions are implemented using ASP.NET. You can use the session to store and retrieve values.
Unlike SessionStorage in browsers, this session allows you to store all types of variables instread of only strings.

```javascript
Session.set("userId", user.id);

var userId = Session.get("userId");
```

<a name="io"/>

## IO
With the IO object you can interact with the local filesystem. It allows creating, reading, writing and deleting files.

```javascript
var text = IO.read("disclaimer.txt");

IO.write("data.json", "{}");

IO.delete("data.json");
```

<a name="log"/>

## Log
The Log object allows you to write text to a log file

```javascript
Log.write("This is a debug message");
```

<a name="http"/>

## HTTP
With the HTTP object you can execute get and post requests

```javascript
var getResponse = HTTP.get("example.com");

var postResponse = HTTP.post("example.com", JSON.stringify(data));
```

<a name="sql"/>

## SQL
The SQL object allows access to an SQL database. To use this you must define the connection in a file named 'connections.json' in the root of the project. An example:

```json
{
	"Data": {
		"type": "sql",
		"connectionString": "Server=example.com;Database=ExampleName;User Id=sa;Password=test"
	}
}
```

You can then use the SQL object to execute statements with that connection. If the statement starts with 'SELECT' it will return the data as an array with objects.

```javascript
var rows = SQL.execute("Data", "SELECT * FROM Table");
```

<a name="installation"/>

## Installation
- Copy the release folder and place it somewhere on your computer
- Rename the release folder to the name of your project
- Create a new application in IIS with the location set to your project folder
- Set the security rights for your project folder to allow the IIS user

You should now have a running application. The entry point is the main.js file.

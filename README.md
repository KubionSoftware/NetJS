# NetJS

NetJS is a javascript engine implemented in .NET
It allows for backend development in javascript while still using the .NET framework and running your applications in IIS.

The engine implements all standard javascript features.
Next to the standard it adds support for HTML literals and provides some serverside features like session management, IO and logging.

## HTML literatals
You can write html without having to escape it as a string

```html
<div class='users'>
  for(var user of users){
    <div class='user' data-id='#user.id#'>
      <div class='user-name'>user.name</div>
      <div class='user-address'>user.address + ", " + user.city</div>
    </div>
  }
</div>
```

Every scope has a buffer for text output, wheneven an expression outputs a value it is converted to a string and added to the buffer.
This allows you easily output HTML or text without having to store it in a javascript variable for return.

## Include and import
You can include another javascript file. This runs the code in the file and returns the value.

```javascript
include("renderLayout.js", {loggedIn: true});
```

You can also import javascript files. This will run the code in that file in the current scope.
This can be used to import functions or variables.

```javascript
import("date.js");
formatDate(new Date());
```

## Session
Sessions are implemented using ASP.NET. You can use the session to store and retrieve values.
Unlike SessionStorage in browsers, this session allows you to store all types of variables instread of only strings.

```javascript
Session.set("userId", user.id);

var userId = Session.get("userId");
```

## IO
With the IO object you can interact with the local filesystem. It allows creating, reading, writing and deleting files.

```javascript
var text = IO.read("disclaimer.txt");

IO.write("data.json", "{}");

IO.delete("data.json");
```

## Log
The Log object allows you to write text to a log file

```javascript
Log.write("This is a debug message");
```

## HTTP
With the HTTP object you can execute get and post requests

```javascript
var getResponse = HTTP.get("example.com");

var postResponse = HTTP.post("example.com", JSON.stringify(data));
```

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

## Installation
- Create a new folder for your project.
- Copy the binaries from the release folder and place them in a new folder called 'bin'.
- Create a folder called 'src' on the same level as the bin folder.
- Copy the web.config file from the release folder and place it in your project folder.
- Create a new file 'main.js' in your src folder

You should now have the following structure

```
   .
   ├── bin
   │   └── ...
   ├── src
   │   └── main.js
   └── web.config
```

- Create a new application in IIS with the location set to your project folder.
- Set the security rights for your project folder to allow the IIS users.

You should now have a running application. The entry point is the main.js file.

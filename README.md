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

## Include and Import
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

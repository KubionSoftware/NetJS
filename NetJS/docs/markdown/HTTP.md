# HTTP
HTTP class handles HTTP methods for clients.
_This class can create GET, POST and custom requests._




## Index
**Functions:**
* [get(url)](#get(url))

* [post(url, body)](#post(url,-body))

* [execute(connectionName, query)](#execute(connectionName,-query))





## Functions
### get(url)
Executes a GET method and returns the response.
Variable | Description
 ------------------------- | -------------------------
url | the url to fire the GET method at
**Returns:**
a string with a response.




**Example:**
```javascript
var response = HTTP.get("https://google.com");
```




### post(url, body)
Executes a POST method and returns the response.
Variable | Description
 ------------------------- | -------------------------
url | The url to fire the POST method at
body | A body to attach to the POST method
**Returns:**
a string with a response.




**Example:**
```javascript
HTTP.post("https://google.com", {name: "newUser"}.ToString());
```




### execute(connectionName, query)
Executes a GET method with a query.
Variable | Description
 ------------------------- | -------------------------
connectionName | A connection known in the application connections
query | The query to attach to the url
**Returns:**
Returns the response, as a json object if possible.
**Exceptions:**
* _**InternalError**: Thrown if application not found in application scope._




**Example:**
```javascript
HTTP.execute("google search", "q=hello+world");
```





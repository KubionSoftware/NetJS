# Functions
Functions class contain functions that are injected directly into the engine.




## Index
**Functions:**
* [include(file, variables)](#include(file,-variables))

* [import(file)](#import(file))

* [redirect(url)](#redirect(url))





## Functions
### include(file, variables)
includetakes a file, runs the code in the file and returns the value.
If an object is given as second parameter, those variables will be set in the code before execution.
Default filetype is ".js".
Variable | Description
 ------------------------- | -------------------------
file | The file to include
variables | An object with variables to setup the file before execution
**Exceptions:**
* _**InternalError**: Thrown when no application is found in application scope._




**Example:**
```javascript
include("renderLayout.js", {loggedIn: true});
```




### import(file)
import takes a file and runs the code in the file with the current scope.
This way functions and variables can be imported.
Default filetype is ".js".
Variable | Description
 ------------------------- | -------------------------
file | The file to import
**Exceptions:**
* _**InternalError**: Thrown when no application is found in application scope._




**Example:**
```javascript
import("date");
FormatDate(new Date());
```




### redirect(url)
redirect takes an url and redirects a HttpResponse to the given url.
Variable | Description
 ------------------------- | -------------------------
url | A url to redirect to




**Example:**
```javascript
redirect("https://google.com/search?q=hello+world");
```





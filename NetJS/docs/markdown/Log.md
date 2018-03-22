# Log
Log class contains methods for logging to a file.




## Index
**Functions:**
* [write(log)](#write(log))





## Functions
### write(log)
Writes a log to the system configured log.
Variable | Description
 ------------------------- | -------------------------
log | The log that needs to be written
**Exceptions:**
* _**InternalError**: Thrown when there is no application in the application scope._




**Example:**
```javascript
Log.write("Hello world!");
```





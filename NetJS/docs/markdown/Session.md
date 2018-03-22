# Session
Sessions are implemented using ASP.NET. You can use the session to store and retrieve values.
_This class can set, get and delete key-value pairs._
_Unlike SessionStorage in browsers, this session allows you to store all types of variables instread of only strings._




## Index
**Functions:**
* [get(key)](#get(key))

* [set(key, value)](#set(key,-value))

* [delete(key)](#delete(key))

**[Examples](#examples)**:
* [Functions implementation](#Functions-implementation)




## Functions
### get(key)
Sessions.get takes a key, gets the value linked in the session and returns the value.
Variable | Description
 ------------------------- | -------------------------
key | The key to get a value from
**Returns:**
Value linked to key.
**Exceptions:**
* _**InternalError**: Thrown when no application has been found in application scope._




**Example:**
```javascript
var value = Sessions.get("userID");
```




### set(key, value)
Sessions.set takes a key and a value and sets the link in the session.
Variable | Description
 ------------------------- | -------------------------
key | The key to set a value with
value | The value to link to the key
**Exceptions:**
* _**InternalError**: Thrown when no application has been found in application scope._




**Example:**
```javascript
Session.set("userId", user.id);
```




### delete(key)
Sessions.delete takes a key and removes it from the session.
Variable | Description
 ------------------------- | -------------------------
key | The key to get a value from
**Exceptions:**
* _**InternalError**: Thrown when no application has been found in application scope._




**Example:**
```javascript
Sessions.delete("userId");
```




## Examples
### Functions implementation
Here you can see the functions of this class in action:
```javascript
Sessions.set("key", "value");
console.log(Sessions.get("key"); //prints: value
Sessions.delete("key");
```









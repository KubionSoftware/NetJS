# IO
IO class contains methods for file manipulation.
_IO can execute read, write and delete actions on a file._




## Index
**Functions:**
* [write(file, content)](#write(file,-content))

* [read(file)](#read(file))

* [delete(file)](#delete(file))

**[Examples](#examples)**



## Functions
### write(file, content)
Writes content into a file.
Variable | Description
 ------------------------- | -------------------------
file | A filename
content | The content to be written
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in the application scope._




**Example:**
```javascript
IO.write("data.json", { name: "Hello World!");
```




### read(file)
Reads and returns content of a file.
Variable | Description
 ------------------------- | -------------------------
file | A filename to read from 
**Returns:**
The content of the file.
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in the application scope._




**Example:**
```javascript
var content = IO.read("data.json");
```




### delete(file)
Deletes a file.
Variable | Description
 ------------------------- | -------------------------
file | A filename to delete
**Exceptions:**
* _**InternalError**: Thrown when no application can be found in the application scope._




**Example:**
```javascript
IO.delete("data.json");
```




## Examples
IO can read, write and delete a file:
```javascript
var file = "example.txt";
IO.write(file, "Hello World!");
console.log(IO.read(file); //prints: Hello World!
IO.delete(file);
```









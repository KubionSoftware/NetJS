# XDoc
A Compatibility class for XDoc, to ensure possibility of usage of XDoc with NetJS.




## Index
**Functions:**
* [include(name, parameters)](#include(name,-parameters))

* [get(key, context, id)](#get(key,-context,-id))





## Functions
### include(name, parameters)
XDoc.include includes other XDoc templates and returns the result.
Variable | Description
 ------------------------- | -------------------------
name | Name of the included file
parameters | optional, 0 or more parameters to be set before executing the template
**Returns:**
The result of executing the file.
**Exceptions:**
* _**InternalError**: Thrown when no application has been found in the application scope._




### get(key, context, id)
XDoc.get runs a svCache.GetSV with the given parameters.
Variable | Description
 ------------------------- | -------------------------
key | Key for identification
context | ContextName for sv
id | ID needed for svCache.GetSV, can be the returnvalue
**Returns:**
The result of svCache.GetSV
**Exceptions:**
* _**InternalError**: Thrown when no application has been found in application scope._





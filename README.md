ResxToJS
========

Resx2Js is a utility that converts .NET/C# resx files into a JavaScript file which contains a JSON version of the.NET resources.

This will allow you to store all your resources in one place on the .NET side and regenerate them for the JavaScript side
without duplicating the resource entries on the JavaScript side. 


Usage
------

To convert .resx files in your .NET solution into JavaScript files, use the following

To pretty print the output JavaScript, use the following.

ResxToJs.exe -i "path to .NET resource files folder" -o "path to JavaScript resource files folder" --prettyPrint

By default, the generated JavaScript files use the same name as the C# resource files with .js appended to 
the end. To change this, use the below. 

ResxToJs.exe -i "path to .NET resource files folder" -o "path to JavaScript resource files folder" --jsFileName "name of the js file"


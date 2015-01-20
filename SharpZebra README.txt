SharpZebra


What is it?
-----------
SharpZebra is a .Net wrapper for the EPL2 and ZPL languages for communicating with the Zebra range of printers. 


Why SharpZebra?
---------------
Use these APIs if you don't want to deal with the low level language that is EPL2 or ZPL, yet still make use of the Zebra printers. 


File Contents
-------------
 README.txt (this file)
 bin/SharpZebra.dll (the library)


Release Notes
-------------
Version 0.91
- Added ZPL support
- Added support for network and USB printers (includes USB registry parse fix from submitter nimeshdhruve)
- Fixed codepage problem - fixes issues uploading binary files to printers
- Ability to upload custom fonts added
- Added ability to upload text in any font supported by the hosting computer

Version 0.90
- Make IRawPrinter public
- Introduce IRawPrinter so people aren't tied to the implementation of Zebra Printer
- Remove duplicate namespaces
- Initial Version


The Latest Version
------------------

Details of the latest version can be found on the SharpZebra project web site http://www.codeplex.com/sharpzebra


License (based on the MIT License)
----------------------------------
Copyright (c) 2007 Patrick Kua

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
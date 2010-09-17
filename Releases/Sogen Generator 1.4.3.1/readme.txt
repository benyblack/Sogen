Sogen - Code generator for BLToolkit (sogen.codeplex.com)
Quick Start


A. Generate files (by Sogen generator console application)
	1. run SogenGenerator
	2. type "y" to create template config file.
	3. Open "SogenBLToolkitconfig.xml" file and edit that
		3.1. Set root namespace in <rootNamespace>
		3.2. Set data model name postfix in <dataModelNamePostfix>
		3.3. Set export path in <exportPath>
		3.4. Set Sql Connection in   <MSSqlConfig>
		3.5. create <schema>schema name</schema> for each schema that you want generate code.
	 note: for more information see : http://sogen.codeplex.com/wikipage?title=Configuration&referringTitle=Documentation
	4. Run SogenGenerator

B. Create class library
	1. Create a new class library
	2. Add bltoolkit dll to project references (including in download pack & available to download from www.bltoolkit.net)
	3. Add System.Configuration to project references
	4. copy & paste generated files in project
	5. Build project

C. Use genereted class
	Sogen generate a class for each table or view in your database. this classes are in namespaces with name of schemas.
	you can direct use this class and helper methods for insert/update/delete/get.also you can use datamodel of each schema
	see sample project (http://sogen.codeplex.com/releases/) & Bltoolkit documentation (http://bltoolkit.net/Doc.MainPage.ashx) for more info 
	
note: create connection strings in your app.config or web.config for each schema. base on generated config.cs file.
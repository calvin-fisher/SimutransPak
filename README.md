SimutransPak.dll is a .NET library that parses Simutrans paks into strongly typed objects.

Currently, this is most useful in LinqPAD (http://www.linqpad.net/), where you can query existing paks.  C#/LINQ allows for very powerful queries: http://msdn.microsoft.com/en-us/library/vstudio/bb397927.aspx

===========
=  Usage  =
===========

To use, create a new instance of the SimutransPak.Pak class and pass in the full path of the pak folder.

The Pak class has an Objects property, which is a collection of all .dat objects (DatObject class) in its subfolders.  The DatObject class references a DatFile class with information about the file it comes from.

A second optional parameter for the Pak class constructor is the language code to use for translations.  "en" is the default, which will load /text/en.tab

The .ToString() methods of objects and files are implemented so that, in the future, .dat files can be created or modified programmatically.

Several basic queries are included as convenience methods on the Pak class.

============
= Examples =
============

Basic usage in LINQPad
----------------------
var pak = new SimutransPak.Pak(@"C:\Users\username\Documents\GitHub\simutrans-pak128.britain");
var query = pak.Objects.Where(x => x.Name == "foo");
query.Objects.Dump()

Specifying a translation language
---------------------------------
var germanTranslations =
	new SimutransPak.Pak(@"C:\Users\username\Documents\GitHub\simutrans-pak128.britain", "de");

Get all train engines
---------------------
var trainEngines =
	from o in pak.Objects
	where o.Obj == "vehicle"
		&& o.Waytype == "track"
		&& o.Cost != null && o.Cost > 0
		&& o.EngineType != null
	orderby o.IntroYear, o.IntroMonth
	select o;

Find all objects without translations
-------------------------------------
var untranslatedObjects =
	from o in pak.Objects
	where o.NameTranslated == null
	orderby o.Name
	select o;
	
Using anonymous objects to create new "columns"
-----------------------------------------------
var boatsForComparison =	
	from o in pak.Objects
	where o.Obj == "vehicle"
		&& o.Waytype == "water"
		&& o.Payload != null
		&& o.Payload > 0
		&& o.Cost != null
		&& o.Cost > 0
	orderby o.IntroYear, o.IntroMonth
	select new {
		Name = o.NameTranslated ?? o.Name,  // Coalescing translated and program names
		o.IntroYear,
		o.IntroMonth, 
		Freight = o.FreightTranslated ?? o.Freight, // Coalescing translated and program names
		RunningcostPerUnit = // You can create new columns with any operations permitted in the language
			Math.Round((decimal?)o.Runningcost / (decimal?)(o.Payload + (o.OvercrowdedCapacity ?? 0)) ?? 0, 3),
		o.Runningcost,
		o.Payload,
		o.OvercrowdedCapacity };

Locating objects' files
----------------------
var objectOrigins =
	from o in pak.Objects
	orderby o.Name
	select new {
		o.Name,
		o.Obj,
		FileName = o.DatFile.SourceFile.FullName };
				
Comparing different versions of a pak
------------------------------------
var standard = new SimutransPak.Pak(@"C:\Users\username\Documents\GitHub\simutrans-experimental\simutrans\pak128.Britain-standard");
var experimental = new SimutransPak.Pak(@"C:\Users\username\Documents\GitHub\simutrans-experimental\simutrans\pak128.Britain");
var standardOnlyObjects =
	from standardObject in standard.Objects
		where standardObject.Name != null
	join experimentalObject in experimental.Objects
		on standardObject.Name equals experimentalObject.Name
		into common
	from commonObject in common.DefaultIfEmpty()
	where commonObject == null
	orderby standardObject.Name
	select standardObject;
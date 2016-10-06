Crossout DB
===

Crafting Calculator, Price Graphs, Margins and more.

This is the Open Source Project for Crossout Market found here: http://crossoutdb.com/ it provides out of game prices, buy and sell offers as well as price and volume graphs.

Screenshot of the Website: ![Crossout Market](http://i.imgur.com/47N8CjD.png)

Example of the AGS-40 trend: ![AGS-40](http://i.imgur.com/sCNkg4k.png)

Contributing
===

You will need a few things to setup first:

* MySQL Server 5.x
* Visual Studio with .Net 4.6 installed.
* Some experience with Web Development.

Start with Forking the repo and import the MySQL dump crossout_dump.zip on your machine.

Then create the settings file in %appdata%/CrossoutWeb/WebSettings.json or start the project once since the file is created then and edit the file.
```json
{
	"DatabaseName" : "crossout",
	"DatabaseHost" : "localhost",
	"DatabasePassword" : "password",
	"DatabaseUsername" : "crossout",
	"DatabasePort" : 3306,
	"SignalrHost" : "localhost",
	"WebserverPort" : 80,
	"DataHost" : "localhost"
}
```

You may need to start Visual Studio as Administrator for Nancy or Owins Selfhost to work.

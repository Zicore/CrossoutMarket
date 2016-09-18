Crossout Market
===

This is the Open Source Project for Crossout Market found here: http://crossout.zicore.de/ it provides out of game prices, buy and sell offers as well as price and volume graphs.

Contributing
===

You will need a few things to setup first:

* MySQL Server 5.x
* Visual Studio with .Net 4.6 installed.
* Some experience with Web Development.

Start with downloading the MySQL dump crossout_dump.zip and import it on your machine.

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

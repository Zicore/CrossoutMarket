Crossout DB
===

# Disclaimer
We are not affiliated with Gaijin or Targem games in any way. We are just two guys with a passion for markets and games :smile:


Crafting Calculator, Price Graphs, Margins and more.
===

This is the Open Source Project for Crossout Market found here: http://crossoutdb.com/ it provides out of game prices, buy and sell offers as well as price and volume graphs.

Screenshot of the Website: ![Crossout Market](http://i.imgur.com/47N8CjD.png)

Example of the AGS-40 trend: ![AGS-40](http://i.imgur.com/sCNkg4k.png)

Contributing
===

You will need a few things to setup first:

* MySQL Server 5.x
* Visual Studio with .Net 4.6 installed.
* Some experience with Web Development.

Start with Forking the repo and import the MySQL from /Schema/crossout_structure_and_data_no_market.sql on your machine.

Then create the settings file in %appdata%/CrossoutWeb/WebSettings.json or start the project once since the file is created then and edit the file.
```json
{
  "CurrentVersion": "0.7.0",
  "DatabaseName": "crossout",
  "DatabaseHost": "localhost",
  "DatabasePassword": "your db password",
  "DatabaseUsername": "your db username",
  "DatabasePort": 3306,
  "SignalrHost": "localhost",
  "WebserverPort": 80,
  "DataHost": "localhost",
  "GoogleConsumerKey": "",
  "GoogleConsumerSecret": "",
  "FileCarEditorWeaponsExLua": "Resources\\Data\\0.7.0\\gamedata\\def\\ex\\car_editor_weapons_ex.lua",
  "FileCarEditorCabinsLua": "Resources\\Data\\0.7.0\\gamedata\\def\\ex\\car_editor_cabins.lua",
  "FileCarEditorDecorumLua": "Resources\\Data\\0.7.0\\gamedata\\def\\ex\\car_editor_decorum.lua",
  "FileCarEditorWheelsLua": "Resources\\Data\\0.7.0\\gamedata\\def\\ex\\car_editor_wheels.lua",
  "FileCarEditorCoreLua": "Resources\\Data\\0.7.0\\gamedata\\def\\ex\\car_editor_core.lua",
  "FileStringsEnglish": "Resources\\Data\\0.7.0\\strings\\english\\string.txt"
}
```

You may need to start Visual Studio as Administrator for Nancy or Owins Selfhost to work.

Gathering Data
===

## General Data

* Items
* Recipes
* Categories
* Types
* Rarities
* Factions

Manual Work, we go ingame and enter it manually in our database :smile:

## Market Data
We are currently reading the clients memory every ~5 minutes to get the most up to date market data.

## Stats and Descriptions
We basically unpack the game files and read them.

===
FAQ

Q: I don't find the part that reads the data from the market in your repo.
A: It's not part of the repository and not open source (yet), since we don't want that anyone plays with the games memory, also you could get banned for doing so.

Q: Can you implement feature XYZ.
A: We are allways open for suggestions, but we also have our own ideas and todo lists.
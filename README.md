Crossout DB
===

# Disclaimer
We are not affiliated with Gaijin or Targem games in any way. We are just two guys with a passion for markets and games :smile:


Crafting Calculator, Price Graphs, Margins and more.
===

This is the Open Source Project for Crossout Market found here: http://crossoutdb.com/ it provides out of game prices, buy and sell offers as well as price and volume graphs. Also new are item stats, item descriptions, filters and improved usage of [datatables](https://datatables.net/).

Screenshot of the main page: 

![Crossout Market](http://i.imgur.com/a9ovo2O.png)

Example of the Clarinet Tow item page: 

![Clarinet Tow Chart](http://i.imgur.com/pvOwYtU.png)

Example of recipe view: 

![Armored track recipe](http://i.imgur.com/XwO7R2C.png)

Example of stats view: 

![Hurricane stats](http://i.imgur.com/9CfARj3.png)

Contributing
===

**This readme currently only covers some basics, please join our IRC channel if you need any help, with setting everything up properly.**

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

CrossoutDB API
===
Our api provides all data we have gathered so far, in an easy to use form.

# Current API Version 1

## General Information about the API

The API provides Json formatted data models and are as self-explanatory as possible, if you have any questions you can join our IRC channel and we try to help.

All API endpoints currently use GET.

Even though the API is versioned, it can still change at any time without further notice. Hopefully there will be only fixes, but you have been warned :)

Do you have Ideas for more endpoints? Join our IRC or make an issue on our Github repository to tell us :)

## Base URL
```
/api/v1/
```
## Rarities

Results a list of all rarities

```
/api/v1/rarities
```

## Factions

Results a list of all factions

```
/api/v1/factions
```

## Types

Results a list of all types

```
/api/v1/types
```

## Categories

Results a list of all categories

```
/api/v1/categories
```

## Items

Results a list of items, optionally filtered by parameters.

```
/api/v1/items
/api/v1/items?rarity=&category=&faction=&removedItems=&metaItems=&query=
```

Optional Parameters
```
rarity : filters by rarity
category : filters by category
faction : filters by factions
removedItems : shows removed items (default false)
metaItems : shows meta items (default false)
query : search string
```

Examples
```
/api/v1/items?query=shotgun
/api/v1/items?rarity=rare&category=weapon
```

## Item

Results one item.

```
/api/v1/item/{item}
```

Mandatory Parameter
```
{item:int} : item id
```

Examples
```
/api/v1/item/1
```

## Recipe

Results the recipe of the given item. Includes the item and item stats for the item itself.

The recipe data structure is kind of complex and consists of all fields, we need on our site.

```
/api/v1/recipe/{item:int}
```

Mandatory Parameter
```
{item:int} : item id
```

Examples
```
/api/v1/recipe/1
```

## Recipe Deep

Results the recipe of the given item. Includes the item and item stats for the item itself.

Recursively results all the ingredients for the items ingredients too.

```
/api/v1/recipe-deep/{item:int}
```

Mandatory Parameter
```
{item:int} : item id
```

Examples
```
/api/v1/recipe-deep/1
```

How are we gathering Data
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

FAQ
===

Q: I don't find the part that reads the data from the market in your repo.

A: It's not part of the repository and not open source (yet), since we don't want that anyone plays with the games memory, also you 
could get banned for doing so.

&nbsp;

Q: Can you implement feature XYZ.

A: Sure, we are allways open for suggestions, but we also have our own ideas and todo lists and we are working on this project in our free time.

&nbsp;

Q: Where do I find the game files.

A: They are in the Crossout installation folder, you just need to unpack them. Contact us for more information.
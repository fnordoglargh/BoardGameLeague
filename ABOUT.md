# Features

_BoardGameLeague_ is a simple tool by a boardgamer for boardgamers. It allows you to create e.g. players and
games and enter the results of your gaming group.

## General functions

The game entities tab allows adding of entities and also allow changing them. Simply select an item from a list box and
change it. An _apply_ button finalizes changes. There is no undo functionality.

## Players

The Players group box allows you to create players and give them a name and assign a gender.

## Games

The Games group box lets you create games, choos a name, select the minimum and maximum number of players, select a type and assign a game family.

### Game Types

We have four different types of games:

#### Victory Points 

Your typical [eurogame](https://en.wikipedia.org/wiki/Eurogame) with victory points and at least one winner.

#### Win/Loose

Select this for your two player games which only track winners and loosers (like chess or go). This does not work for cooperative games against the game where you only have one winning or one
loosing side.

#### Ranks

Use this in free-for-all games without victory points and more than two players. First rank is the winner by default.

#### Team Ranks

Use this for team games with ranks. Rank number one is the winner by default.

## Game Families

It's up to you how you want to group your games. You could use it to group all 'Settlers of Catan' games and expansions
or even broader and use a category as 'worker placement'. Each game can only have one game family. You also need to be careful
not to group games of different types together. If you do **bgl** cannot evaluate results of the game family.

## Locations

Here you can add places where you play your games. Locations are only used to be assigned to results for now.

## Results

The Results tab has three sub tabs. One to add results, one to change (or delete) results and one to generate reports.

### New Results

A new results needs to be configured starting with the number of players which played a game at a location. Each player needs to 
be selected individually together with the earned points. You need to select at least one winner for a victory point type game.
Other restrictions apply to the other two types. **bgl** will tell you if it expects something else.

### Results View

In this tab you can select existing results and edit them. Deleting a result or a score is **not** undoable. You may still get your old results back by
not saving and reloading the database. There is also a backup folder in %APPDATA%\BoardGameLeague which might still contain you result.

### Report

#### Game and Game Family

The report tab will calculate averages of your players for a certain game or a game family. A table for a victory point type game may look like this:

| Name     | Amount Played | Amount Won | Amount Points | Average Points | Percentage Won | Best Score |
|----------|---------------|------------|---------------|----------------|----------------|------------|
| Player 1 | 1             | 1          | 10            | 10             | 100            | 10         |
| Player 2 | 1             | 0          | 8             | 8              | 0              | 8          |

The other game types will use different column headings.

#### ELO Scoring

The ELO button calculates the ELO scores of all players of a league and displays an overview in a table. For an introduction
how ELO is used to calculate the relative player strength have a look at the [Wikipedia article](https://en.wikipedia.org/wiki/Elo_rating_system).
The ELO scoring uses the formulas from this [Days of Wonder page](https://www.daysofwonder.com/online/en/play/ranking/). 

The table is read only and uses the following categories for its columns:

| Name     | Elo Rating | Games Played | Established Player (over 20 results) |
|----------|------------|--------------|--------------------------------------|
| Player 1 | 1500       | 1            | no                                   |
| Player 2 | 1458       | 23           | yes                                  |

Also note that the ELO scoring will not consider results of solo games.

## Menu Bar

### File

From here you can load a database from disk or save the active database (see title bar). *New Database* will create an empty database in the 
specified location and load it immediately.

### Usage

Opens an HTML file with the description that you are reading right now in either a browser inside bgl or your standard browser.

# Known issues

* Several other special cases like games ending in draws must be tested and enhanced.
* Deselect statistics comboboxes when report tab is left.
* Changes in existing entries are not always noticed (e.g. for games).

# Planned Features

You want to see a feature in this section earlier or you have an idea? Drop me a line and we'll talk about it.

## High Priority

* Graphs for the calculated results.
* Add some kind of indicator to explain the status of a result (new, edited, unchanged).

## Medium Priority

* Add tooltips to explain the comboboxes on the report tab.
* Undo support if a Score or maybe even a result has been deleted.

## Low Priority

* Sort collections (like results on copying) after they have been edited.
* Make locations and genders useful.
* Filter out players already selected in the result entry.
* Link multiple leagues (from different files) if a group of players participates in more than one.

# Changelog

## 0.8.0 (Planned)

### New Functionality

* Charting (at least for ELO progression)
* Added "Teamed Ranks".

### Bug Fixes

* Fixed defects inside the ELO calculation.

## 0.7.1

* Data is correctly applied even if the apply button is not pressed.
* Fixed a crash on selecting a game for entering a result.

## 0.7.0

### New Functionality

* Added game types Win/Loose and Ranks (including nicer column headings in the result table).
* Added ELO calculation for single games and game families.

### Bug Fixes

* If you decide to change the properties of a game in the middle of entering a result, the results tab will reload the game to reflect the changes.

## 0.6

* Combined entity creation and editing into one tab.
* Added a check if (for some reason) the about.html does not exist.

## 0.5.5

### New Functionality

* Added crating, loading and saving of multiple databases.
* Implemented result editing.
* Added Best Score and Average Points to result evaluation.
* Added a mechanism to translate the ABOUT file into HTML to view it from inside the app.
* New DB is loaded after creation.
* The last used database is remembered and used on next startup.
* First remove button will be disabled if there is only one score in the result.

### Bug Fixes

* Filtered list won't be saved to database anymore.
* Game Family text box is now enabled and disabled correctly.
* Maximum player number of new games will be set to the correct value.
* Deactivated some more entry controls after program started to prevent potential problems.
* ELO calculation will not consider solo games anymore.
* Fixed defect which prevented winners from being deleted.
* Fixed severe defect in database loading which caused crashes if a result was selected or wrong display of results in list.

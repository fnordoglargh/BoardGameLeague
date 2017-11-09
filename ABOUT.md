# Features

_BoardGameLeague_ is a simple tool by a boardgamer for boardgamers. It allows you to create e.g. players and
games and enter the results of your gaming group.

## General functions

Most tabs which allow adding of entities also allow changing them. Simply select an item from a list box and
change it. An _apply_ button finalizes changes. There is no undo functionality.

## Players

The Players tab allows you to create players and give them a name and assign a gender.

## Games

The Players tab allows you to create players and give them a name and assign a game family.

## Game Families

It's up to you how you want to group your games. You could use it to group all 'Settlers of Catan' games and expansions
or even broader and use a category as 'worker placement'. Each game can only have one game family.

## Locations

Here you can add places where you play your games. Locations are only used to be assigned to results for now.

## Results

The Results tab has three sub tabs. One to add results, one to change (or delete) results and one to generate reports.

### New Results

A new results needs to be configured starting with the number of players which played a game at a location. Each player needs to 
be selected individually together with the earned points. You need to select at least one winner.

### Results View

In this tab you can select existing results and edit them. Deleting a result or a score is **not** undoable. You may still get your old results back by
not saving and reloading the database. There is also a backup folder in %APPDATA%\BoardGameLeague which might still contain you result.

### Report

#### Game and Game Family

The report tab will calculate averages of your players for a certain game or a game family. A table may look like this:

| Name     | AmountPlayed | AmountWon | AmountPoints | AveragePoints | PercentageWon | BestScore |
|----------|--------------|-----------|--------------|---------------|---------------|-----------|
| Player 1 | 1            | 1         | 10           | 10            | 100           | 10        |
| Player 2 | 1            | 0         | 8            | 8             | 0             | 8         |

#### ELO Scoring

The ELO button calculates the ELO scores of all players of a league and displays an overview in a table. For an introduction
how ELO is used to calculate the relative player strength have a look at the [Wikipedia article](https://en.wikipedia.org/wiki/Elo_rating_system).
The ELO scoring uses the formulas from this [Days of Wonder page](https://www.daysofwonder.com/online/en/play/ranking/). 

The table is read only and uses the following categories for its columns:

| Name     | EloRating | GamesPlayed | IsEstablished |
|----------|-----------|-------------|---------------|
| Player 1 | 1500      |1            | no            |

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

# Planned Features

You want to see a feature in this section earlier or you have an idea? Drop me a line and we'll talk about it.

## High Priority

* Unclutter UI.
* Add some kind of indicator to explain the status of a result (new, edited, unchanged).

## Low Priority

* Remove first remove button.
* Undo support if a Score or maybe even a result has been deleted.
* Add tooltips to explain the comboboxes on the report tab.
* ELO for games or game families.
* Graphs for the calculated results.
* Implement support for games without victory points. If you need a workaround it should be possible to use the
  winner checkbox and simply add 1 point for the winer and 0 for the looser.
* Link multiple leagues (from different files) if a group of players participates in more than one.
* Make locations and genders useful.
* Filter out players already selected in the result entry.

# Changelog

## 0.5.5

### New Functionality

* Added crating, loading and saving of multiple databases.
* Implemented result editing.
* Added Best Score and Average Points to result evaluation.
* Added a mechanism to translate the ABOUT.md file into HTML to view it from inside the app.
* New DB is loaded after creation.
* The last used database is remembered and used on next startup.

### Bug Fixes

* Filtered list won't be saved to database anymore.
* Game Family text box is now enabled and disabled correctly.
* Maximum player number of new games will be set to the correct value.
* Deactivated some more entry controls after program started to prevent potential problems.
* ELO calculation will not consider solo games anymore.

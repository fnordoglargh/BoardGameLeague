# Features

_BoardGameLeague_ is a simple tool by a board gamer for board gamers. It allows you to create e.g. players and
games and keep track of the results for your gaming group.

## General functions

The game entities tab allows adding of entities and also allows changing them. 

To add an item you click the category for the new item and push the _New_ button. For editing you simple select an existing item from a list box,
change the data. An _apply_ button finalizes changes. There is no undo functionality.

![Entity Entry](img/bglEntityEntry.png "Entity Entry")

## Players

The Players group box allows you to create players and give them a name and assign a gender.

## Games

The Games group box lets you create games, choose a name, select the minimum and maximum number of players, select a type and optionally assign a game family.

### Game Types

We have four different types of games:

#### Victory Points 

Your typical [eurogame](https://en.wikipedia.org/wiki/Eurogame) with victory points and at least one winner.

#### Win/Lose

Select this for your two player games which only track winners and losers (like chess or go). This does not work for cooperative games against the game where you only have one winning or one
loosing side.

#### Ranks

Use this in free-for-all games without victory points and more than two players. First rank is the winner by default.

#### Team Ranks

Use this for team games with ranks. Rank number one is the winner by default.

## Game Families

Each game can can be linked to multiple families. It's up to you how you want to group your games. You could use it to group all 'Settlers of Catan' games and expansions
into one large family named 'Catan'. It is also possible to group a game like Agricola into families like 'Worker Placement' and 'Harvest Cycle'.
You also need to be careful not to group games of different types together. If you do, **bgl** cannot evaluate results of the game family.

## Locations

Here you can add places where you play your games. Locations are only used to be assigned to results for now.

## Results

The Results tab contains two sub tabs. One to add results and the other to change (or delete) results.

A result view might look like this:

![Result View](img/bglResultView.png "Result View")

### New Results

A new results needs to be configured starting with the number of players which played a game at a location. Each player needs to 
be selected individually together with the earned points. You need to select at least one winner for a victory point type game.
Other restrictions apply to the other two types. **bgl** will tell you if it expects something else.

### Results View

In this tab you can select existing results and edit them. Deleting a result or a score is **not** undoable. You may still get your old results back by
not saving and reloading the database. There is also a backup folder in %APPDATA%\BoardGameLeague which might still contain you result.

## Reports

bgl's reporting supports getting hard numbers in a table overview or ELO score and point progression in a chart.

### Game statistics &amp; ELO rankings

The same table view is used for reports on the game, game families end ELO scoring.

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

### X over Y

In this tab we get a complete overview of the the whole database in a neat table.

#### Players over Players

Shows how all players performed against all players (but themselves; colored in black). The format is wins, stalemates and losses.

![Players over Players](img/bglPlayersOverPlayers.png "Players over Players")

#### Players over Games

Shows which games have been played by which players.

![Players over Games](img/bglPlayersOverGames.png "Players over Games")

#### Years over Games

Shows all games in which years they were played.

![Years over Games](img/bglYearsOverGames.png "Years over Games")

### ELO Progression Chart

ELO progressions will be generated for the selected player(s) over the

1. entire database
2. selected game
3. selected game family

A resulting diagram might look like this:

![ELO chart](img/bglELO.png "ELO chart")

### Point Progression Chart

Depending on your selection of players from the list the point progression chart will be generated for either the selected game or game family.

The chart will stay empty without any other message if no results can be found.

## Menu Bar

### File

#### Database Interaction

From here you can load a database from disk or save the active database (see title bar). *New Database* will create an empty database in the 
specified location and load it immediately.

#### Preferences

##### Normalize Dates - On

All players in graphs for ELO and point progression have the same start and end date.

##### Normalize Dates - Off

Player graphs start and end with individual results.

##### Transparent Graph Area - On

The area under the graphs for ELO and point progression is transparent.

##### Transparent Graph Area - Off

The area under the graphs is tinted with the graph color.

##### Examples

###### ELO chart - transparent
![ELO chart](img/bglELOTransparent.png "ELO chart - normalized and transparent")
###### ELO chart - normalized and transparent
![ELO chart](img/bglELONormalizedTransparent.png "ELO chart - normalized and transparent")
###### ELO chart - normalized and tinted
![ELO chart](img/bglELONormalized.png "ELO chart - normalized and tinted")
###### ELO chart - tinted
![ELO chart](img/bglELOAllOn.png "ELO chart")

### Help

#### Usage

Opens an HTML file with the description that you are reading right now in either a browser inside bgl or your standard browser.

##### About 

Displays a small window with some version information and credits.

# Known issues

* Several other special cases like games ending in draws must be tested and enhanced.
* Deselect statistics comboboxes when report tab is left.
* Changes in existing entries are not always noticed (e.g. for games).

# Planned Features

You want to see a feature in this section earlier or you have an idea? Drop me a line and we'll talk about it.

## High Priority

* Add total played games in last row of "Years over Games".
* Add tooltip to describe how to group games in families.
* Enhance score comparision table to display best and worst scores based on player count.
* Add a bar chart or table to compare players and games on a basic level (e.g. wins and losses).
* Make locations and genders useful.

## Medium Priority

* Filter out players already selected in the result entry.
* Undo support if a Score or maybe even a result has been deleted.

## Low Priority

* Explore how adding data points with constant distance look like (opposed to current current view with accurate dates).
* Link multiple leagues (from different files) if a group of players participates in more than one.

# Changelog

## 0.9.2

### New Functionality

* Added an info window on the first startup to give users the choice to start with the empty or demo database.

### Bug Fix

* Fixed two defects which caused rare cases of counting wrong results in players over players table.

## 0.9.1

### New Functionality

* Added a small settings menu to control the color below the graphs in charts and give better comparison for players in charts.
* Restructured toolbar items.
* Added minimal description and qualitative screenshots to the ABOUT.md.
* Added a database to demonstrate functionality.

## 0.9.0

### New Functionality

* Added a table which shows all games and players who played them.
* Added a table which shows all games and years in which they were played.
* Added a table which shows how all players stand against all other players.

## 0.8.5

### New Functionality

* Changed relationship of games to game families from 1..1 to 1..n. A game can now be linked to multiple families.
* Added **Worst Score** to Game Statistics for victory point based games.
* Results in the edit list box are always sorted into the correct spot.
* The game type combobox stays deactivated if the selected game is already used in a result.
* Added reports for players.

## 0.8.4

### New Functionality

* Added a small indicator to the result entering tab to signify if it is a completely new result or edited. Indicator resets on adding the result.
* Existing scores are selected if tab key is used to enter a score test box on result entering screen.
* Entities (games) are sorted after editing an existing one or adding a new one.
* Made list box in result edit tab resizable.
* Added notifications for the charts in case that a player does not have enough data.
* Added zoom functionality to the line charts.

### Bug Fixes

* Fixed a defect in results editing where no valid rank was recored for new scores.
* Fixed a binding defect (and cleaned up selections in other controls) while loading a database when a results was open in the viewer.

## 0.8.2

### New Functionality

* Charting for ELO and points progression.
* Added "Teamed Ranks". A mode used for games played in teams.

### Bug Fixes

* Fixed defects inside the ELO calculation.

## 0.7.1

* Data is correctly applied even if the apply button is not pressed.
* Fixed a crash on selecting a game for entering a result.

## 0.7.0

### New Functionality

* Added game types Win/Lose and Ranks (including nicer column headings in the result table).
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

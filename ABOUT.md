# Features

_BoardGameLeague_ is a simple tool by a boardgamer for boardgamers. It allows you to create e.g. players and
games and enter results of your gaming group.

## General functions

Most tabs which allow adding of entities also allow changing them. Simply select an item from a list box and
change it. An _apply_ button finalizes changes. There is no undo functionality.

## Players

The players tab allows you to create players and give them a name and assign a gender.

## Games

The players tab allows you to create players and give them a name and assign a game family.

## Game Families

It's up to you how you want to group your games. You could use it to group all 'Settlers of Catan' games and expansions
or even broader and use a category as 'worker placement'. Each game can only have one game family.

## Locations

Here you can add places where you play your games. Locations are only used to be assigned to results for now.

## Results

The Results tab has three sub tabs. One to add results, one to change (or delete) results and one to generate reports.

### New Results

A new results needs to be configured starting with the number of players which played a game in a location. Each player needs to 
be selected individually together with the earned points. You need to select at least one winner.

### Results View

This tab is currently not really useful and will be enhanced soon.

### Report

#### Game and Game Family

The report tab will calculate averages of your players for a certain game or a game family. A table may look like this:

| Name     | AmountPlayed | AmountWon | AmountPoints | AveragePoints | PercentageWon |
| Player 1 | 1            | 1         | 10           | 10            | 100           |          
| Player 2 | 1            | 0         | 8            | 8             | 0             |

#### ELO Scoring

The ELO button calculates the ELO scores of all players of a league and displays an overview in a table. For an introduction
how ELO is used to calculate the relative player strength have a look at the [Wikipedia article](https://en.wikipedia.org/wiki/Elo_rating_system).
The ELO scoring uses the formulas from this [Days of Wonder page](https://www.daysofwonder.com/online/en/play/ranking/). 

The table is read only and uses the following categories for its columns:

| Name | EloRating | GamesPlayed | IsEstablished |

# Known issues

* ELO calculation doesn't like game results with a single player.
* Several other special cases like games ending in draws must be tested and enhanced.

# Planned Features

You want to see a feature in this section earlier or you have an idea? Drop me a line and we'll talk about it.

## High Priority

* Multiple databases: Loading and saving to independent files.
* Make result editing and deletion work.

## Low Priority

* Graphs for the calculated results.
* Implement support for games without victory points. If you need a workaround it should be possible to use the
  winner checkbox and simply add 1 point for the winer and 0 for the looser.
* Link multiple leagues (from different files) if a group of players participate in more than one.
* Make locations and genders useful.
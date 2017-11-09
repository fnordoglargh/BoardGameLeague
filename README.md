# BoardGameLeague

BoardGameLeague is a desktop application for Windows to track the results of boardgaming sessions.

## Getting Started

.NET 4.6.1 is needed to run BoardgameLeague.

### Compiling on and running on your own

You need Visual Studio 2017 to build BoardGameLeague.  I tried to keep dependencies as flat as possible. 
`grip` is needed to generate the HTML documentation (from the ABOUT.md). If you don't want to install grip, 
simply comment or remove this line from the build events:

```
grip "$(ProjectDir)\..\ABOUT.md" --export "$(ProjectDir)\bin\Debug\about.html
```

Apart from that *Build Solution* will work out of the box.

## Features

Please consult the [about page](ABOUT.md) for the feature set and future plans.

## Author

* *Martin Woelke* - contact me under bgl.boardgameleague at gmail.com

## Test and Feedback

* Annukka Nitsche-Woelke
* Bijo Varghese
* Jayakrishnan Kollaikkal
* Eveline Woelke

## Built With

* [log4net](https://logging.apache.org/log4net) - A port of the excellent Apache log4j™ framework to the Microsoft® .NET runtime. Licensed under the [Apache License, Version 2.0](BoardGameLeague/licenses/Apache2.txt).
* [NUnit](http://nunit.org) - A unit-testing framework for all .Net languages. Licensed under [MIT license](BoardGameLeague/licenses/mit.txt).
* [grip](https://github.com/joeyespo/grip) - Render local readme files before sending off to GitHub.

## FAQ

### Why is BoardGameLeague not on platform _x_ and framework _y_?

BoardGameLeague is a leisure activity and a product for my gaming groups. I like the technology behind it 
(databinding with WPF and XML as a database) and wanted to apply and enhance what I already knew.

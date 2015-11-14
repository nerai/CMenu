# CMenu
A simple console menu manager for C#

CMenu aims to simplify writing console menus. Instead of manually prompting the user for input and parsing it, you define commands in a short, structured and comprehensive way. You do not have to worry about all the annoying details and can do much more with less code.



## Advantages

* Simple, short syntax
  * Simple commands are created in a single line
  * Complex commands can be created or modified in steps or in their own, short class
* Powerful and extensible
  * Inner commands effortlessly parse parameters to commands
  * Nested menus allow fully or partially independent submenus
  * Own commands as well as subclassing existing commands are fully supported
* Low maintenance
  * Adaptive, automatically configures itself for optimal usage. For instance, command abbreviations are created and updated automatically when new commands (with potentially similar name) are added, or removed.
  * Very tolerant about its input, allowing partial matches and being (by default) case insensitive.
* Integrated tutorial and examples
  * Contains a library of example commands, including a conditional operator, a macro recorder and a procedural call structure
  * Comprehensive, documented code in case you want to dig into the details
* Result: An easy to use menu structure
  * Effortless command detection, including automatic abbreviations and partial matching
  * Intelligent corrections for mistyped commands
  * Integrated useful help which also works in command tree branches



## Quickstart

A single command is comprised of a keyword (selector), an optional help text describing it, and, most importantly, its behavior. The behavior can be defined as a simple lambda.

	var menu = new CMenu ();
	menu.Add ("test", s => Console.WriteLine ("Hello world!"));
	menu.Run ();

	$ test
	Hello world!

To create a command with help text, simply add it during definition. Use the integrated "help" command to view help texts.

	menu.Add ("time",
		s => Console.WriteLine (DateTime.UtcNow),
		"Writes the current time");

	$ time
	2015.10.01 17:54:38
	$ help time
	Writes the current time

CMenu keeps an index of all available commands and lists them upon user request via typing "help". Moreover, it also automatically assigns abbreviations to all commands (if useful) and keeps them up-to-date when you later add new commands with similar keywords.

	$ help
	Available commands:
	e   | exit
	h   | help
	q   | quit
	ti  | time
	te  | test
	Type "help <command>" for individual command help.
	$ te
	Hello world!


	
## Further reading

### Common usage
Refer to [common use cases](doc/common-usage.md). This includes command creation, integrated help, automatic command abbreviations and case sensitivity.

### Advanced usage
There is documentation about [more complex use cases](doc/advanced.md), including input queue modification, inner commands, nested menus and default commands and sharing code between items.

### Example project

The source code contains an [example project](doc/example_project.md). It offers commands, which illustrate several (more or less advanced) use cases. It may be useful to reference them in your own projects.


	
## Project history and developer information

Another (much larger) project of mine uses a somewhat complex console menu, and I finally got annoyed enough by repeated, virtually equal code fragments to refactor and extract those pieces of code into a separate project. As I figured it was complete enough to be useful to others, I called it CMenu and put it on github.

In the future, there are certainly a lot of things to improve about CMenu, and I would definitely not call it complete or particularly well made. The changes and issues found on this project are mostly caused by improvements and requirement changes in said larger project. I update CMenu with the relevant changes in parallel when I find the time and will continue to do so for the foreseeable future.

I'm happy to hear if you found this useful, and open to suggestions to improve it.


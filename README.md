# CMenu
A simple console menu manager for C#


CMenu is a lightweight, low-ceremony framework for building console menus in .Net. Instead of manually prompting the user for input and parsing it, you define commands in a short, structured and comprehensive way, and let CMenu handle the rest.

CMenu aims for low overhead - simple stuff should be simple to implement. If you don't use a feature, you don't need to know anything about it.

At the same time, complex scenarios are supported. Large menus can easily be split into several classes. Background self-configuration. You do not have to worry about all the annoying details involved in larger menus, it will just work.

Most importantly, your menus will be quick and simple to use. Commands can be abbreviated, a smart parser enables even partial matching. Help for users is integrated.


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

### Advantages
An [overview of the benefits of CMenu](doc/advantages.md).



## Project history and developer information

Another (much larger) project of mine uses a somewhat complex console menu, and I finally got annoyed enough by repeated, virtually equal code fragments to refactor and extract those pieces of code into a separate project. As I figured it was complete enough to be useful to others, I called it CMenu and put it on github.

In the future, there are certainly a lot of things to improve about CMenu, and I would definitely not call it complete or particularly well made. The changes and issues found on this project are mostly caused by improvements and requirement changes in said larger project. I update CMenu with the relevant changes in parallel when I find the time and will continue to do so for the foreseeable future.

I'm happy to hear if you found this useful, and open to suggestions to improve it.


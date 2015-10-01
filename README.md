# CMenu
A simple console menu manager for C#

CMenu aims to simplify writing console menus. Instead of manually prompting the user for input and parsing it, you define single commands in a structured and comprehensive way.

A single command is comprised of a keyword (selector), an optional help text describing it, and, most importantly, its behavior. The behavior can be defined as a simple lambda, and it is not unusual for a whole command to be defined in a single line.

Example:

	// Create menu
	var menu = new CMenu ();

	// Add simple Hello World command.
	menu.Add ("hello", s => Console.WriteLine ("Hello world!"));

	// Add command with behavior defined in separate method.
	menu.Add ("len", s => PrintLen (s));

	// Add alternative way to stop processing input (by default, "quit" is provided).
	menu.Add ("exit", s => MenuResult.Quit);

	// Add menu item with help text.
	menu.Add (
		"time",
		s => Console.WriteLine (DateTime.UtcNow),
		"time\nWrites the current time (UTC).");

	// Run menu. The menu will run until quit by the user.
	menu.Run ();
	
	[...]
	static void PrintLen (string s)
	{
		Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
	}

While running, CMenu will prompt the user for input, then feeds it to the respective command. Let's see how to use the "len" command defined above:

	$ len 54321
	String "54321" has length 5

CMenu keeps an index of all available commands and lists them upon user request via typing "help". Moreover, it also automatically assigns abbreviations to all commands (if useful) and keeps them up-to-date when you later add new commands with similar keywords.

	$ help
	Available commands:
	e   | exit
	      hello
	      help
	l   | len
	q   | quit
	t   | time
	Type "help <command>" for individual command help.

The builtin command "help" also displays usage information of individual commands:

	$ help quit
	quit
	Quits menu processing.
	$ help help
	help [command]
	Displays a help text for the specified command, or
	Displays a list of all available commands.

Commands can by entered abbreviated, as long as it is clear which one was intended. If it is not clear, then the possible options will be displayed. Commands are always case INsensitive.

	$ hel
	Command <hel> not unique. Candidates: hello, help
	$ hell
	Hello world!

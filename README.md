# CMenu
A simple console menu manager for C#

CMenu aims to simplify writing console menus. Instead of manually prompting the user for input and parsing it, you define single commands in a structured and comprehensive way.



## Creating and using basic commands

A single command is comprised of a keyword (selector), an optional help text describing it, and, most importantly, its behavior. The behavior can be defined as a simple lambda, and it is not unusual for a whole command to be defined in a single line.

	// Create menu
	var menu = new CMenu ();

	// Add simple Hello World command
	menu.Add ("hello", s => Console.WriteLine ("Hello world!"));

	// Run menu. The menu will run until quit by the user.
	menu.Run ();

While running, CMenu will continuously prompt the user for input, then feeds it to the respective command. Let's see how to use the "hello" command defined above:

	$ hello
	Hello world!

If the command happens to be more complex, you can just put it in a separate method.
	
	menu.Add ("len", s => PrintLen (s));

	[...]
	
	static void PrintLen (string s)
	{
		Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
	}

	$ len 54321
	String "54321" has length 5

It is also possible to return an exit code to signal that processing should be stopped.
By default, the command "quit" exists for this purpose. Let's add an alternative way to stop processing input.

	menu.Add ("exit", s => MenuResult.Quit);

To create a command with help text, simply add it during definition or later.

	menu.Add ("time",
		s => Console.WriteLine (DateTime.UtcNow),
		"Writes the current time (UTC).");
	menu["time"].HelpText += " (UTC).";

	$ time
	2015.10.01 17:54:38
	$ help time
	Writes the current time (UTC).



## Command abbreviations and integrated help 

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

	$ help q
	quit
	Quits menu processing.
	$ help help
	help [command]
	Displays a help text for the specified command, or
	Displays a list of all available commands.

Commands can by entered abbreviated, as long as it is clear which one was intended. If it is not clear, then the possible options will be displayed. Commands are case INsensitive by default, you can change that with the StringComparison property.

	$ hel
	Command <hel> not unique. Candidates: hello, help
	$ hell
	Hello world!



## Modifying the input queue

It is also possible to modify the input queue.
Check out how the "repeat" command adds its argument to the input queue three times.

	// Add a command which repeats another command
	menu.Add ("repeat",
		s => Repeat (s),
		"Repeats a command 3 times.");

	[...]
	
	static void Repeat (string s)
	{
		menu.Input (s);
		menu.Input (s);
		menu.Input (s);
	}

	$ repeat hello
	Hello world!
	Hello world!
	Hello world!
	$ r l 123
	String "123" has length 3
	String "123" has length 3
	String "123" has length 3

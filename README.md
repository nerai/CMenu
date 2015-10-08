# CMenu
A simple console menu manager for C#

CMenu aims to simplify writing console menus. Instead of manually prompting the user for input and parsing it, you define commands in a short, structured and comprehensive way. You do not have to worry about all the annoying details and can do much more with less code.

CMenu offers:

* very simple and short creation of menus
* flexible inner commands and even nested menus
* a library of example commands, including a conditional operator and a macro recorder
* comprehensive, documented code in case you want to dig into the details

Users will also be delighted by:

* effortless command detection, including automatic abbreviations and partial matching
* intelligent corrections for mistyped commands
* integrated useful help which also works in command tree branches



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

	static void PrintLen (string s)
	{
		Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
	}

	$ len 54321
	String "54321" has length 5

It is also possible to return an exit code to signal that processing should be stopped.
By default, the command "quit" exists for this purpose. Let's add an alternative way to stop processing input.

	menu.Add ("exit", s => MenuResult.Quit);

To create a command with help text, simply add it during definition.

	menu.Add ("time",
		s => Console.WriteLine (DateTime.UtcNow),
		"Help for \"time\": Writes the current time");

	$ time
	2015.10.01 17:54:38
	$ help time
	Help for "time": Writes the current time
	
You can also access individual commands to edit them later, though this is rarely required.

	menu["time"].HelpText += " (UTC).";
	
	$ help time
	Help for "time": Writes the current time (UTC).



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

Commands can by entered abbreviated, as long as it is clear which one was intended. If it is not clear, then the possible options will be displayed.

	$ hel
	Command <hel> not unique. Candidates: hello, help
	$ hell
	Hello world!

Commands are case *in*sensitive by default. This can be changed using the `StringComparison` property.
When in case sensitive mode, CMenu will helpfully point out similar commands with different caseing.

	menu.StringComparison = StringComparison.InvariantCulture;
	menu.Add ("Hello", s => Console.WriteLine ("Hi!"));

	$ help
	Available commands:
	e   | exit
	H   | Hello
	      hello
	      help
	l   | len
	q   | quit
	r   | repeat
	t   | time
	Type "help <command>" for individual command help.
	$ H
	Hi!
	$ h
	Command <h> not unique. Candidates: hello, help
	$ hE
	Unknown command: hE
	Did you mean "hello", "Hello" or "help"?



## Modifying the input queue

It is also possible to modify the input queue by adding input:
* Either at its beginning, so that it will be executed immediately after
* or at its end, so that it will be processed when the complete current queue has been processed.

Check out how the "repeat" command adds its argument to the input queue two times.

	// Add a command which repeats another command
	menu.Add ("repeat",
		s => Repeat (s),
		"Repeats a command two times.");

	static void Repeat (string s)
	{
		menu.Input (s, true);
		menu.Input (s, true);
	}

	$ repeat hello
	Hello world!
	Hello world!
	$ r l 123
	String "123" has length 3
	String "123" has length 3



## Inner commands

If a command needs further choices, you may want to select those in a similar manner as in the menu. To do that, simply add sub-items to the main item. If no other behavior is specified, the main item will continue selection within those embedded items.

	var mi = menu.Add ("convert", "convert upper|lower [text]\nConverts the text to upper or lower case");
	mi.Add ("upper", s => Console.WriteLine (s.ToUpperInvariant ()), "Converts to upper case");
	mi.Add ("lower", s => Console.WriteLine (s.ToLowerInvariant ()), "Converts to lower case");

	$ convert upper aBcD
	ABCD
	$ convert lower aBcD
	abcd

The integrated help is able to "peek" into commands.

	$ help convert
	convert upper|lower [text]
	Converts the text to upper or lower case
	$ help c u
	Converts to upper case



## Nested menus and default commands

If a command requires several lines of input itself, you can add a CMenu instead of a CMenuItem. A CMenu always offers input prompts in addition to the usual behavior and subitems of a CMenuItem. You may also opt for a custom prompt character, or even disable it.

It may be useful to capture all input which lacks a corresponding command in a "default" command. The default command has the unique selector null.

Let's see an example for a command which calculactes the sum of integers entered by the user. The sum is calculated and output once the user enters "=", which will be implemented as a subcommand. Capturing the integers is done with a default command. To clarify to the user that this is a different menu, we also replace the prompt character with a "+".

	public class MI_Add : CMenu
	{
		public MI_Add ()
			: base ("add")
		{
			HelpText = ""
				+ "add\n"
				+ "Adds numbers until \"=\" is entered.\n";
			PromptCharacter = "+";

			Add ("=", s => MenuResult.Quit, "Prints the sum and quits the add submenu");
			Add (null, s => Add (s));
		}

		private int _Sum = 0;

		private void Add (string s)
		{
			int i;
			if (int.TryParse (s, out i)) {
				_Sum += i;
			}
			else {
				Console.WriteLine (s + " is not a valid number.");
			}
		}

		public override MenuResult Execute (string arg)
		{
			Console.WriteLine ("You're now in submenu <Add>.");
			Console.WriteLine ("Enter numbers. To print their sum and exit the submenu, enter \"=\".");
			_Sum = 0;
			Run ();
			Console.WriteLine ("Sum = " + _Sum);
			return MenuResult.Normal;
		}
	}

	$ add
	You're now in submenu <Add>.
	Enter numbers. To print their sum and exit the submenu, enter "=".
	+ 2
	+ 3
	+ =
	Sum = 5



## Example project

The source code contains an example project. It offers commands, which illustrate several (more or less advanced) use cases. It may be useful to reference them in your own projects.

### echo

	echo [text]
	Prints the specified text to stdout.

	$ echo 123
	123

### if

	if [not] <condition> <command>
	Executes <command> if <condition> is met.
	If the modifier <not> is given, the condition result is reversed.
	
	It is allowed to specify multiple concurrent <not>, each of which invert the condition again.
	By default, the conditons "true" and "false" are known. Further conditions can be added by the developer.
	Condition combination is not currently supported, though it can be emulated via chaining ("if <c1> if <c2> ...")

	$ if true echo 1
	1
	$ if not true echo 1
	$ if not false echo 1
	1

### pause

	pause
	Stops further operation until the enter key is pressed.

### record

	record name
	Records all subsequent commands to the specified file name.
	Recording can be stopped by the command endrecord
	Stored records can be played via the "replay" command.
	
	Nested recording is not supported.

	$ record r1
	Recording started. Enter "endrecord" to finish.
	record> echo 1
	record> echo 2
	record> endrecord

### replay

	replay name
	Replays all commands stored in the specified file name.
	Replaying puts all stored commands in the same order on the stack as they were originally entered.
	
	Nested replaying is supported.

	$ replay r1
	1
	2



## Project history and developer information

Another (much much larger) project of mine uses a somewhat complex console menu, and I finally got annoyed enough by repeated, virtually equal code fragments to extract the control code into a separate, specialiced project. As I noticed a bunch of queries for something like this, and figured it was complete enough to be useful to others, I put it on github.

For the future, there are certainly a lot of things to improve about CMenu, and I would definitely not call it complete or particularly well made. The changes and issues found on this project are mostly caused by improvements and requirement changes in said larger project. I update CMenu with the relevant changes in parallel when I find the time and will continue to do so for the foreseeable future.

I'm happy to hear if you found this useful, and open to suggestions to improve it.

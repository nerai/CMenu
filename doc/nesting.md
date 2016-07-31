# Nesting



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

If only a single inner item exists, users do not have to type it. Automatic fall-through forwards to the inner item directly.

	var mi = menu.Add ("fall");
	mi.Add ("through", s => Console.WriteLine ("Fell through to the innermost item."));

	$ fall through
	Fell through to the innermost item.
	$ fall t
	Fell through to the innermost item.
	$ fall 
	Fell through to the innermost item.
	$ fall
	Fell through to the innermost item.
	$ fall xy
	Unknown command: xy



## Nested menus and default commands

Usually, all commands are processed by the same CMenu, which itself consists of several CMenuItems each responsible for a different command type. However, sometimes a command opens up a new submenu, often with commands different from its parent menu.

To achieve this functionality, just append a new child CMenu to its parent. When the user enters the submenu, all typing will be routed through it instead of the parent menu. The user can at any time return to the parent menu by quitting the child menu, for instace by providing a "quit" command.

Embedding a CMenu instead of a CMenuItem is advantageous if you do not want to keep track of state manually, possible including a menu stack (which parent menu to return to when a submenu is quit). You may opt for a custom prompt character to distinguish the submenu from its parent menu.

Especially in this context, it may be useful to capture all input which lacks a corresponding command in a "default" command. The default command has the unique selector null.

Let's see an example for a command which calculactes the sum of integers entered by the user. The sum is calculated and output once the user enters "=", which will be implemented as a subcommand. Capturing the integers is done with a default command. To clarify to the user that this is a different menu, we also replace the prompt character with a "+".

	public class MI_Add : CMenu
	{
		public MI_Add ()
			: base ("add")
		{
			HelpText = ""
				+ "add\n"
				+ "Adds numbers until \"=\" is entered.";
			PromptCharacter = "+";

			Add ("=", s => Quit (), "Prints the sum and quits the add submenu");
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

		public override void Execute (string arg)
		{
			Console.WriteLine ("You're now in submenu <Add>.");
			Console.WriteLine ("Enter numbers. To print their sum and exit the submenu, enter \"=\".");
			_Sum = 0;
			Run ();
			Console.WriteLine ("Sum = " + _Sum);
		}
	}

	$ add
	You're now in submenu <Add>.
	Enter numbers. To print their sum and exit the submenu, enter "=".
	+ 2
	+ 3
	+ =
	Sum = 5



### Sharing code between nested items

If your inner menu items should share code, you need to overwrite the menu's Execute method, then call ExecuteChild to resume processing in child nodes.

This allows you to alter the command received by the children, or to omit their processing altogether (e.g. in case a common verification failed).

	var m = menu.Add ("shared");
	m.SetAction (s => {
		Console.Write ("You picked: ");
		m.ExecuteChild (s);
	});
	m.Add ("1", s => Console.WriteLine ("Option 1"));
	m.Add ("2", s => Console.WriteLine ("Option 2"));

	$ shared 1
	You picked: Option 1



## Initialization syntax for menu trees

It may be useful to create complex menu trees using collection initializers.

	var m = new CMenu () {
		new CMenuItem ("1") {
			new CMenuItem ("1", s => Console.WriteLine ("1-1")),
			new CMenuItem ("2", s => Console.WriteLine ("1-2")),
		},
		new CMenuItem ("2") {
			new CMenuItem ("1", s => Console.WriteLine ("2-1")),
			new CMenuItem ("2", s => Console.WriteLine ("2-2")),
		},
	};
	
	$2 1
	2-1

You can also combine object and collection initializers

	m = new CMenu () {
		PromptCharacter = "combined>",
		MenuItem = {
			new CMenuItem ("1", s => Console.WriteLine ("1")),
			new CMenuItem ("2", s => Console.WriteLine ("2")),
		}
	};

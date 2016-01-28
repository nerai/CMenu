# Advanced usage

## Modifying the input queue

It is also possible to modify the input queue. The `IO` class provides flexible means to add input either directly or via an `IEnumerable<string>`. The latter allows you to stay in control over the input even after you added it, for instance by changing its content or canceling it.

Check out how the "repeat" command adds its argument to the input queue two times.

	// Add a command which repeats another command
	menu.Add ("repeat",
		s => {
			IO.ImmediateInput (s);
			IO.ImmediateInput (s);
		},
		"Repeats a command two times.");

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



## Disabled commands

Commands which cannot currently be used, but should still be available in the menu tree at other times, can disable themselves. Disabled commands cannot be used and are not listed by `help`.

In this example, a global flag (`bool Enabled`) is used to determine the visibility of disabled commands. It is initially cleared, the 'enable' command sets it.

	m.Add ("enable", s => Enabled = true);

Create a new inline command, then set its visilibity function so it returns the above flag.

	var mi = m.Add ("inline", s => Console.WriteLine ("Disabled inline command was enabled!"));
	mi.SetVisibilityCondition (() => Enabled);

	$ inline
	Unknown command: inline
	$ enable
	$ inline
	Disabled inline command was enabled!

It is also possible to override the visibility by subclassing.

	private class DisabledItem : CMenuItem
	{
		public DisabledItem ()
			: base ("subclassed")
		{
			HelpText = "This command, which is defined in its own class, is disabled by default.";
		}

		public override bool IsVisible ()
		{
			return Enabled;
		}

		public override void Execute (string arg)
		{
			Console.WriteLine ("Disabled subclassed command was enabled!");
		}
	}

	$ subclassed
	Unknown command: subclassed
	$ enable
	$ subclassed
	Disabled subclassed command was enabled!

Invisible commands are not displayed by `help`:

	$ help
	Available commands:
	e   | enable
	h   | help
	q   | quit
	$ enable
	$ help
	Available commands:
	e   | enable
	h   | help
	i   | inline
	q   | quit
	s   | subclassed

Command abbreviations do not change when hidden items become visible, i.e. it is made sure they are already long enough. This avoids confusion about abbreviations suddenly changing.

	m.Add ("incollision", s => Console.WriteLine ("The abbreviation of 'incollision' is longer to account for the hidden 'inline' command."));

	$ help
	Available commands:
	e   | enable
	h   | help
	inc | incollision
	q   | quit
	$ enable
	$ help
	Available commands:
	e   | enable
	h   | help
	inc | incollision
	inl | inline
	q   | quit
	s   | subclassed



## Passive mode

By default, input is handled in active mode, i.e. CMenu will actively prompt the user for required input and read their console input.

This behavior may be undesirable if you want closer control, for instance:

* In a GUI environment, creating input in the GUI instead of the console as usual
* In a batch or shell environment, feeding stored input instead of prompting the user for it

To suppress active prompting, enable passive mode by setting the `PassiveMode` flag. The menu will then wait for programmatic input, e.g. via `IO.AddInput`.

	IO is currently in active mode - you will be prompted for input.
	The 'passive' command will turn passive mode on, which disables interactive input.
	The 'active' command will turn active mode back on.
	Please enter 'passive'.
	$ p
	Passive mode selected. Input will be ignored.
	A timer will be set which will input 'active' in 5 seconds.
	5...
	4...
	3...
	2...
	1...
	0...
	Sending input 'active' to the IO queue.
	Active mode selected.
	$ 

Side note: Switching from active to passive mode during an (active) input query (i.e. while the user is being prompted for input) is supported but may lead to undesired behavior. In particular, the prompt will still wait for input after switching. This is due to limitations in the underlying system.


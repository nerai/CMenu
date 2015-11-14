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
			return MenuResult.Default;
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

If your inner menu items should share code (e.g. common basic validation), there are two ways to implement this.

First option: Override Execute in their parent menu item so it first executes the shared code, then resumes normal processing.

	class SharedViaOverride : CMenuItem
	{
		public SharedViaOverride ()
			: base ("shared-override")
		{
			Add ("1", s => Console.WriteLine ("First child"));
			Add ("2", s => Console.WriteLine ("Second child"));
		}

		public override MenuResult Execute (string arg)
		{
			Console.WriteLine ("This code is shared between all children of this menu item.");
			if (DateTime.UtcNow.Millisecond < 500) {
				return MenuResult.Default;
			}
			else {
				// Proceed normally.
				return base.Execute (arg);
			}
		}
	}

Second option: Use the return values of Execute to indicate if processing should continue with the children, or return immediately. Returning is the default.

	var msr = menu.Add ("shared-result", s => {
		Console.WriteLine ("This code is shared between all children of this menu item.");
		if (DateTime.UtcNow.Millisecond < 500) {
			return MenuResult.Proceed;
		}
		else {
			return MenuResult.Return;
		}
	});
	msr.Add ("1", s => Console.WriteLine ("First child"));
	msr.Add ("2", s => Console.WriteLine ("Second child"));

Which option you chose is up to you. MenuResults have the advantage of compactness and do not require a deriving from CMenuItem. For larger commands, it may be preferable to use a separate class. Note that you are still free to use MenuResult values within an overridden Execute.


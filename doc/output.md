# Output

Any output a menu item writes is, by default, printed to the console. Output hooks allow intercepting this output. This can be useful to change of format it before printing, or to redirect or clone it to other locations (e.g. a logfile).

There are five kinds of output events that can be intercepted. They all refer to the equivalently named method or property of the `Console` class.

* Write
* WriteLine
* SetForegroundColor
* SetBackgroundColor
* ResetColor

It is recommended to let all custom menu items opt in to this pattern. For this purpose, simply call `OnWrite` instead of `Console.Write`, etc.

If no hook is present in a particular menu item, the call is redirected to its parent item. If no hook is found while moving up the chain, the regular method of the `Console` class is invoked. If a hook is found in a child item, the parent's hooks are not invoked.

Hooks can be added or removed at any time and for any menu item individually.

## Example

This class demonstrates using output hooks. It intercepts `Console.Write` commands and prints all lower-case letters with a red background. It would just as well be possible to copy the printed text to a log file.

	private class OutputHook : CMenuItem
	{
		public OutputHook () : base ("outputhook")
		{
			HelpText = "Demonstrates using output hooks.";
			Add ("add", s => Parent.Write += HookedWrite);
			Add ("remove", s => Parent.Write -= HookedWrite);
		}

		private static void HookedWrite (string s)
		{
			var fc = Console.ForegroundColor;
			var bc = Console.BackgroundColor;

			foreach (var c in s) {
				if (!char.IsLower (c)) {
					Console.Write (c);
				}
				else {
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.Red;
					Console.Write (c);
					Console.ForegroundColor = fc;
					Console.BackgroundColor = bc;
				}
			}
		}
	}

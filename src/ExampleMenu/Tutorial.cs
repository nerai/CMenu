using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	class Tutorial
	{
		private CMenu menu;

		public void Run ()
		{
			Basics ();
			CaseSensitivity ();
			InputModification ();
			InnerCommands ();
			NestedCommands ();
			SharingInInners ();
			IO.ImmediateInput ("help");
		}

		private void Basics ()
		{
			// Create menu
			menu = new CMenu ();

			// Add simple Hello World command
			menu.Add ("hello", s => Console.WriteLine ("Hello world!"));

			/*
			 * If the command happens to be more complex, you can just put it in a separate method.
			 */
			menu.Add ("len", s => PrintLen (s));

			/*
			 * It is also possible to return an exit code to signal that processing should be stopped.
			 * By default, the command "quit" exists for this purpose. Let's add an alternative way to stop processing input.
			 */
			menu.Add ("exit", s => menu.Quit ());

			/*
			 * To create a command with help text, simply add it during definition.
			 */
			menu.Add ("time",
				s => Console.WriteLine (DateTime.UtcNow),
				"Help for \"time\": Writes the current time");

			/*
			 * You can also access individual commands to edit them later, though this is rarely required.
			 */
			menu["time"].HelpText += " (UTC).";

			// Run menu. The menu will run until quit by the user.
			Console.WriteLine ("Enter \"help\" for help.");
			Console.WriteLine ("Enter \"quit\" to quit (in this case, the next step of this demo will be started).");
			menu.Run ();

			Console.WriteLine ("(First menu example completed, starting the next one...)");
		}

		private void PrintLen (string s)
		{
			Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
		}

		private void CaseSensitivity ()
		{
			/*
			 * Commands are case *in*sensitive by default. This can be changed using the `StringComparison` property.
			 */
			menu.StringComparison = StringComparison.InvariantCulture;
			menu.Add ("Hello", s => Console.WriteLine ("Hi!"));

			Console.WriteLine ("The menu is now case sensitive.");
			menu.Run ();
		}

		private void InputModification ()
		{
			/*
			 * It is also possible to modify the input queue.
			 * Check out how the "repeat" command adds its argument to the input queue two times.
			 */
			menu.Add ("repeat",
				s => {
					IO.ImmediateInput (s);
					IO.ImmediateInput (s);
				},
				"Repeats a command two times.");

			Console.WriteLine ("New command available: repeat");
			menu.Run ();
		}

		private void InnerCommands ()
		{
			var mi = menu.Add ("convert", "convert upper|lower [text]\nConverts the text to upper or lower case");
			mi.Add ("upper", s => Console.WriteLine (s.ToUpperInvariant ()), "Converts to upper case");
			mi.Add ("lower", s => Console.WriteLine (s.ToLowerInvariant ()), "Converts to lower case");

			Console.WriteLine ("New command <convert> available. It features the inner commands \"upper\" and \"lower\".");
			menu.Run ();
		}

		private void NestedCommands ()
		{
			menu.Add (new MI_Add ());

			Console.WriteLine ("New command <add> available.");
			IO.ImmediateInput ("help add");
			menu.Run ();
		}

		private void SharingInInners ()
		{
			/*
			 * If your inner menu items should share code, you need to overwrite the menu's Execute
			 * method, then call ExecuteChild to resume processing in child nodes.
			 *
			 * This allows you to alter the command received by the children, or to omit their
			 * processing altogether (e.g. in case a common verification failed).
			 */
			var m = menu.Add ("shared");
			m.SetAction (s => {
				Console.Write ("You picked: ");
				m.ExecuteChild (s);
			});
			m.Add ("1", s => Console.WriteLine ("Option 1"));
			m.Add ("2", s => Console.WriteLine ("Option 2"));

			Console.WriteLine ("New command <shared> available.");
			menu.Run ();
		}
	}
}

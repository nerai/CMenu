using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	/// <summary>
	/// Test program for CMenu.
	/// </summary>
	class Program
	{
		static CMenu menu;

		static void Main (string[] args)
		{
			Console.WriteLine ("Simple CMenu demonstration");

			Basics ();
			CaseSensitivity ();
			InputModification ();
			InnerCommands ();
			NestedCommands ();
			Examples ();
		}

		static void Basics ()
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
			menu.Add ("exit", s => MenuResult.Quit);

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

		static void PrintLen (string s)
		{
			Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
		}

		static void CaseSensitivity ()
		{
			/*
			 * Commands are case *in*sensitive by default. This can be changed using the `StringComparison` property.
			 */
			menu.StringComparison = StringComparison.InvariantCulture;
			menu.Add ("Hello", s => Console.WriteLine ("Hi!"));

			Console.WriteLine ("The menu is now case sensitive.");
			menu.Run ();
		}

		static void InputModification ()
		{
			/*
			 * It is also possible to modify the input queue.
			 * Check out how the "repeat" command adds its argument to the input queue two times.
			 */
			menu.Add ("repeat",
				s => Repeat (s),
				"Repeats a command two times.");

			Console.WriteLine ("New command available: repeat");
			menu.Run ();
		}

		static void Repeat (string s)
		{
			menu.Input (s, true);
			menu.Input (s, true);
		}

		static void InnerCommands ()
		{
			var mi = menu.Add ("convert", "convert upper|lower [text]\nConverts the text to upper or lower case");
			mi.Add ("upper", s => Console.WriteLine (s.ToUpperInvariant ()), "Converts to upper case");
			mi.Add ("lower", s => Console.WriteLine (s.ToLowerInvariant ()), "Converts to lower case");

			Console.WriteLine ("New command <convert> available. It features the inner commands \"upper\" and \"lower\".");
			menu.Run ();
		}

		static void NestedCommands ()
		{
			menu.Add (new MI_Add ());

			Console.WriteLine ("New command <add> available.");
			menu.Input ("help add", true);
			menu.Run ();
		}

		static void Examples ()
		{
			menu.Add (new MI_Echo ());
			menu.Add (new MI_If (menu));
			menu.Add (new MI_Pause ());
			menu.Add (new MI_Record ());
			menu.Add (new MI_Replay (menu));

			Console.WriteLine ("Several example commands were added to the menu.");
			menu.Input ("help", true);
			menu.Run ();
		}
	}
}

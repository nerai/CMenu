using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// Test program for CMenu.
	/// </summary>
	class Program
	{
		static void Main (string[] args)
		{
			Console.WriteLine ("Simple CMenu demonstration");
			Console.WriteLine ("Enter \"help\" for help.");

			// Create menu
			var menu = new CMenu ();

			// Add simple Hello World command.
			menu.AddMenuItem (new CMenuItem ("hello", s => Console.WriteLine ("Hello world!")));

			// Add command with behavior defined in separate method.
			menu.AddMenuItem (new CMenuItem ("len", s => Len (s)));

			// Add alternative way to stop processing input (by default, "quit" is provided).
			menu.AddMenuItem (new CMenuItem ("exit", s => MenuResult.Quit));

			// Add menu item with help text.
			menu.AddMenuItem (new CMenuItem (
				"time",
				s => Console.WriteLine (DateTime.UtcNow),
				"time\nWrites the current time (UTC)."));

			// Run menu. The menu will run until quit by the user.
			menu.Run ();

			Console.WriteLine ("Finished!");
		}

		private static void Len (string s)
		{
			Console.WriteLine ("String \"" + s + "\" has length " + s.Length);
		}
	}
}

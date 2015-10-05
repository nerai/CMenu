using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Replay : IMenuItem
	{
		private readonly CMenu _Menu;

		public MI_Replay (CMenu menu)
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;
		}

		public string Selector
		{
			get { return "replay"; }
		}

		public string HelpText
		{
			get
			{
				return "replay name\n"
					+ "Replays all commands stored in the specified file name.\n"
					+ "Replaying puts all stored commands in the same order on the stack as they were originally entered.\n"
					+ "\n"
					+ "Nested replaying is supported.\n";
			}
		}

		public MenuResult Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("You must enter a name to identify this command group.");
				return MenuResult.Normal;
			}

			Directory.CreateDirectory (".\\Records\\");
			var lines = File.ReadAllLines (".\\Records\\" + arg + ".txt");
			foreach (var line in lines) {
				_Menu.Input (line, false);
			}

			return MenuResult.Normal;
		}
	}
}

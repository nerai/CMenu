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
				return "replay\n"
					+ "XXX.";
			}
		}

		public MenuResult Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("You must enter a name to identify this command group.");
				// todo: besser: alle vorhandenen records auflisten
				return MenuResult.Normal;
			}

			Directory.CreateDirectory (".\\Records\\");
			var lines = File.ReadAllLines (".\\Records\\" + arg + ".txt"); // todo: file not found
			foreach (var line in lines) {
				_Menu.Input (line, false);
			}

			return MenuResult.Normal;
		}
	}
}

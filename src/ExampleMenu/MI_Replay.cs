using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Replay : CMenuItem
	{
		private readonly CMenu _Menu;

		private readonly IRecordStore _Store;

		public MI_Replay (CMenu menu, IRecordStore store)
			: base ("replay")
		{
			_Store = store;

			HelpText = ""
				+ "replay name\n"
				+ "Replays all commands stored in the specified file name.\n"
				+ "Replaying puts all stored commands in the same order on the stack as they were originally entered.\n"
				+ "\n"
				+ "Nested replaying is supported.";

			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;
		}

		public override MenuResult Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("You must enter a name to identify this command group.");
				return MenuResult.Normal;
			}

			var lines = _Store.GetRecord (arg);
			foreach (var line in lines) {
				_Menu.Input (line, false);
			}

			return MenuResult.Normal;
		}
	}
}

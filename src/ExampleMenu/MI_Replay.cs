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
				+ "replay [name]\n"
				+ "Replays all commands stored in the specified file name, or\n"
				+ "Displays a list of all records.\n"
				+ "\n"
				+ "Replaying puts all stored commands in the same order on the stack as they were originally entered.\n"
				+ "Nested replaying is supported.";

			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;
		}

		public string EndReplayCommand = "endreplay";

		public override MenuResult Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("Known records: " + string.Join (", ", _Store.GetRecordNames ()));
				return MenuResult.Normal;
			}

			var lines = _Store.GetRecord (arg);
			foreach (var line in lines) {
				// todo: normale comparison verwenden
				// todo: dynamisch (zB als body von if)
				if (line.Equals (EndReplayCommand)) {
					break;
				}
				_Menu.Input (line, false);
			}

			return MenuResult.Normal;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Examples.Recording
{
	public class MI_Replay : CMenuItem
	{
		private readonly CMenu _Menu;
		private readonly IRecordStore _Store;

		public string EndReplayCommand = "endreplay";

		public MI_Replay (CMenu menu, IRecordStore store)
			: base ("replay")
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}
			if (store == null) {
				throw new ArgumentNullException ("store");
			}

			_Store = store;

			HelpText = ""
				+ "replay [name]\n"
				+ "Replays all commands stored in the specified file name, or\n"
				+ "Displays a list of all records.\n"
				+ "\n"
				+ "Replaying puts all stored commands in the same order on the stack as they were originally entered.\n"
				+ "Replaying stops when the line \"" + EndReplayCommand + "\" is encountered.";

			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;
		}

		public override void Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("Known records: " + string.Join (", ", _Store.GetRecordNames ()));
				return;
			}

			var rec = _Store.GetRecord (arg);
			if (rec != null) {
				var lines = rec.TakeWhile (line => !line.Equals (EndReplayCommand));
				IO.AddInput (lines);
			}
		}
	}
}

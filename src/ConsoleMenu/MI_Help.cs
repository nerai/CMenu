using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	public class MI_Help : CMenuItem
	{
		private readonly CMenu _Menu;

		public MI_Help (CMenu menu)
			: base ("help")
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;

			HelpText = ""
				+ "help [command]\n"
				+ "Displays a help text for the specified command, or\n"
				+ "Displays a list of all available commands.\n";
		}

		public override MenuResult Execute (string arg)
		{
			if (arg == null) {
				throw new ArgumentNullException ("arg");
			}

			var cmd = MenuUtil.SplitFirstWord (ref arg);
			if (string.IsNullOrEmpty (cmd)) {
				Console.WriteLine ("Available commands:");
				var abbreviations = _Menu.CommandAbbreviations ().OrderBy (it => it.Key);
				foreach (var ab in abbreviations) {
					if (ab.Value == null) {
						Console.Write ("      ");
					}
					else {
						Console.Write (ab.Value.PadRight (3) + " | ");
					}
					Console.WriteLine (ab.Key);
				}
				Console.WriteLine ("Type \"help <command>\" for individual command help.");
			}
			else {
				var it = _Menu.GetMenuItem (cmd, true);
				if (it != null) {
					if (it.HelpText == null) {
						Console.WriteLine ("No help available for " + it.Selector);
					}
					else {
						Console.WriteLine (it.HelpText);
					}
				}
			}

			return MenuResult.Normal;
		}
	}
}

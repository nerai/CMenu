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
			if (cmd == "") {
				Console.WriteLine ("Available commands:");
				var commands = _Menu
					.Select (it => it.Selector)
					.OrderBy (it => it);
				foreach (var sel in commands) {
					var ab = GetAbbreviation (sel);
					string res;
					if (ab.Length < sel.Length - 1) {
						res = ab.PadRight (3) + " | ";
					}
					else {
						res = "      ";
					}
					res += sel;

					Console.WriteLine (res);
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

		private string GetAbbreviation (string cmd)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			for (int i = 1; i <= cmd.Length; i++) {
				var sub = cmd.Substring (0, i);
				if (_Menu.GetMenuItem (sub, false) != null) {
					return sub;
				}
			}
			return cmd;
		}
	}
}

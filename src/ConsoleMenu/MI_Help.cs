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
				DisplayAvailableCommands (_Menu, false);
			}
			else {
				var it = _Menu.GetMenuItem (cmd, true);
				if (it != null) {
					DisplayHelp (arg, it);
				}
			}

			return MenuResult.Normal;
		}

		private static void DisplayHelp (string arg, IMenuItem context)
		{
			if (arg == null) {
				throw new ArgumentNullException ("arg");
			}
			if (context == null) { //XXX?
				throw new ArgumentNullException ("context");
			}

			var cmd = MenuUtil.SplitFirstWord (ref arg);
			var mc = context as MenuItemCollection;

			if (string.IsNullOrEmpty (cmd)) {
				DisplayItemHelp (context);
				if (mc != null) {
					DisplayAvailableCommands (mc, true);
				}
			}
			else {
				if (mc == null) {
					DisplayItemHelp (context);
					if (!string.IsNullOrEmpty (arg)) {
						Console.WriteLine ("Inner command \"" + arg + "\" not found.");
					}
				}
				else {
					var inner = mc.GetMenuItem (cmd, true);
					if (inner == null) {
						return;
					}
					else {
						DisplayHelp (arg, inner);
					}
				}
			}
		}

		private static void DisplayItemHelp (IMenuItem item)
		{
			if (item.HelpText == null) {
				Console.WriteLine ("No help available for " + item.Selector);
			}
			else {
				Console.WriteLine (item.HelpText);
			}
		}

		private static void DisplayAvailableCommands (MenuItemCollection menu, bool inner)
		{
			if (!inner) {
				Console.WriteLine ("Available commands:");
			}
			var abbreviations = menu.CommandAbbreviations ().OrderBy (it => it.Key);
			foreach (var ab in abbreviations) {
				if (ab.Value == null) {
					Console.Write ("      ");
				}
				else {
					Console.Write (ab.Value.PadRight (3) + " | ");
				}
				Console.WriteLine (ab.Key);
			}
			if (!inner) {
				Console.WriteLine ("Type \"help <command>\" for individual command help.");
			}
		}
	}
}

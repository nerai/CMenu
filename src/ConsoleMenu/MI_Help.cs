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
			DisplayHelp (arg, _Menu, false);
			return MenuResult.Normal;
		}

		private static void DisplayHelp (string arg, CMenuItem context, bool isInner)
		{
			if (arg == null) {
				throw new ArgumentNullException ("arg");
			}
			if (context == null) {
				throw new ArgumentNullException ("context");
			}

			if (string.IsNullOrEmpty (arg)) {
				DisplayItemHelp (context, !context.Any ());
				DisplayAvailableCommands (context, isInner);
				return;
			}

			var cmd = arg;
			var inner = context.GetMenuItem (ref cmd, out arg, false, false);
			if (inner != null) {
				DisplayHelp (arg, inner, true);
				return;
			}

			DisplayItemHelp (context, true);
			if (!string.IsNullOrEmpty (arg)) {
				Console.WriteLine ("Inner command \"" + arg + "\" not found.");
			}
		}

		private static void DisplayItemHelp (CMenuItem item, bool force)
		{
			if (item == null) {
				throw new ArgumentNullException ("item");
			}

			if (item.HelpText == null) {
				if (force) {
					Console.WriteLine ("No help available for " + item.Selector);
				}
			}
			else {
				Console.WriteLine (item.HelpText);
			}
		}

		private static void DisplayAvailableCommands (MenuItemCollection menu, bool inner)
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

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

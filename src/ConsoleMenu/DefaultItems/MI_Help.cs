using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu.DefaultItems
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
				+ "Displays a list of all available commands.";
		}

		public override void Execute (string arg)
		{
			DisplayHelp (arg, _Menu, false);
		}

		private void DisplayHelp (string arg, CMenuItem context, bool isInner)
		{
			if (arg == null) {
				throw new ArgumentNullException ("arg");
			}
			if (context == null) {
				throw new ArgumentNullException ("context");
			}

			if (string.IsNullOrEmpty (arg)) {
				if (!DisplayItemHelp (context, !context.Any ())) {
					DisplayAvailableCommands (context, isInner);
				}
				return;
			}

			var cmd = arg;
			var inner = context.GetMenuItem (ref cmd, out arg, false, false, false);
			if (inner != null) {
				DisplayHelp (arg, inner, true);
				return;
			}

			OnWriteLine ("Could not find inner command \"" + cmd + "\".");
			if (context.Selector != null) {
				OnWriteLine ("Help for " + context.Selector + ":");
			}
			DisplayItemHelp (context, true);
		}

		private bool DisplayItemHelp (CMenuItem item, bool force)
		{
			if (item == null) {
				throw new ArgumentNullException ("item");
			}

			if (item.HelpText == null) {
				if (force) {
					OnWriteLine ("No help available for " + item.Selector);
				}
				return false;
			}
			else {
				OnWriteLine (item.HelpText);
				return true;
			}
		}

		private void DisplayAvailableCommands (CMenuItem menu, bool inner)
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			if (!inner) {
				OnWriteLine ("Available commands:");
			}
			var abbreviations = menu.CommandAbbreviations ().OrderBy (it => it.Key);
			foreach (var ab in abbreviations) {
				if (ab.Value == null) {
					OnWrite ("      ");
				}
				else {
					OnWrite (ab.Value.PadRight (3) + " | ");
				}
				OnWriteLine (ab.Key);
			}
			if (!inner) {
				OnWriteLine ("Type \"help <command>\" for individual command help.");
			}
		}
	}
}

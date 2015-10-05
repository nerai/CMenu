using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_If : IMenuItem
	{
		private readonly CMenu _Menu;

		public delegate bool ConditionCheck (ref string arg);

		public readonly Dictionary<string, ConditionCheck> Conditions = new Dictionary<string, ConditionCheck> ();

		public MI_If (CMenu menu)
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;

			Conditions.Add ("true", True);
			Conditions.Add ("false", False);
		}

		private bool True (ref string arg)
		{
			return true;
		}

		private bool False (ref string arg)
		{
			return false;
		}

		public string Selector
		{
			get { return "if"; }
		}

		public string HelpText
		{
			get
			{
				return "if\n"
					+ "XXX.";
			}
		}

		public MenuResult Execute (string arg)
		{
			var cond = CMenu.SplitFirstWord (ref arg);
			bool ok = false;
			bool invert = false;

			while ("not".Equals (cond, _Menu.StringComparison)) {
				invert = !invert;
				cond = CMenu.SplitFirstWord (ref arg);
			}

			ConditionCheck cc;
			if (!Conditions.TryGetValue (cond, out cc)) {
				Console.WriteLine ("Unknown condition: " + cond);
			}

			ok = cc (ref arg);
			ok ^= invert;

			if (ok) {
				_Menu.Input (arg, true);
			}

			return MenuResult.Normal;
		}
	}
}

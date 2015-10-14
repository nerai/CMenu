using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_If : CMenuItem
	{
		private readonly CMenu _Menu;

		public delegate bool ConditionCheck (ref string arg);

		public readonly Dictionary<string, ConditionCheck> Conditions = new Dictionary<string, ConditionCheck> ();

		public MI_If (CMenu menu)
			: base ("if")
		{
			HelpText = ""
				+ "if [not] <condition> <command>\n"
				+ "Executes <command> if <condition> is met.\n"
				+ "If the modifier <not> is given, the condition result is reversed.";
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

		public override MenuResult Execute (string arg)
		{
			var cond = MenuUtil.SplitFirstWord (ref arg);
			bool ok = false;
			bool invert = false;

			while ("not".Equals (cond, _Menu.StringComparison)) {
				invert = !invert;
				cond = MenuUtil.SplitFirstWord (ref arg);
			}

			ConditionCheck cc;
			if (!Conditions.TryGetValue (cond, out cc)) {
				Console.WriteLine ("Unknown condition: " + cond);
				return MenuResult.Normal;
			}

			ok = cc (ref arg);
			ok ^= invert;

			if (ok) {
				IO.ImmediateInput (arg);
			}

			return MenuResult.Normal;
		}
	}
}

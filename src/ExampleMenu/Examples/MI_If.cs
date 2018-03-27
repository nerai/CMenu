using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConsoleMenu;

namespace NConsoleMenu.Sample.Examples
{
	public class MI_If : CMenuItem
	{
		public delegate bool ConditionCheck (ref string arg);

		public readonly Dictionary<string, ConditionCheck> Conditions;

		public MI_If ()
			: base ("if")
		{
			HelpText = ""
				+ "if [not] <condition> <command>\n"
				+ "Executes <command> if <condition> is met.\n"
				+ "If the modifier <not> is given, the condition result is reversed.";

			Conditions = new Dictionary<string, ConditionCheck> (StringComparison.GetCorrespondingComparer ());
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

		public override StringComparison StringComparison
		{
			get {
				return base.StringComparison;
			}
			set {
				base.StringComparison = value;
			}
		}

		public override void Execute (string arg)
		{
			var cond = MenuUtil.SplitFirstWord (ref arg);
			bool ok = false;
			bool invert = false;

			while ("not".Equals (cond, StringComparison)) {
				invert = !invert;
				cond = MenuUtil.SplitFirstWord (ref arg);
			}

			ConditionCheck cc;
			if (!Conditions.TryGetValue (cond, out cc)) {
				OnWriteLine ("Unknown condition: " + cond);
				return;
			}

			ok = cc (ref arg);
			ok ^= invert;

			if (ok) {
				CQ.ImmediateInput (arg);
			}
		}
	}
}

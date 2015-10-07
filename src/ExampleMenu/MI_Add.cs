using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Add : CMenu
	{
		public MI_Add ()
			: base ("add")
		{
			HelpText = ""
				+ "add\n"
				+ "Adds numbers until \"=\" is entered.\n";

			Default = new CMenuItem (null, s => Add (s));
		}

		private int _Sum = 0;

		private MenuResult Add (string s)
		{
			if ("=".Equals (s)) {
				return MenuResult.Quit;
			}
			_Sum += int.Parse (s);
			return MenuResult.Normal;
		}

		public override MenuResult Execute (string arg)
		{
			Console.WriteLine ("Entering menu <Add>. Enter numbers. To print their sum and exit, enter \"=\".");
			Run ();
			Console.WriteLine ("Sum = " + _Sum);
			return MenuResult.Normal;
		}
	}
}

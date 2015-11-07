using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Pause : CMenuItem
	{
		public MI_Pause ()
			: base ("pause")
		{
			HelpText = ""
				+ "pause\n"
				+ "Stops further operation until the enter key is pressed.";
		}

		public override MenuResult Execute (string arg)
		{
			Console.ReadLine ();

			return MenuResult.Default;
		}
	}
}

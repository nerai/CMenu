using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConsoleMenu;

namespace NConsoleMenu.Sample.Examples
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

		public override void Execute (string arg)
		{
			Console.ReadLine ();
		}
	}
}

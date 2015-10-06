using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	public class MI_Quit : CMenuItem
	{
		public MI_Quit ()
			: base ("quit")
		{
			HelpText = ""
				+ "quit\n"
				+ "Quits menu processing.\n";
		}

		public override MenuResult Execute (string arg)
		{
			return MenuResult.Quit;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu.DefaultItems
{
	public class MI_Quit : CMenuItem
	{
		private readonly CMenu _Menu;

		public MI_Quit (CMenu menu)
			: base ("quit")
		{
			_Menu = menu;
			HelpText = ""
				+ "quit\n"
				+ "Quits menu processing.";
		}

		public override void Execute (string arg)
		{
			_Menu.Quit ();
		}
	}
}

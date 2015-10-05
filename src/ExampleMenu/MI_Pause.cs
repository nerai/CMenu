using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Pause : IMenuItem
	{
		public string Selector
		{
			get { return "pause"; }
		}

		public string HelpText
		{
			get
			{
				return "pause\n"
					+ "XXX.";
			}
		}

		public MenuResult Execute (string arg)
		{
			Console.ReadLine ();

			return MenuResult.Normal;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Echo : IMenuItem
	{
		public string Selector
		{
			get { return "echo"; }
		}

		public string HelpText
		{
			get
			{
				return "echo [text]\n"
					+ "Prints the specified text to stdout.\n";
			}
		}

		public MenuResult Execute (string arg)
		{
			Console.WriteLine (arg);
			return MenuResult.Normal;
		}
	}
}

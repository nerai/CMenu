using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Examples
{
	public class MI_Echo : CMenuItem
	{
		public MI_Echo ()
			: base ("echo")
		{
			HelpText = ""
				+ "echo [text]\n"
				+ "Prints the specified text to stdout.";
		}

		public override void Execute (string arg)
		{
			Console.WriteLine (arg);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NConsoleMenu;

namespace NConsoleMenu.Sample.Examples
{
	/// <summary>
	/// When the command takes a complex argument, comprised of
	/// multiple parts, a Regex is a simple and quite powerful way
	/// to handle it.
	/// 
	/// This command 'Greet' uses two parameters, a string and a
	/// number. Parsing them is done in the action body.
	/// </summary>
	public class MI_ComplexArgument : CMenuItem
	{
		public MI_ComplexArgument ()
			: base ("greet")
		{
			HelpText = "Greets a name a number of times.";
		}

		public override void Execute (string arg)
		{
			var regex = new Regex (@"(?<name>[^ ]*) (?<times>[0-9]*)");
			var match = regex.Match (arg);
			var name = match.Groups["name"].Value;
			var times = int.Parse (match.Groups["times"].Value);
			for (int i = 0; i < times; i++) {
				Console.WriteLine ($"Hello {name}!");
			}
		}
	}
}

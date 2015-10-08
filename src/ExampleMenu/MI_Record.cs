using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Record : CMenuItem
	{
		public string EndrecordCommand = "endrecord";

		public MI_Record ()
			: base ("record")
		{
			HelpText = ""
				+ "record name\n"
				+ "Records all subsequent commands to the specified file name.\n"
				+ "Recording can be stopped by the command \"" + EndrecordCommand + "\"\n"
				+ "Stored records can be played via the \"replay\" command.\n"
				+ "\n"
				+ "Nested recording is not supported.";
		}

		public override MenuResult Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("You must enter a name to identify this command group.");
				return MenuResult.Normal;
			}

			var lines = new List<string> ();
			for (; ; ) {
				var line = Console.ReadLine ();
				if (EndrecordCommand.Equals (line)) {
					break;
				}
				lines.Add (line);
			}

			Directory.CreateDirectory (".\\Records\\");
			File.WriteAllLines (".\\Records\\" + arg + ".txt", lines);

			return MenuResult.Normal;
		}
	}
}

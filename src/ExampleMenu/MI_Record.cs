using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Record : CMenu
	{
		private List<string> _Lines;

		private string _EndRecordCommand = "endrecord";

		private readonly IRecordStore _Store;

		public MI_Record (IRecordStore store)
			: base ("record")
		{
			_Store = store;

			HelpText = ""
				+ Selector + " name\n"
				+ "Records all subsequent commands to the specified file name.\n"
				+ "Recording can be stopped by the command \"" + EndRecordCommand + "\"\n"
				+ "Stored records can be played via the \"replay\" command.\n"
				+ "\n"
				+ "Nested recording is not supported.";

			Add (EndRecordCommand, s => MenuResult.Quit, "Finishes recording.");
			Add (null, s => _Lines.Add (s));
		}

		public string EndRecordCommand
		{
			get
			{
				return _EndRecordCommand;
			}
			set
			{
				this[_EndRecordCommand].Selector = value;
				_EndRecordCommand = value;
			}
		}

		public override MenuResult Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				Console.WriteLine ("You must enter a name to identify this command group.");
				return MenuResult.Normal;
			}

			_Lines = new List<string> ();
			Run ();
			_Store.AddRecord (arg, _Lines);
			_Lines = null;

			return MenuResult.Normal;
		}
	}
}

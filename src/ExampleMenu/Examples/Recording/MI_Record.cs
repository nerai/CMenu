using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Examples.Recording
{
	public class MI_Record : CMenu
	{
		private List<string> _Lines;

		private string _EndRecordCommand = "endrecord";

		private readonly IRecordStore _Store;

		public MI_Record (IRecordStore store)
			: base ("record", false)
		{
			if (store == null) {
				throw new ArgumentNullException ("store");
			}

			_Store = store;

			HelpText = ""
				+ Selector + " name\n"
				+ "Records all subsequent commands to the specified file name.\n"
				+ "Recording can be stopped by the command \"" + EndRecordCommand + "\"\n"
				+ "Stored records can be played via the \"replay\" command.\n"
				+ "\n"
				+ "Nested recording is not supported.";
			PromptCharacter = "record>";

			Add (EndRecordCommand, s => Quit (), "Finishes recording.");
			Add (null, s => _Lines.Add (s));
		}

		public string EndRecordCommand
		{
			get {
				return _EndRecordCommand;
			}
			set {
				this[_EndRecordCommand].Selector = value;
				_EndRecordCommand = value;
			}
		}

		public override void Execute (string arg)
		{
			if (string.IsNullOrWhiteSpace (arg)) {
				OnWriteLine ("You must enter a name to identify this command group.");
				return;
			}

			OnWriteLine ("Recording started. Enter \"" + EndRecordCommand + "\" to finish.");
			_Lines = new List<string> ();
			Run ();
			_Store.AddRecord (arg, _Lines);
			_Lines = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Proc : CMenu
	{
		private List<string> _Lines;

		private string _EndRecordCommand = "endproc";

		public MI_Proc ()
			: base ("proc")
		{
			PromptCharacter = "proc>";
			Add (EndRecordCommand, s => MenuResult.Quit);
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
				Console.WriteLine ("You must enter a name to identify this proc.");
				return MenuResult.Normal;
			}

			Console.WriteLine ("Recording started. Enter \"" + EndRecordCommand + "\" to finish.");
			_Lines = new List<string> ();
			Run ();
			ProcManager.Instance.Procs[arg] = _Lines;
			_Lines = null;

			return MenuResult.Normal;
		}
	}
}

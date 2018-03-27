using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NConsoleMenu;

namespace NConsoleMenu.Sample.Examples.Procedures
{
	public class MI_Proc : CMenu
	{
		private List<string> _Lines;
		private string _EndRecordCommand = "endproc";

		private readonly ProcManager _Mgr;

		public MI_Proc (ProcManager mgr)
			: base ("proc", false)
		{
			if (mgr == null) {
				throw new ArgumentNullException ("mgr");
			}

			_Mgr = mgr;

			PromptCharacter = "proc>";
			Add (EndRecordCommand, s => Quit ());
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
				OnWriteLine ("You must enter a name to identify this proc.");
				return;
			}

			OnWriteLine ("Recording started. Enter \"" + EndRecordCommand + "\" to finish.");
			_Lines = new List<string> ();
			Run ();
			_Mgr.AddProc (arg, _Lines);
			_Lines = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConsoleMenu;
using NConsoleMenu.Sample.Examples.Procedures;
using NConsoleMenu.Sample.Examples.Recording;

namespace NConsoleMenu.Sample.Examples
{
	class ExamplesMenu : CMenu
	{
		public ExamplesMenu ()
		{
			Selector = "examples";
			PromptCharacter = "examples>";

			Add (new MI_Add ());

			Add (new MI_Echo ());
			Add (new MI_ComplexArgument ());
			Add (new MI_If ());
			Add (new MI_Pause ());
			Add (new MI_Stopwatch ());

			var frs = new FileRecordStore ();
			Add (new MI_Record (frs));
			Add (new MI_Replay (this, frs));

			var procmgr = new ProcManager ();
			Add (new MI_Proc (procmgr));
			Add (new MI_Call (this, procmgr));
			Add (new MI_Return (this, procmgr));
			Add (new MI_Goto (procmgr));

			OnRun += m => {
				Console.Write ("Example menu - ");
				m.CQ.ImmediateInput ("help");
			};
		}
	}
}

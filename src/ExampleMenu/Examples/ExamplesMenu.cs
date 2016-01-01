using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;
using ExampleMenu.Procedures;
using ExampleMenu.Recording;

namespace ExampleMenu
{
	class ExamplesMenu : CMenu
	{
		public ExamplesMenu ()
			: base ("examples")
		{
			PromptCharacter = "examples>";

			Add (new MI_Add ());

			Add (new MI_Echo ());
			Add (new MI_If ());
			Add (new MI_Pause ());

			var frs = new FileRecordStore ();
			Add (new MI_Record (frs));
			Add (new MI_Replay (this, frs));

			var procmgr = new ProcManager ();
			Add (new MI_Proc (procmgr));
			Add (new MI_Call (this, procmgr));
			Add (new MI_Return (this, procmgr));
			Add (new MI_Goto (procmgr));
		}

		public override void Execute (string arg)
		{
			Console.Write ("Example menu - ");
			IO.ImmediateInput ("help");
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Examples.Procedures
{
	public class MI_Call : CMenuItem
	{
		private readonly CMenu _Menu;
		private readonly ProcManager _Mgr;

		public MI_Call (CMenu menu, ProcManager mgr)
			: base ("call")
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}
			if (mgr == null) {
				throw new ArgumentNullException ("mgr");
			}

			_Menu = menu;
			_Mgr = mgr;
		}

		public override void Execute (string arg)
		{
			CQ.AddInput (_Mgr.GenerateInputForProc (arg));
		}
	}
}

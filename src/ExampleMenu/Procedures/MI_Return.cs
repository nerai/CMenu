using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Procedures
{
	public class MI_Return : CMenuItem
	{
		private readonly ProcManager _Mgr;

		public MI_Return (CMenu menu, ProcManager mgr)
			: base ("return")
		{
			_Mgr = mgr;
		}

		public override MenuResult Execute (string arg)
		{
			_Mgr.Return ();
			return MenuResult.Normal;
		}
	}
}

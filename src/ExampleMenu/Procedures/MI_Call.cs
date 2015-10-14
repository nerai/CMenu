using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Procedures
{
	public class MI_Call : CMenuItem
	{
		private readonly CMenu _Menu;
		private readonly ProcManager _Mgr;

		public MI_Call (CMenu menu, ProcManager mgr)
			: base ("call")
		{
			_Menu = menu;
			_Mgr = mgr;
		}

		public override MenuResult Execute (string arg)
		{
			IO.AddInput (_Mgr.GenerateInput (arg));
			return MenuResult.Normal;
		}
	}
}

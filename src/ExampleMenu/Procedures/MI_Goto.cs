using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Procedures
{
	public class MI_Goto : CMenuItem
	{
		private readonly ProcManager _Mgr;

		public MI_Goto (ProcManager mgr)
			: base ("goto")
		{
			if (mgr == null) {
				throw new ArgumentNullException ("mgr");
			}

			_Mgr = mgr;
		}

		public override void Execute (string arg)
		{
			_Mgr.Jump (arg);
		}
	}
}

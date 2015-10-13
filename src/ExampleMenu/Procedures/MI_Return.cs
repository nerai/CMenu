using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Return : CMenuItem
	{
		public MI_Return (CMenu menu)
			: base ("return")
		{
		}

		public override MenuResult Execute (string arg)
		{
			ProcManager.Instance.ShouldReturn = true;
			return MenuResult.Normal;
		}
	}
}

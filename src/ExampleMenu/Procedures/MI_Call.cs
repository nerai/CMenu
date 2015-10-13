using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class MI_Call : CMenuItem
	{
		private readonly CMenu _Menu;

		public MI_Call (CMenu menu)
			: base ("call")
		{
			_Menu = menu;
		}

		public override MenuResult Execute (string arg)
		{
			var lines = ProcManager.Instance.Procs[arg];
			foreach (var line in lines) {
				_Menu.Input (line);
				if (ProcManager.Instance.ShouldReturn) {
					ProcManager.Instance.ShouldReturn = false;
					break;
				}
			}
			return MenuResult.Normal;
		}
	}
}

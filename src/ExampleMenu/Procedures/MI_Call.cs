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
		private readonly ProcManager _Mgr;

		public MI_Call (CMenu menu, ProcManager mgr)
			: base ("call")
		{
			_Menu = menu;
			_Mgr = mgr;
		}

		public override MenuResult Execute (string arg)
		{
			List<string> lines;
			if (!_Mgr.Procs.TryGetValue (arg, out lines)) {
				Console.WriteLine ("Unknown procedure: " + arg);
			}
			IO.AddInput (CreateInput (lines));
			return MenuResult.Normal;
		}

		private IEnumerable<string> CreateInput (List<string> lines)
		{
			foreach (var line in lines) {
				yield return line;
				if (_Mgr.ShouldReturn) {
					_Mgr.ShouldReturn = false;
					break;
				}
			}
		}
	}
}

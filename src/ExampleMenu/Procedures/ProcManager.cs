using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Procedures
{
	public class ProcManager
	{
		public readonly Dictionary<string, List<string>> Procs = new Dictionary<string, List<string>> ();

		public bool ShouldReturn = false;
	}
}

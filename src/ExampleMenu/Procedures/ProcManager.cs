using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	// todo refactor
	public class ProcManager
	{
		public static readonly ProcManager Instance = new ProcManager ();

		public readonly Dictionary<string, List<string>> Procs = new Dictionary<string, List<string>> ();

		public bool ShouldReturn = false;
	}
}

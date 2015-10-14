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
		private class Proc
		{
			public readonly List<string> Commands = new List<string> ();

			public Proc (IEnumerable<string> content)
			{
				Commands = new List<string> (content);
			}
		}

		private readonly Dictionary<string, Proc> _Procs = new Dictionary<string, Proc> ();

		private bool _RequestReturn = false;

		public void AddProc (string name, IEnumerable<string> content)
		{
			var proc = new Proc (content);
			_Procs.Add (name, proc);
		}

		public IEnumerable<string> GenerateInput (string procname)
		{
			Proc proc;
			if (!_Procs.TryGetValue (procname, out proc)) {
				Console.WriteLine ("Unknown procedure: " + proc);
				yield break;
			}

			int i = 0;
			while (i < proc.Commands.Count) {
				var line = proc.Commands[i];
				yield return line;

				if (_RequestReturn) {
					_RequestReturn = false;
					break;
				}
			}
		}

		public void Return ()
		{
			_RequestReturn = true;
		}
	}
}

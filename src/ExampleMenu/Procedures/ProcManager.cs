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
			public readonly Dictionary<string, int> JumpMarks = new Dictionary<string, int> ();

			public Proc (IEnumerable<string> content)
			{
				Commands = new List<string> (content);

				for (int i = 0; i < Commands.Count; i++) {
					var s = Commands[i];
					if (s.StartsWith (":")) {
						s = s.Substring (1);
						var name = MenuUtil.SplitFirstWord (ref s);
						JumpMarks[name] = i;
						Commands[i] = s;
					}
				}
			}
		}

		private readonly Dictionary<string, Proc> _Procs = new Dictionary<string, Proc> ();

		private bool _RequestReturn = false;
		private string _RequestJump = null;

		public void AddProc (string name, IEnumerable<string> content)
		{
			if (_Procs.ContainsKey (name)) {
				Console.WriteLine ("Procedure \"" + name + "\" is already defined.");
				return;
			}

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
				i++;

				if (_RequestReturn) {
					_RequestReturn = false;
					break;
				}
				if (_RequestJump != null) {
					i = proc.JumpMarks[_RequestJump]; // todo check
					_RequestJump = null;
				}
			}
		}

		public void Return ()
		{
			_RequestReturn = true;
		}

		public void Jump (string mark)
		{
			_RequestJump = mark;
		}
	}
}

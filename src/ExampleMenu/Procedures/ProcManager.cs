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
				if (content == null) {
					throw new ArgumentNullException ("content");
				}

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
			if (name == null) {
				throw new ArgumentNullException ("name");
			}
			if (content == null) {
				throw new ArgumentNullException ("content");
			}

			if (_Procs.ContainsKey (name)) {
				Console.WriteLine ("Procedure \"" + name + "\" is already defined.");
				return;
			}

			var proc = new Proc (content);
			_Procs.Add (name, proc);
		}

		public IEnumerable<string> GenerateInputForProc (string name)
		{
			if (name == null) {
				throw new ArgumentNullException ("procname");
			}

			Proc proc;
			if (!_Procs.TryGetValue (name, out proc)) {
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
					int to;
					if (proc.JumpMarks.TryGetValue (_RequestJump, out to)) {
						i = to;
					}
					else {
						Console.WriteLine ("Could not find jump target \"" + _RequestJump + "\", aborting.");
						yield break;
					}
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

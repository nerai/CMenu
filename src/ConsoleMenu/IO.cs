using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	// todo doc
	public static class IO
	{
		private class Frame
		{
			public IEnumerator<string> E;

			public Frame (IEnumerable<string> source)
			{
				E = source.GetEnumerator ();
			}
		}

		private static readonly Stack<Frame> _Frames = new Stack<Frame> ();
		private static readonly Stack<string> _PromptCharacters = new Stack<string> ();

		static IO ()
		{
			AddInput (DefaultInputSource ());
		}

		// todo doc
		public static string QueryInput ()
		{
			for (; ; ) {
				var f = _Frames.Peek ();
				while (!f.E.MoveNext ()) {
					_Frames.Pop ();
					f = _Frames.Peek ();
				}

				var input = f.E.Current;
				if (!string.IsNullOrWhiteSpace (input)) {
					return input;
				}
			}
		}

		private static IEnumerable<string> DefaultInputSource ()
		{
			for (; ; ) {
				if (_PromptCharacters.Any ()) {
					var prompt = _PromptCharacters.Peek ();
					if (prompt != null) {
						Console.Write (prompt + " ");
					}
				}
				yield return Console.ReadLine ();
			}
		}

		// todo doc
		public static void AddInput (IEnumerable<string> source)
		{
			_Frames.Push (new Frame (source));
		}

		// todo doc
		public static void ImmediateInput (string source)
		{
			AddInput (new string[] { source });
		}

		// todo doc
		public static void PushPromptCharacter (string prompt)
		{
			_PromptCharacters.Push (prompt);
		}

		// todo doc
		public static void PopPromptCharacter ()
		{
			_PromptCharacters.Pop ();
		}
	}
}

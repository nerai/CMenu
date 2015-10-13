using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
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

		static IO ()
		{
			AddInput (DefaultInputSource ());
		}

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
				yield return Console.ReadLine ();
			}
		}

		public static void AddInput (IEnumerable<string> source)
		{
			_Frames.Push (new Frame (source));
		}

		public static void ImmediateInput (string source)
		{
			AddInput (new string[] { source });
		}
	}
}

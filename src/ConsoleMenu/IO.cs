using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	public static class IO
	{
		private static readonly Stack<IEnumerator<string>> _Frames = new Stack<IEnumerator<string>> ();

		static IO ()
		{
			AddInput (DefaultInputSource ());
		}

		public static string QueryInput ()
		{
			var f = _Frames.Peek ();
			while (!f.MoveNext ()) {
				_Frames.Pop ();
				f = _Frames.Peek ();
			}
			return f.Current;
		}

		private static IEnumerable<string> DefaultInputSource ()
		{
			for (; ; ) {
				yield return Console.ReadLine ();
			}
		}

		public static void AddInput (IEnumerable<string> source)
		{
			_Frames.Push (source.GetEnumerator ());
		}

		public static void ImmediateInput (string source)
		{
			AddInput (new string[] { source });
		}
	}
}

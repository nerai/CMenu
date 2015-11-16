using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// Provides global I/O functions in the context of CMenu.
	/// </summary>
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

		/// <summary>
		/// Returns the next available line of input.
		/// </summary>
		/// <param name="prompt">
		/// String to prompt, or null.
		/// </param>
		public static string QueryInput (string prompt)
		{
			for (; ; ) {
				var input = GetNextFrameInput ();

				if (input == null) {
					if (prompt != null) {
						Console.Write (prompt + " ");
					}
					input = Console.ReadLine ();
				}

				if (!string.IsNullOrWhiteSpace (input)) {
					return input;
				}
			}
		}

		private static string GetNextFrameInput ()
		{
			while (_Frames.Any ()) {
				var f = _Frames.Peek ();
				if (!f.E.MoveNext ()) {
					_Frames.Pop ();
					continue;
				}
				return f.E.Current;
			}
			return null;
		}

		/// <summary>
		/// Adds a new input source on top of the input stack.
		///
		/// This source will be used until it is exhausted, then the previous source will be used in the same manner.
		/// </summary>
		public static void AddInput (IEnumerable<string> source)
		{
			if (source == null) {
				throw new ArgumentNullException ("source");
			}

			_Frames.Push (new Frame (source));
		}

		/// <summary>
		/// Puts a single line of input on top of the stack.
		/// </summary>
		public static void ImmediateInput (string source)
		{
			if (source == null) {
				throw new ArgumentNullException ("source");
			}

			AddInput (new string[] { source });
		}
	}
}

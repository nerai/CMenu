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
		private static readonly Stack<string> _PromptCharacters = new Stack<string> ();

		static IO ()
		{
			AddInput (DefaultInputSource ());
		}

		/// <summary>
		/// Returns the next available line of input.
		/// </summary>
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

		/// <summary>
		/// Sets the current prompt character in case of console input.
		/// </summary>
		/// <param name="prompt">
		/// String to prompt, or null.
		/// </param>
		public static void PushPromptCharacter (string prompt)
		{
			_PromptCharacters.Push (prompt);
		}

		/// <summary>
		/// Restores the prompt character valid prior to its last change.
		/// </summary>
		public static void PopPromptCharacter ()
		{
			_PromptCharacters.Pop ();
		}
	}
}

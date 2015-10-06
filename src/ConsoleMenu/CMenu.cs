using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// A console menu structure, comprised of various menu items.
	///
	/// <example>
	/// Create a menu which can display the time:
	/// <code>
	/// var menu = new CMenu ();
	/// menu.Add ("time", s => Console.WriteLine (DateTime.UtcNow));
	/// menu.Run ();
	/// </code>
	/// </example>
	/// </summary>
	public class CMenu : MenuItemCollection
	{
		private readonly List<string> _InputQueue = new List<string> ();

		/// <summary>
		/// Create a new CMenu.
		///
		/// <para>
		/// The menu will initially contain the following commands:
		/// <list type="bullet">
		/// <item>help</item>
		/// <item>quit</item>
		/// </list>
		/// </para>
		/// </summary>
		public CMenu ()
		{
			Add (new MI_Quit ());
			Add (new MI_Help (this));
		}

		/// <summary>
		/// Start console promting and processing.
		/// </summary>
		public void Run ()
		{
			while (true) {
				string input;
				if (_InputQueue.Count > 0) {
					input = _InputQueue.First ();
					_InputQueue.RemoveAt (0);
				}
				else {
					Console.Write ("$ ");
					input = Console.ReadLine ();
				}

				if (string.IsNullOrWhiteSpace (input)) {
					continue;
				}

				var result = ExecuteInner (input);
				if (result == MenuResult.Quit) {
					break;
				}
			}
		}

		/// <summary>
		/// Add line to input queue.
		/// </summary>
		/// <param name="line">
		/// The line to add to the input queue.
		/// </param>
		public void Input (string line, bool atBeginning)
		{
			if (atBeginning) {
				_InputQueue.Insert (0, line);
			}
			else {
				_InputQueue.Add (line);
			}
		}
	}
}

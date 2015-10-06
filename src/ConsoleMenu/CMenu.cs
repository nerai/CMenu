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
		public static string SplitFirstWord (ref string from)
		{
			var split = from.Split (new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
			from = split.Length > 1 ? split[1].TrimStart () : "";
			var word = split.Length > 0 ? split[0].Trim () : "";
			return word;
		}

		private readonly List<string> _InputQueue = new List<string> ();

		private void DisplayHelp (string command)
		{
			if (command == null) {
				throw new ArgumentNullException ("command");
			}

			var cmd = SplitFirstWord (ref command);
			if (cmd == "") {
				var cmds = _Menu
					.Select (it => {
						var sel = it.Selector;
						var ab = GetAbbreviation (sel);
						string res;
						if (ab.Length < sel.Length - 1) {
							res = ab.PadRight (3) + " | ";
						}
						else {
							res = "      ";
						}
						res += sel;
						return res;
					})
					.OrderBy (it => it.TrimStart ());
				Console.WriteLine ("Available commands:");
				foreach (var it in cmds) {
					Console.WriteLine (it);
				}
				Console.WriteLine ("Type \"help <command>\" for individual command help.");
			}
			else {
				var it = GetMenuItem (cmd, true);
				if (it != null) {
					if (it.HelpText == null) {
						Console.WriteLine ("No help available for " + it.Selector);
					}
					else {
						Console.WriteLine (it.HelpText);
					}
				}
			}
		}

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
			var helphelp = ""
				+ "help [command]\n"
				+ "Displays a help text for the specified command, or\n"
				+ "Displays a list of all available commands.\n";
			Add (new CMenuItem ("help", s => DisplayHelp (s), helphelp));

			var helpquit = ""
				+ "quit\n"
				+ "Quits menu processing.\n";
			Add (new CMenuItem ("quit", s => MenuResult.Quit, helpquit));
		}

		private string GetAbbreviation (string cmd)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			for (int i = 1; i <= cmd.Length; i++) {
				var sub = cmd.Substring (0, i);
				if (GetMenuItem (sub, false) != null) {
					return sub;
				}
			}
			return cmd;
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

				var cmd = SplitFirstWord (ref input);
				var it = GetMenuItem (cmd, true);
				if (it == null) {
					continue;
				}

				var result = it.Execute (input);

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

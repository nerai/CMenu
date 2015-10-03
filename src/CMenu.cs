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
	public class CMenu
	{
		private static string SplitFirstWord (ref string from)
		{
			var split = from.Split (new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
			from = split.Length > 1 ? split[1].TrimStart () : "";
			var word = split.Length > 0 ? split[0].Trim () : "";
			return word;
		}

		private readonly List<CMenuItem> _Menu = new List<CMenuItem> ();
		private readonly Queue<string> _InputQueue = new Queue<string> ();

		public StringComparison StringComparison { get; set; }

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
			StringComparison = StringComparison.InvariantCultureIgnoreCase;

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

		/// <summary>
		/// Gets or sets the CMenuItem associated with the specified keyword.
		/// </summary>
		/// <param name="key">
		/// Keyword of the CMenuItem. The selector must match perfectly (i.e. is not an abbreviation of the keyword).
		/// </param>
		/// <value>
		/// The CMenuItem associated with the specified keyword, or null.
		/// </value>
		/// <returns>
		/// The menu item associated with the specified keyword.
		/// </returns>
		public CMenuItem this[string key]
		{
			get
			{
				if (key == null) {
					throw new ArgumentNullException ("key");
				}

				var item = _Menu.FirstOrDefault (it => it.Selector.Equals (key, StringComparison));
				if (item == null) {
					item = new CMenuItem (key);
					_Menu.Add (item);
				}
				return item;
			}
			set
			{
				if (key == null) {
					throw new ArgumentNullException ("key");
				}

				var old = this[key];
				if (old != null) {
					_Menu.Remove (old);
				}
				if (value != null) {
					_Menu.Add (value);
				}
			}
		}

		/// <summary>
		/// Add new command.
		///
		/// The menu's internal structure and abbreviations are updated automatically.
		/// </summary>
		/// <param name="it">Command to add.</param>
		public void Add (CMenuItem it)
		{
			if (it == null) {
				throw new ArgumentNullException ("it");
			}

			_Menu.Add (it);
		}

		/// <summary>
		/// Adds a new command from keyword, behavior and help.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected. The behavior provides feedback to the menu.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem Add (string selector, Func<string, MenuResult> execute, string help = null)
		{
			var it = new CMenuItem (selector, execute, help);
			Add (it);
			return it;
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword, behavior and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem Add (string selector, Action<string> execute, string help = null)
		{
			var it = new CMenuItem (selector, execute, help);
			Add (it);
			return it;
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

		private CMenuItem GetMenuItem (string cmd, bool complain)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			var from = _Menu.Where (it => it.Execute != null);

			var its = from
				.Where (it => cmd.Equals (it.Selector, StringComparison))
				.ToArray ();
			if (its.Length == 0) {
				its = from
					.Where (it => cmd.StartsWith (it.Selector, StringComparison))
					.OrderBy (it => it.Selector)
					.ToArray ();
			}

			if (its.Length == 1) {
				return its[0];
			}

			if (complain) {
				if (its.Length == 0) {
					Console.WriteLine ("Unknown command: " + cmd);
				}
				else {
					Console.WriteLine (
						"Command <" + cmd + "> not unique. Candidates: " +
						string.Join (", ", its.Select (it => it.Selector)));
				}
			}

			return null;
		}

		/// <summary>
		/// Start console promting and processing.
		/// </summary>
		public void Run ()
		{
			while (true) {
				string input;
				if (_InputQueue.Count > 0) {
					input = _InputQueue.Dequeue ();
				}
				else {
					Console.Write ("$ ");
					input = Console.ReadLine ();
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
		public void Input (string line)
		{
			_InputQueue.Enqueue (line);
		}
	}
}

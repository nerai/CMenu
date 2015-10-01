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
	/// menu.AddMenuItem (new CMenuItem ("time", s => Console.WriteLine (DateTime.UtcNow)));
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

		private void DisplayHelp (string s)
		{
			var cmd = SplitFirstWord (ref s);
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
			AddMenuItem (new CMenuItem ("help", s => DisplayHelp (s), helphelp));

			var helpquit = ""
				+ "quit\n"
				+ "Quits menu processing.\n";
			AddMenuItem (new CMenuItem ("quit", s => MenuResult.Quit, helpquit));
		}

		// todo desc
		// todo key null
		public CMenuItem this[string key]
		{
			get
			{
				var item = _Menu.FirstOrDefault (it => it.Selector.Equals (key, StringComparison.InvariantCultureIgnoreCase));
				if (item == null) {
					item = new CMenuItem (key);
					_Menu.Add (item);
				}
				return item;
			}
			set
			{
				var old = this[key];
				if (old != null) {
					_Menu.Remove (old);
				}
				_Menu.Add (value);
			}
		}

		/// <summary>
		/// Add new command. Internal structure and abbreviations are updated automatically.
		/// </summary>
		/// <param name="it">Command to add.</param>
		public void AddMenuItem (CMenuItem it)
		{
			_Menu.Add (it);
		}

		private string GetAbbreviation (string s)
		{
			for (int i = 1; i <= s.Length; i++) {
				var sub = s.Substring (0, i);
				if (GetMenuItem (sub, false) != null) {
					return sub;
				}
			}
			return s;
		}

		private CMenuItem GetMenuItem (string cmd, bool complain)
		{
			var its = _Menu
				.Where (it => it.Execute != null)
				.Where (it => it.Selector.StartsWith (cmd, StringComparison.InvariantCultureIgnoreCase))
				.ToArray ();
			if (its.Length == 1) {
				return its[0];
			}

			if (complain) {
				if (its.Length == 0) {
					Console.WriteLine ("Unknown command: " + cmd);
				}
				else {
					Console.WriteLine ("Command <" + cmd + "> not unique.");
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
				Console.Write ("$ ");
				var line = Console.ReadLine ();
				var cmd = SplitFirstWord (ref line);

				var it = GetMenuItem (cmd, true);
				if (it == null) {
					continue;
				}

				var result = it.Execute (line);

				if (result == MenuResult.Quit) {
					break;
				}
			}
		}
	}
}

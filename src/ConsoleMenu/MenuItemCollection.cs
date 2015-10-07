using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// A collection of IMenuItems
	///
	/// Offers various ways to add, retrieve and use items.
	/// </summary>
	public class MenuItemCollection : IEnumerable<CMenuItem>
	{
		// todo descr of detault
		// todo clarifications and/or refactoring about where this is used
		// todo change all null checks appropriately
		// todo check all users of ienumerable
		private readonly DefaultDictionary<string, CMenuItem> _Menu = new DefaultDictionary<string, CMenuItem> ();

		private StringComparison _StringComparison;

		/// <summary>
		/// Gets or sets how entered commands are compared.
		///
		/// By default, the comparison is case insensitive and culture invariant.
		/// </summary>
		public StringComparison StringComparison
		{
			get
			{
				return _StringComparison;
			}
			set
			{
				_StringComparison = value;
				_Menu.SetComparer (value.GetCorrespondingComparer ());
			}
		}

		/// <summary>
		/// Create a new, empty MenuItemCollection
		/// </summary>
		public MenuItemCollection ()
		{
			StringComparison = StringComparison.InvariantCultureIgnoreCase;
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
				return _Menu.TryGetValue (key);
			}
			set
			{
				_Menu[key] = value;
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

			_Menu.Add (it.Selector, it);
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
		/// Adds a new command from keyword and help.
		/// </summary>
		/// <param name="selector">Keyword</param>
		public CMenuItem Add (string selector, string help = null)
		{
			return Add (selector, (Func<string, MenuResult>) null, help);
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

		private CMenuItem[] GetCommands (string cmd, StringComparison comparison)
		{
			var mi = _Menu.TryGetValue (cmd);
			if (mi != null) {
				return new[] { mi };
			}

			var its = _Menu.Values
				.Where (it => it.Selector.StartsWith (cmd, comparison))
				.OrderBy (it => it.Selector)
				.ToArray ();
			return its;
		}

		/// <summary>
		/// Retrieves the IMenuItem associated with the specified keyword.
		///
		/// If no single item matches perfectly, the search will broaden to all items starting with the keyword.
		///
		/// In case sensitive mode, missing match which could be solved by different casing will re reported if complain is specified.
		/// </summary>
		/// <param name="cmd">A keyword that uniquely identifies the searched menu item</param> TODO
		/// <param name="complain">If true, clarifications about missing or superfluous matches will be written to stdout</param>
		/// <returns>The single closest matching menu item, or null in case of 0 or multiple matches</returns>
		public CMenuItem GetMenuItem (string line, out string args, bool complain, bool useDefault) // todo doc
		{
			if (line == null) {
				throw new ArgumentNullException ("line");
			}

			args = line;
			var cmd = MenuUtil.SplitFirstWord (ref args);

			var its = GetCommands (cmd, StringComparison);

			if (its.Length == 1) {
				return its[0];
			}

			if (its.Length > 1) {
				if (complain) {
					Console.WriteLine (
						"Command <" + cmd + "> not unique. Candidates: " +
						string.Join (", ", its.Select (it => it.Selector)));
				}
				return null;
			}

			var def = _Menu[null];
			if (def != null) {
				args = line;
				return def;
			}

			if (complain) {
				Console.WriteLine ("Unknown command: " + cmd);

				if (StringComparison.IsCaseSensitive ()) {
					var suggestions = GetCommands (cmd, StringComparison.InvariantCultureIgnoreCase);
					if (suggestions.Length > 0) {
						if (suggestions.Length == 1) {
							Console.WriteLine ("Did you mean \"" + suggestions[0].Selector + "\"?");
						}
						else if (suggestions.Length <= 5) {
							Console.Write ("Did you mean ");
							Console.Write (string.Join (", ", suggestions.Take (suggestions.Length - 1).Select (sug => "\"" + sug.Selector + "\"")));
							Console.Write (" or \"" + suggestions.Last ().Selector + "\"?");
							Console.WriteLine ();
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Executes the command specified in the argument and returns its result.
		/// </summary>
		/// <param name="args">Command to execute using contained commands.</param>
		/// <returns>The result of execution, or <c>MenuResult.Normal</c> in case of errors.</returns>
		protected MenuResult ExecuteInner (string args)
		{
			var it = GetMenuItem (args, out args, true, true);
			if (it != null) {
				return it.Execute (args);
			}
			return MenuResult.Normal;
		}

		public IEnumerator<CMenuItem> GetEnumerator ()
		{
			return _Menu.Values.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IDictionary<string, string> CommandAbbreviations ()
		{
			var dict = new Dictionary<string, string> ();

			foreach (var it in _Menu.Values) {
				var sel = it.Selector;
				var ab = GetAbbreviation (sel);
				if (ab.Length >= sel.Length - 1) {
					ab = null;
				}
				dict.Add (sel, ab);
			}

			return dict;
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
	}
}

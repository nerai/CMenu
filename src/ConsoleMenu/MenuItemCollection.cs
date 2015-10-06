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
	public class MenuItemCollection : IEnumerable<IMenuItem>
	{
		private readonly List<IMenuItem> _Menu = new List<IMenuItem> ();

		/// <summary>
		/// Gets or sets how entered commands are compared.
		///
		/// By default, the comparison is case insensitive and culture invariant.
		/// </summary>
		public StringComparison StringComparison { get; set; }

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
		public IMenuItem this[string key]
		{
			get
			{
				if (key == null) {
					throw new ArgumentNullException ("key");
				}

				var item = _Menu.FirstOrDefault (it => it.Selector.Equals (key, StringComparison));
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
		public void Add (IMenuItem it)
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

		private IMenuItem[] GetCommands (string cmd, StringComparison comparison)
		{
			var its = _Menu
				.Where (it => it.Selector.Equals (cmd, comparison))
				.ToArray ();
			if (its.Length == 0) {
				its = _Menu
					.Where (it => it.Selector.StartsWith (cmd, comparison))
					.OrderBy (it => it.Selector)
					.ToArray ();
			}
			return its;
		}

		/// <summary>
		/// Retrieves the IMenuItem associated with the specified keyword.
		///
		/// If no single item matches perfectly, the search will broaden to all items starting with the keyword.
		///
		/// In case sensitive mode, missing match which could be solved by different casing will re reported if complain is specified.
		/// </summary>
		/// <param name="cmd">A keyword that uniquely identifies the searched menu item</param>
		/// <param name="complain">If true, clarifications about missing or superfluous matches will be written to stdout</param>
		/// <returns>The single closest matching menu item, or null in case of 0 or multiple matches</returns>
		protected IMenuItem GetMenuItem (string cmd, bool complain)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			var its = GetCommands (cmd, StringComparison);

			if (its.Length == 1) {
				return its[0];
			}

			if (complain) {
				if (its.Length == 0) {
					Console.WriteLine ("Unknown command: " + cmd);
					if (false
						|| StringComparison == StringComparison.CurrentCulture
						|| StringComparison == StringComparison.InvariantCulture
						|| StringComparison == StringComparison.Ordinal) {
						var suggestions = GetCommands (cmd, StringComparison.InvariantCultureIgnoreCase);
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
				else {
					Console.WriteLine (
						"Command <" + cmd + "> not unique. Candidates: " +
						string.Join (", ", its.Select (it => it.Selector)));
				}
			}

			return null;
		}

		/// <summary>
		/// Executes the command specified in the argument and returns its result.
		/// </summary>
		/// <param name="arg">Command to execute using contained commands.</param>
		/// <returns>The result of execution, or <c>MenuResult.Normal</c> in case of errors.</returns>
		protected MenuResult ExecuteInner (string arg)
		{
			var cmd = MenuUtil.SplitFirstWord (ref arg);

			var it = GetMenuItem (cmd, true);
			if (it != null) {
				return it.Execute (arg);
			}

			return MenuResult.Normal;
		}

		public IEnumerator<IMenuItem> GetEnumerator ()
		{
			return _Menu.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}

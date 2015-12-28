using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// A single console menu item. It consists of a selector (keyword), a help text and the individual behavior.
	///
	/// Also offers various ways to add, retrieve and use subitems.
	///
	/// <example>
	/// Create a hellow world command:
	/// <code>
	/// var menuitem = new CMenuItem ("hello", s => Console.WriteLine ("Hello world!"));
	/// </code>
	/// </example>
	/// </summary>
	public class CMenuItem : IEnumerable<CMenuItem>
	{
		private Dictionary<string, CMenuItem> _Menu = new Dictionary<string, CMenuItem> (StringComparer.InvariantCultureIgnoreCase);
		private CMenuItem _Default = null;

		private StringComparison? _StringComparison;

		/// <summary>
		/// Parent of this item, if any.
		/// </summary>
		public CMenuItem Parent { get; private set; }

		/// <summary>
		/// This menu item.
		///
		/// <remarks>
		/// This property can be used to combine object and collection initializers.
		/// <example>
		/// var m = new CMenuItem ("parent") {
		///	HelpText = "help", // object initializer
		///	MenuItem = { // collection initializer
		///		new CMenuItem ("child 1"),
		///		new CMenuItem ("child 2"),
		///	}
		/// };
		/// </example>
		/// </remarks>
		/// </summary>
		public CMenuItem MenuItem { get { return this; } }

		/// <summary>
		/// Gets or sets how entered commands are compared.
		///
		/// By default, the comparison is case insensitive and culture invariant.
		/// </summary>
		public virtual StringComparison StringComparison
		{
			get
			{
				if (_StringComparison.HasValue) {
					return _StringComparison.Value;
				}
				if (Parent != null) {
					return Parent.StringComparison;
				}
				return StringComparison.InvariantCultureIgnoreCase;
			}
			set
			{
				_StringComparison = value;
				_Menu = new Dictionary<string, CMenuItem> (_Menu, value.GetCorrespondingComparer ());
			}
		}

		/// <summary>
		/// Keyword to select this item.
		/// </summary>
		public string Selector
		{
			get;
			set;
		}

		/// <summary>
		/// Descriptive help text.
		/// </summary>
		public string HelpText
		{
			get;
			set;
		}

		private Action<string> _Execute;

		/// <summary>
		/// Sets the behavior upon selection
		/// </summary>
		/// <param name="action">
		/// Behavior when selected.
		/// </param>
		public void SetAction (Action<string> action)
		{
			_Execute = action;
		}

		/// <summary>
		/// Behavior upon selection.
		///
		/// By default, if present, this node's behavior will be executed.
		/// Else, execution will be delegated to the appropriate child.
		/// </summary>
		public virtual void Execute (string arg)
		{
			if (_Execute != null) {
				_Execute (arg);
				return;
			}

			if (this.Any ()) {
				ExecuteChild (arg);
			}
			else {
				throw new NotImplementedException ("This menu item does not have an associated behavior yet.");
			}
		}

		private Func<bool> _Visible;

		public void SetVisibilityCondition (Func<bool> condition)
		{
			_Visible = condition;
		}

		public virtual bool IsVisible ()
		{
			if (_Visible != null) {
				return _Visible ();
			}

			return true;
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword, behavior and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem (string selector, Action<string> execute, string help = null)
		{
			Selector = selector;
			HelpText = help;
			SetAction (execute);
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword.
		/// </summary>
		/// <param name="selector">Keyword</param>
		public CMenuItem (string selector)
			: this (selector, (Action<string>) null)
		{ }

		/// <summary>
		/// Add new command.
		///
		/// The menu's internal structure and abbreviations are updated automatically.
		/// </summary>
		/// <param name="it">Command to add.</param>
		/// <returns>The added CMenuItem</returns>
		public T Add<T> (T it) where T : CMenuItem
		{
			if (it == null) {
				throw new ArgumentNullException ("it");
			}
			if (it.Parent != null) {
				throw new ArgumentException ("Menuitem already has a parent.", "it");
			}

			if (it.Selector != null) {
				_Menu.Add (it.Selector, it);
			}
			else {
				if (_Default != null) {
					throw new ArgumentException ("The default item was already set.", "it");
				}
				_Default = it;
			}

			it.Parent = this;

			return it;
		}

		/// <summary>
		/// Adds a new command from keyword and help.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="help">Descriptive help text</param>
		/// <returns>The added CMenuItem</returns>
		public CMenuItem Add (string selector, string help)
		{
			return Add (selector, (Action<string>) null, help);
		}

		/// <summary>
		/// Adds a new command from keyword.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <returns>The added CMenuItem</returns>
		public CMenuItem Add (string selector)
		{
			return Add (selector, null);
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword, behavior and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected.</param>
		/// <param name="help">Descriptive help text</param>
		/// <returns>The added CMenuItem</returns>
		public CMenuItem Add (string selector, Action<string> execute, string help = null)
		{
			var it = new CMenuItem (selector, execute, help);
			Add (it);
			return it;
		}

		/// <summary>
		/// Gets or sets the CMenuItem associated with the specified keyword.
		///
		/// Disabled items are returned. Use the null key to access the default item.
		/// </summary>
		/// <param name="key">
		/// Keyword of the CMenuItem. The selector must match perfectly (i.e. is not an abbreviation of the keyword).
		///
		/// If the key is null, the value refers to the default item.
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
					return _Default;
				}
				CMenuItem it;
				_Menu.TryGetValue (key, out it);
				return it;
			}
			set
			{
				if (key == null) {
					_Default = value;
				}
				else {
					_Menu[key] = value;
				}
			}
		}

		/// <summary>
		/// Returns the commands equal, or starting with, the specified cmd.
		///
		/// Does neither return the default menu item nor any disabled items.
		/// </summary>
		private CMenuItem[] GetCommands (string cmd, StringComparison comparison)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			CMenuItem mi;
			_Menu.TryGetValue (cmd, out mi);
			if (mi != null && !mi.IsVisible ()) {
				mi = null;
			}
			if (mi != null) {
				return new[] { mi };
			}

			var its = _Menu.Values
				.Where (it => it.Selector.StartsWith (cmd, comparison))
				.Where (it => it.IsVisible ())
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
		///
		/// If <c>useDefault</c> is set and a default item is present, it will be returned and no complaint will be generated.
		/// </summary>
		/// <param name="cmd">
		/// In: The command, possibly with arguments, from which the keyword is extracted which uniquely identifies the searched menu item.
		/// Out: The keyword uniquely identifying a menu item, or null if no such menu item was found.
		/// </param>
		/// <param name="args">
		/// Out: The arguments which were supplied in addition to a keyword.
		/// </param>
		/// <param name="complain">
		/// If true, clarifications about missing or superfluous matches will be written to stdout.
		/// </param>
		/// <param name="useDefault">
		/// The single closest matching menu item, or the default item if no better fit was found, or null in case of 0 or multiple matches.
		/// </param>
		/// <returns></returns>
		public CMenuItem GetMenuItem (ref string cmd, out string args, bool complain, bool useDefault)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			/*
			 * Is there a fitting child menu?
			 */
			var original = cmd;
			args = cmd;
			cmd = MenuUtil.SplitFirstWord (ref args);

			var its = GetCommands (cmd, StringComparison);

			if (its.Length == 1) {
				return its[0];
			}
			if (its.Length > 1) {
				if (complain) {
					var s = cmd == ""
						? "Command incomplete."
						: "Command <" + cmd + "> not unique.";
					Console.WriteLine (
						s + " Candidates: " +
						string.Join (", ", its.Select (it => it.Selector)));
				}
				return null;
			}

			/*
			 * Is there a fallback?
			 */
			var def = this[null];
			if (def != null) {
				cmd = null;
				args = original;
				return def;
			}

			/*
			 * We found nothing. Display this failure?
			 */
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
		/// Executes the specified command using only children (instead of this node's own behavior).
		/// </summary>
		/// <param name="args">Command to execute using contained commands.</param>
		public void ExecuteChild (string args)
		{
			var cmd = args;
			var it = GetMenuItem (ref cmd, out args, true, true);
			if (it != null) {
				it.Execute (args);
			}
		}

		/// <summary>
		/// Returns an enumerator over all menu items contained in this item.
		///
		/// The default item will not be enumerated.
		/// </summary>
		public IEnumerator<CMenuItem> GetEnumerator ()
		{
			return _Menu
				.Values
				.Where (mi => mi.IsVisible ())
				.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>
		/// Returns a dictionary containing all contained menu items and their corresponding abbreviation.
		///
		/// The abbreviations will be updated if commands are added, changed or removed.
		///
		/// The default menu item will not be returned.
		/// </summary>
		public IDictionary<string, string> CommandAbbreviations ()
		{
			var dict = new Dictionary<string, string> ();

			foreach (var it in this) {
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
				string dummy;
				if (GetMenuItem (ref sub, out dummy, false, false) != null) {
					return sub;
				}
			}
			return cmd;
		}

		public override string ToString ()
		{
			return "[" + Selector + "]";
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NConsoleMenu
{
	/// <summary>
	/// A single console menu item. It consists of a selector (keyword), a help text and the
	/// individual behavior. Also offers various ways to add, retrieve and use subitems.
	///
	/// <example>
	/// To create a hello world command:
	/// <code>
	/// var menuitem = new CMenuItem ("hello", s => Console.WriteLine ("Hello world!"));
	/// </code>
	/// </example>
	/// </summary>
	public class CMenuItem : IEnumerable<CMenuItem>
	{
		private static readonly StringComparer ICIC = StringComparer.InvariantCultureIgnoreCase;

		private Dictionary<string, CMenuItem> _Menu = new Dictionary<string, CMenuItem> (ICIC);
		private CMenuItem _Default = null;
		private StringComparison? _StringComparison;
		private Action<string> _Execute;
		private Func<bool> _Enabled;
		private CommandQueue _CQ;

		/// <summary>
		/// A factory method that constructs menu items when calling one of the Add methods.
		/// 
		/// This factory applies to Add methods of this instance as well as all its children
		/// (unless they override it with their own), so the factory should usually be set
		/// on the root of a menu tree.
		/// 
		/// By default, it simply constructs a CMenuItem, but you can replace
		/// that with any subclass of CMenuItem.
		/// </summary>
		// todo add to documentation
		public Func<string, Action<string>, string, CMenuItem> MenuItemFactory = null;

		public event Action<string> Write = null;

		public event Action<string> WriteLine = null;

		public event Action<ConsoleColor> SetForegroundColor = null;

		public event Action<ConsoleColor> SetBackgroundColor = null;

		public event Action ResetColor = null;

		protected void OnWrite (string s)
		{
			var e = Write;
			if (e != null) {
				e (s);
				return;
			}

			if (Parent != null) {
				Parent.OnWrite (s);
				return;
			}

			Console.Write (s);
		}

		protected void OnWriteLine (string s = null)
		{
			var e = WriteLine;
			if (e != null) {
				e (s);
				return;
			}

			if (Parent != null) {
				Parent.OnWriteLine (s);
				return;
			}

			var e2 = Write;
			if (e2 != null) {
				e2 (s + Environment.NewLine);
				return;
			}

			Console.WriteLine (s);
		}

		protected void OnSetForegroundColor (ConsoleColor c)
		{
			var e = SetForegroundColor;
			if (e != null) {
				e (c);
				return;
			}

			if (Parent != null) {
				Parent.OnSetForegroundColor (c);
				return;
			}

			Console.ForegroundColor = c;
		}

		protected void OnSetBackgroundColor (ConsoleColor c)
		{
			var e = SetBackgroundColor;
			if (e != null) {
				e (c);
				return;
			}

			if (Parent != null) {
				Parent.OnSetBackgroundColor (c);
				return;
			}

			Console.BackgroundColor = c;
		}

		protected void OnResetColor ()
		{
			var e = ResetColor;
			if (e != null) {
				e ();
				return;
			}

			if (Parent != null) {
				Parent.OnResetColor ();
				return;
			}

			Console.ResetColor ();
		}

		/// <summary>
		/// The command queue (CQ) associated with this menu item. Nested menus will
		/// use the same CQ as their parents.
		///
		/// The CQ keeps a stack of all commands to be executed by the menu item that
		/// has the focus at the time. It allows manually adding immediate or delayed
		/// input, which will be used instead of prompting the user for input on the
		/// console.
		/// </summary>
		public CommandQueue CQ
		{
			get {
				if (_CQ == null) {
					_CQ = new CommandQueue ();
				}
				return _CQ;
			}
		}

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
		/// <code>
		/// var m = new CMenuItem ("parent") {
		///	HelpText = "help", // object initializer
		///	MenuItem = { // collection initializer
		///		new CMenuItem ("child 1"),
		///		new CMenuItem ("child 2"),
		///	}
		/// };
		/// </code>
		/// </example>
		/// </remarks>
		/// </summary>
		public CMenuItem MenuItem { get { return this; } }

		/// <summary>
		/// Gets or sets how entered commands are compared.
		///
		/// <para>
		/// By default, the comparison is case insensitive and culture invariant.
		/// </para>
		/// </summary>
		public virtual StringComparison StringComparison
		{
			get {
				if (_StringComparison.HasValue) {
					return _StringComparison.Value;
				}
				if (Parent != null) {
					return Parent.StringComparison;
				}
				return StringComparison.InvariantCultureIgnoreCase;
			}
			set {
				_StringComparison = value;
				_Menu = new Dictionary<string, CMenuItem> (
					_Menu,
					value.GetCorrespondingComparer ());
			}
		}

		/// <summary>
		/// The Keyword used to select this item.
		/// </summary>
		public string Selector
		{
			get {
				return _Selector;
			}
			set {
				if (_Selector != value) {
					var p = Parent;
					if (p != null) {
						Parent.Remove (this);
					}
					_Selector = value;
					if (p != null) {
						p.Add (this);
					}
				}
			}
		}

		private string _Selector;

		/// <summary>
		/// Remove a child menu item.
		/// </summary>
		/// <param name="it">
		/// Item to be removed.
		/// </param>
		/// <returns>
		/// True iff the item was found and successfully removed.
		/// </returns>
		public bool Remove (CMenuItem it)
		{
			if (it.Parent != this) {
				throw new ArgumentException ("Item to be removed from parent is not a child", "it");
			}

			it.Parent = null;

			if (it == _Default) {
				_Default = null;
				return true;
			}

			return _Menu.Remove (it.Selector);
		}

		/// <summary>
		/// A descriptive help text.
		/// </summary>
		public string HelpText
		{
			get;
			set;
		}

		/// <summary>
		/// Sets the behavior upon selection. This overrides the default behavior of <c>Execute</c>.
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
		/// <para>
		/// If present, this node's behavior property will be executed.
		/// Else, execution will be delegated to the appropriate child.
		/// </para>
		///
		/// <remarks>
		/// When overriding <c>Execute</c> in a derived class, it is usually recommended to include a call to
		/// either <c>base.Execute</c> or <c>ExecuteChild</c>.
		/// </remarks>
		/// </summary>
		public virtual void Execute (string arg)
		{
			if (_Execute != null) {
				_Execute (arg);
				return;
			}

			if (!this.Any ()) {
				throw new NotImplementedException ("This menu item does not have an associated behavior yet.");
			}

			ExecuteChild (arg);
		}

		/// <summary>
		/// Executes the specified command using only children (instead of this node's own behavior).
		///
		/// If no fitting child could be found, an error message will be displayed.
		/// </summary>
		/// <param name="arg">
		/// Command to execute using contained commands.
		/// </param>
		public void ExecuteChild (string arg)
		{
			var cmd = arg;
			var it = GetMenuItem (ref cmd, out arg, true, true, false);
			if (it != null) {
				it.Execute (arg);
			}
		}

		/// <summary>
		/// Sets a condition function which determines if this menu item is enabled.
		///
		/// <para>
		/// Disabled menu items cannot be run and are excluded from command listings by <c>help</c>.
		/// </para>
		///
		/// <remarks>
		/// The condition is examined anew every time its result is needed, so it ought to be cheap to call.
		/// </remarks>
		/// </summary>
		public void SetEnablednessCondition (Func<bool> condition)
		{
			_Enabled = condition;
		}

		/// <summary>
		/// Returns true iff this item is enabled.
		///
		/// <para>
		/// Disabled menu items cannot be run and are excluded from command listings by <c>help</c>.
		/// </para>
		/// </summary>
		public virtual bool IsEnabled ()
		{
			if (_Enabled != null) {
				return _Enabled ();
			}

			return true;
		}

		/// <summary>
		/// Creates a new CMenuItem using the specified keyword, behavior and help text.
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
		/// Creates a new CMenuItem using the specified keyword.
		/// </summary>
		/// <param name="selector">Keyword</param>
		public CMenuItem (string selector)
			: this (selector, (Action<string>) null)
		{ }

		/// <summary>
		/// Creates a new command using the specified keyword.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <returns>The added CMenuItem</returns>
		public CMenuItem Add (string selector)
		{
			return Add (selector, null);
		}

		/// <summary>
		/// Creates a new command using the specified keyword and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="help">Descriptive help text</param>
		/// <returns>The added CMenuItem</returns>
		public CMenuItem Add (string selector, string help)
		{
			return Add (selector, null, help);
		}

		/// <summary>
		/// Creates a new CMenuItem using the specified keyword, behavior and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected.</param>
		/// <param name="help">Descriptive help text</param>
		/// <returns>The added CMenuItem</returns>
		public CMenuItem Add (string selector, Action<string> execute, string help = null)
		{
			CMenuItem it;
			var factory = ChainToRoot ()
				.Select (mi => mi.MenuItemFactory)
				.FirstOrDefault (fac => fac != null);
			if (factory != null) {
				it = factory (selector, execute, help);
			}
			else {
				it = new CMenuItem (selector, execute, help);
			}

			Add (it);
			return it;
		}

		/// <summary>
		/// Enumerates all menu items from this to root.
		/// </summary>
		public IEnumerable<CMenuItem> ChainToRoot ()
		{
			var p = this;
			do {
				yield return p;
				p = p.Parent;
			}
			while (p != null);
		}

		/// <summary>
		/// Adds a command.
		///
		/// <remarks>
		/// The menu's internal structure and abbreviations are updated automatically.
		/// </remarks>
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
			else {
				it.Parent = this;
			}

			if (it._CQ != null && !it._CQ.IsEmpty ()) {
				throw new ArgumentException ("Menuitem already has a nonempty command queue.", "it");
			}
			else {
				var orig = it._CQ;
				foreach (var mi in it.EnumerateTree ()) {
					Debug.Assert (mi._CQ == orig);
					mi._CQ = CQ;
				}
			}

			if (it.Selector != null) {
				if (_Menu.ContainsKey (it.Selector)) {
					throw new ArgumentException ("Selector of entry to be added is already in use.", "it");
				}
				_Menu.Add (it.Selector, it);
			}
			else {
				if (_Default != null) {
					throw new ArgumentException ("Tried to add a default item but the default item has already been set.", "it");
				}
				_Default = it;
			}

			return it;
		}

		/// <summary>
		/// Gets or sets the CMenuItem associated with the specified keyword.
		///
		/// <para>
		/// Disabled items are returned. Use the null key to access the default item.
		/// </para>
		/// </summary>
		///
		/// <param name="key">
		/// Keyword of the CMenuItem. The selector must match perfectly (i.e. is not an abbreviation of the keyword).
		///
		/// If the key is null, the value refers to the default item.
		/// </param>
		///
		/// <value>
		/// The CMenuItem associated with the specified keyword, or null.
		/// </value>
		///
		/// <returns>
		/// The menu item associated with the specified keyword.
		/// </returns>
		public CMenuItem this[string key]
		{
			get {
				if (key == null) {
					return _Default;
				}
				_Menu.TryGetValue (key, out var it);
				return it;
			}
			set {
				if (key == null) {
					_Default = value;
				}
				else {
					_Menu[key] = value;
				}
			}
		}

		/// <summary>
		/// Returns all commands equal to, or starting with, the specified argument.
		///
		/// <para>
		/// If a perfect match was found, it will be returned solely.
		/// Else, all prefix matches will be returned.
		/// </para>
		/// <para>
		/// Does not return the default menu item.
		/// </para>
		/// </summary>
		///
		/// <param name="includeDisabled">
		/// Disabled menu items are included iff this is set.
		/// </param>
		private CMenuItem[] GetCommands (string cmd, StringComparison comparison, bool includeDisabled)
		{
			if (cmd == null) {
				throw new ArgumentNullException ("cmd");
			}

			/*
			 * Is there a perfect hit?
			 */
			_Menu.TryGetValue (cmd, out var mi);
			if (!includeDisabled && mi != null && !mi.IsEnabled ()) {
				mi = null;
			}
			if (mi != null) {
				return new[] { mi };
			}

			/*
			 * Just return anything with a fitting prefix
			 */
			var its = _Menu.Values
				.Where (it => it.Selector.StartsWith (cmd, comparison))
				.Where (it => includeDisabled || it.IsEnabled ())
				.OrderBy (it => it.Selector)
				.ToArray ();
			return its;
		}

		/// <summary>
		/// Retrieves the <c>IMenuItem</c> associated with the specified keyword.
		///
		/// If no single item matches perfectly, the search will broaden to all items starting with the keyword.
		///
		/// In case sensitive mode, missing match which could be solved by different casing will re reported if
		/// <c>complain</c> is specified.
		///
		/// If <c>useDefault</c> is set and a default item is present, it will be returned and no complaint
		/// will be generated.
		/// </summary>
		///
		/// <param name="cmd">
		/// In: The command, possibly with arguments, from which the keyword is extracted which uniquely
		/// identifies the searched menu item.
		/// Out: The keyword uniquely identifying a menu item, or null if no such menu item was found.
		/// </param>
		/// <param name="args">
		/// Out: The arguments which were supplied in addition to a keyword.
		/// </param>
		/// <param name="complain">
		/// If true, clarifications about missing or superfluous matches will be written to stdout.
		/// </param>
		/// <param name="useDefault">
		/// The single closest matching menu item, or the default item if no better fit was found, or null in
		/// case of 0 or multiple matches.
		/// </param>
		/// <param name="includeDisabled">
		/// Disabled menu items are included iff this is set.
		/// </param>
		public CMenuItem GetMenuItem (
			ref string cmd,
			out string args,
			bool complain,
			bool useDefault,
			bool includeDisabled)
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

			var its = GetCommands (cmd, StringComparison, includeDisabled);

			if (its.Length == 1) {
				return its[0];
			}
			if (its.Length > 1) {
				if (complain) {
					var s = cmd == ""
						? "Command incomplete."
						: "Command <" + cmd + "> not unique.";
					OnWriteLine (
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
				OnWriteLine ("Unknown command: " + cmd);

				if (StringComparison.IsCaseSensitive ()) {
					var suggestions = GetCommands (cmd, StringComparison.InvariantCultureIgnoreCase, includeDisabled);
					if (suggestions.Length > 0) {
						if (suggestions.Length == 1) {
							OnWriteLine ("Did you mean \"" + suggestions[0].Selector + "\"?");
						}
						else if (suggestions.Length <= 5) {
							var sugs = string.Join (", ", suggestions
								.Take (suggestions.Length - 1)
								.Select (sug => "\"" + sug.Selector + "\""));
							var s = "Did you mean " + sugs + " or \"" + suggestions.Last ().Selector + "\"?";
							OnWriteLine (s);
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Returns an enumerator over all menu items contained in this item.
		/// <para>
		/// The default item will not be enumerated.
		/// </para>
		/// </summary>
		public IEnumerator<CMenuItem> GetEnumerator ()
		{
			return _Menu
				.Values
				.Where (mi => mi.IsEnabled ())
				.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>
		/// Enumerate, depth first, the tree of all child items, regardless of their status.
		/// </summary>
		protected IEnumerable<CMenuItem> EnumerateTree ()
		{
			var stack = new Stack<CMenuItem> ();
			stack.Push (this);

			while (stack.Any ()) {
				var mi = stack.Pop ();
				yield return mi;
				foreach (var child in mi._Menu.Values) {
					stack.Push (child);
				}
			}
		}

		/// <summary>
		/// Returns a dictionary containing all contained menu items and their corresponding abbreviation.
		///
		/// <para>
		/// The abbreviations will be updated if commands are added, changed or removed.
		/// </para>
		/// <para>
		/// The default menu item will not be returned.
		/// </para>
		/// <para>
		/// Hidden menu items will not be returned, though they are considered when generating abbreviations.
		/// </para>
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
				if (GetMenuItem (ref sub, out var dummy, false, false, true) != null) {
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

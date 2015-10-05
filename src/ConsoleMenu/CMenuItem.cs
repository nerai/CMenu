using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// A single console menu item. It consists of a selector (keyword), a help text and the individual behavior.
	///
	/// <example>
	/// Create a hellow world command:
	/// <code>
	/// var menuitem = new CMenuItem ("hello", s => Console.WriteLine ("Hello world!"));
	/// </code>
	/// </example>
	/// </summary>
	public class CMenuItem
	{
		/// <summary>
		/// Keyword to select this item.
		/// </summary>
		public readonly string Selector;

		/// <summary>
		/// Descriptive help text.
		/// </summary>
		public string HelpText;

		/// <summary>
		/// Behavior upon selection.
		/// </summary>
		internal Func<string, MenuResult> Execute;

		/// <summary>
		/// Creates a new CMenuItem from keyword, behavior and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected. The behavior provides feedback to the menu.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem (string selector, Func<string, MenuResult> execute, string help = null)
		{
			if (selector == null) {
				throw new ArgumentNullException ("selector");
			}

			Selector = selector;
			HelpText = help;
			SetAction (execute);
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword, behavior and help text.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem (string selector, Action<string> execute, string help = null)
		{
			if (selector == null) {
				throw new ArgumentNullException ("selector");
			}

			Selector = selector;
			HelpText = help;
			SetAction (execute);
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword.
		/// </summary>
		/// <param name="selector">Keyword</param>
		public CMenuItem (string selector)
			: this (selector, (Func<string, MenuResult>) null)
		{ }

		/// <summary>
		/// Sets the behavior upon selection
		/// </summary>
		/// <param name="action">
		/// Behavior when selected. The behavior provides feedback to the menu.
		/// </param>
		public void SetAction (Func<string, MenuResult> action)
		{
			Execute = action;
		}

		/// <summary>
		/// Sets the behavior upon selection
		/// </summary>
		/// <param name="action">
		/// Behavior when selected.
		/// </param>
		public void SetAction (Action<string> action)
		{
			if (action == null) {
				Execute = null;
			}
			else {
				Execute = s => {
					action (s);
					return MenuResult.Normal;
				};
			}
		}
	}
}

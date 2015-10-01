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
		/// Behavior upon selection.
		/// </summary>
		public Func<string, MenuResult> Execute;

		/// <summary>
		/// Descriptive help text.
		/// </summary>
		public string HelpText;

		/// <summary>
		/// Creates a new CMenuItem from keyword and behavior.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected. The behavior provides feedback to the menu.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem (string selector, Func<string, MenuResult> execute, string help)
		{
			Selector = selector;
			Execute = execute;
			HelpText = help;
		}

		/// <summary>
		/// Creates a new CMenuItem from keyword and behavior.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected. The behavior provides no feedback to the menu.</param>
		/// <param name="help">Descriptive help text</param>
		public CMenuItem (string selector, Action<string> execute, string help)
			: this (
			selector,
			s => { execute (s); return MenuResult.Normal; },
			help)
		{ }

		/// <summary>
		/// Creates a new CMenuItem from keyword and behavior.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected. The behavior provides feedback to the menu.</param>
		public CMenuItem (string selector, Func<string, MenuResult> execute)
			: this (selector, execute, "No help available.")
		{ }

		/// <summary>
		/// Creates a new CMenuItem from keyword and behavior.
		/// </summary>
		/// <param name="selector">Keyword</param>
		/// <param name="execute">Behavior when selected. The behavior provides no feedback to the menu.</param>
		public CMenuItem (string selector, Action<string> execute)
			: this (selector, execute, "No help available.")
		{ }

		/// <summary>
		/// Creates a new CMenuItem from keyword.
		/// </summary>
		/// <param name="selector">Keyword</param>
		public CMenuItem (string selector)
			: this (selector, (Func<string, MenuResult>) null, "No help available.")
		{ }
	}
}

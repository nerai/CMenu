using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// A single console menu item. It consists of a selector (keyword), a help text and the individual behavior.
	/// </summary>
	public interface IMenuItem
	{
		/// <summary>
		/// Keyword to select this item.
		/// </summary>
		string Selector { get; }

		/// <summary>
		/// Descriptive help text.
		/// </summary>
		string HelpText { get; }

		/// <summary>
		/// Behavior upon selection.
		/// </summary>
		MenuResult Execute (string arg);
	}
}

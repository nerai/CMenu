using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// Result of command processing.
	///
	/// Either <c>Normal</c> (no change) or <c>Quit</c> (menu processing exits).
	/// </summary>
	public enum MenuResult
	{
		/// <summary>
		/// Use default action based on context
		/// </summary>
		Default = 0,

		/// <summary>
		/// Do not process child menu items.
		/// </summary>
		Return = 0,

		/// <summary>
		/// Do continue execution in child menu items.
		/// </summary>
		Proceed = 10,

		/// <summary>
		/// Quit current menu execution (return control to containing menu).
		/// </summary>
		Quit = 255,
	}
}

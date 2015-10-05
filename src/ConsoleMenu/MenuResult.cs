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
		Normal = 0,
		Quit = 255,
	}
}

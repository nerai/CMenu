using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	/// <summary>
	/// Extension methods for StringComparison.
	/// </summary>
	public static class StringComparisonExtensions
	{
		/// <summary>
		/// Checks if a StringComparison value is case sensitive.
		/// </summary>
		public static bool IsCaseSensitive (this StringComparison sc)
		{
			return false
				|| sc == StringComparison.CurrentCulture
				|| sc == StringComparison.InvariantCulture
				|| sc == StringComparison.Ordinal;
		}

		/// <summary>
		/// Returns a StringComparer with the same comparison as the given StringComparison.
		/// </summary>
		public static StringComparer GetCorrespondingComparer (this StringComparison sc)
		{
			switch (sc) {
				case StringComparison.CurrentCulture:
					return StringComparer.CurrentCulture;

				case StringComparison.CurrentCultureIgnoreCase:
					return StringComparer.CurrentCultureIgnoreCase;

				case StringComparison.InvariantCulture:
					return StringComparer.InvariantCulture;

				case StringComparison.InvariantCultureIgnoreCase:
					return StringComparer.InvariantCultureIgnoreCase;

				case StringComparison.Ordinal:
					return StringComparer.Ordinal;

				case StringComparison.OrdinalIgnoreCase:
					return StringComparer.OrdinalIgnoreCase;

				default:
					throw new InvalidOperationException ("Unknown string comparison value.");
			}
		}
	}
}

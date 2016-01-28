using System;
using System.Collections.Generic;
using System.Globalization;
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
		/// Returns a culture which is appropriate for usage with the specified StringComparison.
		/// </summary>
		public static CultureInfo RelatedCulture (this StringComparison sc)
		{
			if (false
				|| sc == StringComparison.InvariantCulture
				|| sc == StringComparison.InvariantCultureIgnoreCase
				|| sc == StringComparison.Ordinal
				|| sc == StringComparison.OrdinalIgnoreCase) {
				return CultureInfo.InvariantCulture;
			}
			return CultureInfo.CurrentCulture;
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

		/// <summary>
		/// Returns true if a string contains a substring, using the specified culture and comparison options.
		/// </summary>
		public static bool Contains (this string s, string value, CultureInfo culture, CompareOptions options = CompareOptions.None)
		{
			return 0 <= culture.CompareInfo.IndexOf (s, value, options);
		}

		/// <summary>
		/// Returns true if a string contains a substring, using the specified StringComparison.
		/// </summary>
		public static bool Contains (this string s, string value, StringComparison sc)
		{
			CompareOptions co;
			switch (sc) {
			case StringComparison.CurrentCulture:
				co = CompareOptions.None;
				break;

			case StringComparison.CurrentCultureIgnoreCase:
				co = CompareOptions.IgnoreCase;
				break;

			case StringComparison.InvariantCulture:
				co = CompareOptions.None;
				break;

			case StringComparison.InvariantCultureIgnoreCase:
				co = CompareOptions.IgnoreCase;
				break;

			case StringComparison.Ordinal:
				co = CompareOptions.Ordinal;
				break;

			case StringComparison.OrdinalIgnoreCase:
				co = CompareOptions.OrdinalIgnoreCase;
				break;

			default:
				throw new InvalidOperationException ("Unknown string comparison value.");
			}

			return s.Contains (value, sc.RelatedCulture (), co);
		}
	}
}

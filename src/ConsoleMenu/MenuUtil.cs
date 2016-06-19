using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	public static class MenuUtil
	{
		private static readonly char[] Whitespace = new char[] { ' ', '\t' };

		/// <summary>
		/// Split the first word, separated by whitespace, from the rest of the string and return it.
		/// </summary>
		/// <param name="from">
		/// String from which to select the first word. This parameter will be changed to exclude the
		/// split off word.
		/// </param>
		/// <returns>
		/// First whitespace-separated word in argument.
		/// </returns>
		public static string SplitFirstWord (ref string from)
		{
			if (from == null) {
				throw new ArgumentNullException ("from");
			}

			var split = from.Split (Whitespace, 2, StringSplitOptions.RemoveEmptyEntries);
			from = split.Length > 1 ? split[1].TrimStart () : "";
			var word = split.Length > 0 ? split[0].Trim () : "";
			return word;
		}
	}
}

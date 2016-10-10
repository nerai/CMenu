using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu.Examples.Recording
{
	public class LooseSelectException : Exception
	{
		public LooseSelectException (string message) : base (message)
		{
		}
	}

	public class LooseSelectUtil
	{
		/// <summary>
		/// Loose string comparison. Returns the best match using increasingly inaccurate comparisons.
		/// Also makes sure there is a sole match at that level of accuracy.
		///
		/// Spaces in the select string are ignored.
		///
		/// The levels are:
		/// <list>
		/// <item>Perfect match (abcd in abcd)</item>
		/// <item>Prefix match (ab in abcd)</item>
		/// <item>Containing match (bc in abcd)</item>
		/// <item>Matching ordered sequence of characters (bd in abcd)</item>
		/// </list>
		///
		/// If no sufficiently unique match was found, a LooseSelectException is thrown.
		/// </summary>
		public static string LooseSelect (
			IEnumerable<string> options,
			string find,
			StringComparison sc)
		{
			find = find.Replace (" ", "");
			var ec = sc.GetCorrespondingComparer ();
			var matches = new List<string> ();
			int bestQuality = 0;

			foreach (var s in options) {
				int quality = -1;

				if (s.Equals (find, sc)) {
					quality = 10;
				}
				else if (s.StartsWith (find, sc)) {
					quality = 8;
				}
				else if (s.Contains (find, sc)) {
					quality = 6;
				}
				else if (StringContainsSequence (s, find, sc)) {
					quality = 3;
				}

				if (quality >= bestQuality) {
					if (quality > bestQuality) {
						bestQuality = quality;
						matches.Clear ();
					}
					matches.Add (s);
				}
			}

			if (matches.Count == 1) {
				return matches[0];
			}

			if (matches.Count > 1) {
				throw new LooseSelectException ("Identifier not unique: " + find);
			}
			else {
				throw new LooseSelectException ("Could not find identifier: " + find);
			}
		}

		private static bool StringContainsSequence (
			string str,
			string sequence,
			StringComparison sc)
		{
			int i = 0;
			foreach (var c in sequence) {
				i = str.IndexOf (c.ToString (), i, sc);
				if (i == -1) {
					return false;
				}
				i++;
			}
			return true;
		}
	}
}

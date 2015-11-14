using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	class Util
	{
		public static string LooseSelect (IEnumerable<string> source, string select, StringComparison sc)
		{
			select = select.Replace (" ", "");
			var ec = sc.GetCorrespondingComparer ();
			var matches = new List<string> ();
			int bestQuality = -1;

			foreach (var s in source) {
				int quality = -1;

				if (s.Equals (select, sc)) {
					quality = 10;
				}
				else if (s.StartsWith (select, sc)) {
					quality = 8;
				}
				else if (s.Contains (select, sc)) {
					quality = 6;
				}
				else if (StringContainsSequence (s, select)) {
					quality = 3;
				}
				else {
					quality = 0;
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
				Console.WriteLine ("Identifier not unique: " + select);
			}
			else {
				Console.WriteLine ("Could not find identifier: " + select);
			}
			return null;
		}

		private static bool StringContainsSequence (string str, string sequence)
		{
			int i = 0;
			foreach (var c in sequence) {
				i = str.IndexOf (c, i) + 1;
				if (i == 0) {
					return false;
				}
			}
			return true;
		}
	}
}

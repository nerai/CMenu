using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	public class MenuUtil
	{
		public static string SplitFirstWord (ref string from)
		{
			var split = from.Split (new char[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
			from = split.Length > 1 ? split[1].TrimStart () : "";
			var word = split.Length > 0 ? split[0].Trim () : "";
			return word;
		}
	}
}

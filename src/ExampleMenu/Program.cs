using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	public class Program
	{
		static void Main (string[] args)
		{
			Console.WriteLine ("Example project for CMenu");
			var menu = CreateMenu ();
			menu.Run ();
		}

		public static CMenu CreateMenu ()
		{
			var menu = new CMenu ();

			menu.Add (new MI_Echo ());
			menu.Add (new MI_If (menu));
			menu.Add (new MI_Pause ());
			menu.Add (new MI_Record ());
			menu.Add (new MI_Replay (menu));

			return menu;
		}
	}
}

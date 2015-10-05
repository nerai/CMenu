using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;

namespace ExampleMenu
{
	class Program
	{
		static void Main (string[] args)
		{
			Console.WriteLine ("Example project for CMenu");
			var menu = new CMenu ();

			menu.Add (new MI_Echo ());
			menu.Add (new MI_If (menu));
			menu.Add (new MI_Pause ());
			menu.Add (new MI_Record ());
			menu.Add (new MI_Replay (menu));

			menu.Run ();
		}
	}
}

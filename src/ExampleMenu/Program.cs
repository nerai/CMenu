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

			menu.Run ();
		}
	}
}

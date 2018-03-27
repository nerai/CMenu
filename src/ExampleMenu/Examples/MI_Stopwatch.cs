using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NConsoleMenu;

namespace NConsoleMenu.Sample.Examples
{
	public class MI_Stopwatch : CMenuItem
	{
		private Stopwatch _SW;

		public MI_Stopwatch ()
				: base ("stopwatch")
		{
			HelpText = "stopwatch start|stop\n"
				+ "Starts a stopwatch or stops it and displays the elapsed time.";

			Add ("start", s => {
				_SW = Stopwatch.StartNew ();
			});
			Add ("stop", s => {
				_SW.Stop ();
				OnWriteLine ("Elapsed: " + _SW.ElapsedMilliseconds + "ms");
			});
		}
	}
}

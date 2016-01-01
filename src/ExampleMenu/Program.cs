using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu;
using ExampleMenu.Procedures;
using ExampleMenu.Recording;

namespace ExampleMenu
{
	/// <summary>
	/// Test program for CMenu.
	/// </summary>
	class Program
	{
		static void Main (string[] args)
		{
			Console.WriteLine ("Simple CMenu demonstration");

			var mainmenu = new CMenu ();
			mainmenu.PromptCharacter = "main>";
			mainmenu.Add ("tutorial", s => new Tutorial ().Run ());
			mainmenu.Add ("tree-init", s => TreeInitialization ());
			mainmenu.Add ("disabled", s => DisabledCommands ());
			mainmenu.Add ("examples", s => Examples ());

			IO.ImmediateInput ("help");
			mainmenu.Run ();
		}

		static void TreeInitialization ()
		{
			/*
			 * It may be useful to create complex menu trees using collection initializers
			 */
			var m = new CMenu () {
				new CMenuItem ("1") {
					new CMenuItem ("1", s => Console.WriteLine ("1-1")),
					new CMenuItem ("2", s => Console.WriteLine ("1-2")),
				},
				new CMenuItem ("2") {
					new CMenuItem ("1", s => Console.WriteLine ("2-1")),
					new CMenuItem ("2", s => Console.WriteLine ("2-2")),
				},
			};
			m.PromptCharacter = "tree>";
			m.Run ();

			/*
			 * You can also combine object and collection initializers
			 */
			m = new CMenu () {
				PromptCharacter = "combined>",
				MenuItem = {
					new CMenuItem ("1", s => Console.WriteLine ("1")),
					new CMenuItem ("2", s => Console.WriteLine ("2")),
				}
			};
			m.Run ();
		}

		static bool DisabledCommandsEnabled = false;

		static void DisabledCommands ()
		{
			var m = new CMenu ();

			/*
			 * In this example, a global flag is used to determine the visibility of disabled commands.
			 * It is initially cleared, the 'enable' command sets it.
			 */
			DisabledCommandsEnabled = false;
			m.Add ("enable", s => DisabledCommandsEnabled = true);

			/*
			 * Create a new inline command, then set its visilibity function so it returns the above flag.
			 */
			var mi = m.Add ("inline", s => Console.WriteLine ("Disabled inline command was enabled!"));
			mi.SetVisibilityCondition (() => DisabledCommandsEnabled);

			/*
			 * Command abbreviations do not change when hidden items become visible, i.e. it is made sure they are already long
			 * enough. This avoids confusion about abbreviations suddenly changing.
			 */
			m.Add ("incollision", s => Console.WriteLine ("The abbreviation of 'incollision' is longer to account for the hidden 'inline' command."));

			/*
			 * It is also possible to override the visibility by subclassing.
			 */
			m.Add (new DisabledItem ());
			m.Run ();
		}

		private class DisabledItem : CMenuItem
		{
			public DisabledItem ()
				: base ("subclassed")
			{
				HelpText = "This command, which is defined in its own class, is disabled by default.";
			}

			public override bool IsVisible ()
			{
				return DisabledCommandsEnabled;
			}

			public override void Execute (string arg)
			{
				Console.WriteLine ("Disabled subclassed command was enabled!");
			}
		}

		static void Examples ()
		{
			var m = new CMenu ();

			m.Add (new MI_Add ());

			m.Add (new MI_Echo ());
			m.Add (new MI_If ());
			m.Add (new MI_Pause ());

			var frs = new FileRecordStore ();
			m.Add (new MI_Record (frs));
			m.Add (new MI_Replay (m, frs));

			var procmgr = new ProcManager ();
			m.Add (new MI_Proc (procmgr));
			m.Add (new MI_Call (m, procmgr));
			m.Add (new MI_Return (m, procmgr));
			m.Add (new MI_Goto (procmgr));

			IO.ImmediateInput ("help");
			m.Run ();
		}
	}
}

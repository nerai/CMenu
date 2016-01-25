using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleMenu;
using ExampleMenu.Examples;

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
			mainmenu.Add ("passive", s => PassiveMode ());
			mainmenu.Add (new ExamplesMenu ());

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

		static void PassiveMode ()
		{
			var m = new CMenu ();
			m.Add ("passive", s => {
				IO.PassiveMode = true;
				Console.WriteLine ("Passive mode selected. Input will be ignored.");
				Console.WriteLine ("A timer will be set which will input 'active' in 5 seconds.");
				new Thread (() => {
					for (int i = 5; i >= 0; i--) {
						Console.WriteLine (i + "...");
						Thread.Sleep (1000);
					}
					Console.WriteLine ("Sending input 'active' to the IO queue.");
					IO.ImmediateInput ("active");
				}).Start ();
			});
			m.Add ("active", s => {
				IO.PassiveMode = false;
				Console.WriteLine ("Active mode selected.");
			});

			Console.WriteLine ("IO is currently in active mode - you will be prompted for input.");
			Console.WriteLine ("The 'passive' command will turn passive mode on, which disables interactive input.");
			Console.WriteLine ("The 'active' command will turn active mode back on.");
			Console.WriteLine ("Please enter 'passive'.");

			m.Run ();
		}
	}
}

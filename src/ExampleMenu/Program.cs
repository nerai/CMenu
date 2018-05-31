using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NConsoleMenu;
using NConsoleMenu.Sample.Examples;

namespace NConsoleMenu.Sample
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
			mainmenu.Add ("promptless", s => PromptlessMode ());
			mainmenu.Add ("immediate", s => ImmediateMode ());
			mainmenu.Add (new ExamplesMenu ());
			mainmenu.Add (new OutputHook ());

			mainmenu.CQ.ImmediateInput ("help");
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

		static bool myEnabledProperty = false;

		/*
		 * Commands which cannot currently be used, but should still be available in the
		 * menu tree at other times, can disable themselves. Disabled commands cannot be
		 * used and are not listed by `help`.
		 */
		static void DisabledCommands ()
		{
			var m = new CMenu ();

			/*
			 * In this example, a global flag (`bool myEnabledProperty`) is used to
			 * determine the visibility of disabled commands. It is initially cleared,
			 * the `enable` command sets it.
			 */
			myEnabledProperty = false;
			m.Add ("enable", s => myEnabledProperty = true);

			/*
			 * Create a new inline command, then set its enabledness function so it
			 * returns the above flag.
			 */
			var mi = m.Add ("inline", s => Console.WriteLine ("Disabled inline command was enabled!"));
			mi.SetEnablednessCondition (() => myEnabledProperty);

			/*
			 * It is also possible to override the visibility by subclassing.
			 */
			m.Add (new DisabledItem ());
			m.Run ();

			/*
			 * Disabled commands are not displayed by `help`.
			 */

			/*
			 * Command abbreviations do not change when hidden items become visible,
			 * i.e. it is made sure they are already long enough. This avoids confusion
			 * about abbreviations suddenly changing.
			 */
			m.Add ("incollision", s => Console.WriteLine ("The abbreviation of 'incollision' is longer to account for the hidden 'inline' command."));
		}

		private class DisabledItem : CMenuItem
		{
			public DisabledItem ()
				: base ("subclassed")
			{
				HelpText = "This command, which is defined in its own class, is disabled by default.";
			}

			public override bool IsEnabled ()
			{
				return myEnabledProperty;
			}

			public override void Execute (string arg)
			{
				Console.WriteLine ("Disabled subclassed command was enabled!");
			}
		}

		static void PromptlessMode ()
		{
			var m = new CMenu ();
			m.Add ("promptless", s => {
				m.CQ.PromptUserForInput = false;
				Console.WriteLine ("Promptless mode selected. Input will be ignored.");
				Console.WriteLine ("A timer will be set which will input 'active' in 5 seconds.");
				new Thread (() => {
					for (int i = 5; i >= 0; i--) {
						Console.WriteLine (i + "...");
						Thread.Sleep (1000);
					}
					Console.WriteLine ("Sending input 'active' to the command queue.");
					m.CQ.ImmediateInput ("active");
				}).Start ();
			});
			m.Add ("active", s => {
				m.CQ.PromptUserForInput = true;
				Console.WriteLine ("Prompting mode selected again.");
			});

			Console.WriteLine ("IO is currently in active mode - you will be prompted for input.");
			Console.WriteLine ("The 'promptless' command will turn on promptless mode, which disables interactive input.");
			Console.WriteLine ("The 'active' command will turn active mode back on.");
			Console.WriteLine ("Please enter 'promptless' (or 'p').");

			m.Run ();
		}

		static void ImmediateMode ()
		{
			var m = new CMenu ();
			m.ImmediateMenuMode = true;
			m.Add ("foo", s => Console.WriteLine ("foo"));
			m.Add ("bar", s => Console.WriteLine ("bar"));
			m.Run ();
		}

		/// <summary>
		/// <para>
		/// This class demonstrates using output hooks. It intercepts Console.Write
		/// commands and prints all lower-case letters with a red background. It would just
		/// as well be possible to copy the printed text to a log file.
		///
		/// Any output a menu item writes is, by default, printed to the console. Output
		/// hooks allow intercepting this output. This can be useful to change or redirect
		/// it before printing, or to redirect or clone it to other locations (e.g. a
		/// logfile).
		///
		/// There are five kinds of output events that can be intercepted. They all refer
		/// to the equivalently named method or property of the Console class.
		/// <list type="bullet">
		/// <item>Write</item>
		/// <item>WriteLine</item>
		/// <item>SetForegroundColor</item>
		/// <item>SetBackgroundColor</item>
		/// <item>ResetColor</item>
		/// </list>
		/// </para>
		/// <para>
		/// It is recommended to let all custom menu items opt in to this pattern. For this
		/// purpose, simply call OnWrite instead of Console.Write, etc.
		///
		/// If no hook is present in a particular menu item, the call is redirected to its
		/// parent item. If no hook is found while moving up the chain, the regular method
		/// of the Console class is invoked. If a hook is found in a child item, the
		/// parent's hooks are not invoked.
		///
		/// Hooks can be added or removed at any time and for any menu item individually.
		/// </para>
		/// </summary>
		private class OutputHook : CMenuItem
		{
			public OutputHook () : base ("outputhook")
			{
				HelpText = "Demonstrates using output hooks.";

				Add ("add", s => Parent.Write += HookedWrite);
				Add ("remove", s => Parent.Write -= HookedWrite);
			}

			private static void HookedWrite (string s)
			{
				var fc = Console.ForegroundColor;
				var bc = Console.BackgroundColor;

				foreach (var c in s) {
					if (!char.IsLower (c)) {
						Console.Write (c);
					}
					else {
						Console.ForegroundColor = ConsoleColor.Black;
						Console.BackgroundColor = ConsoleColor.Red;
						Console.Write (c);
						Console.ForegroundColor = fc;
						Console.BackgroundColor = bc;
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleMenu.DefaultItems;

namespace ConsoleMenu
{
	/// <summary>
	/// A console menu structure, comprised of various menu items.
	///
	/// <example>
	/// To create a menu which displays the current time:
	/// <code>
	/// var menu = new CMenu ();
	/// menu.Add ("time", s => Console.WriteLine (DateTime.UtcNow));
	/// menu.Run ();
	/// </code>
	/// </example>
	/// </summary>
	public class CMenu : CMenuItem
	{
		private bool _Quit;

		/// <summary>
		/// Is executed before this menu begins processing
		/// </summary>
		public event Action<CMenu> OnRun = null;

		/// <summary>
		/// Is executed after this menu stopped processing
		/// </summary>
		public event Action<CMenu> OnQuit = null;

		/// <summary>
		/// Create a new CMenu.
		///
		/// <para>
		/// Iff no selector was specified, the menu will initially contain the following commands:
		/// <list type="bullet">
		/// <item><c>help</c></item>
		/// <item><c>quit</c></item>
		/// </list>
		/// </para>
		/// </summary>
		public CMenu (string selector = null)
			: base (selector)
		{
			if (selector == null) {
				Add (new MI_Quit (this));
				Add (new MI_Help (this));
			}
		}

		/// <summary>
		/// The string which is displayed in front of every prompt (i.e. query for user input).
		///
		/// <para>
		/// Set to null to disable explicit prompting.
		/// </para>
		/// </summary>
		public string PromptCharacter = "$";

		/// <summary>
		/// Start console prompting and processing.
		///
		/// <para>
		/// Immediately before processing begins, the event <c>OnRun</c> is called.
		/// Immediately after processing has ended, the event <c>OnQuit</c> called.
		/// </para>
		/// </summary>
		public void Run ()
		{
			_Quit = false;

			if (OnRun != null) {
				OnRun (this);
			}

			while (!_Quit) {
				var input = IO.QueryInput (PromptCharacter);
				ExecuteChild (input);
			}

			if (OnQuit != null) {
				OnQuit (this);
			}
		}

		/// <summary>
		/// Stops menu processing. Control will return to the parent menu.
		///
		/// <para>
		/// Processing can be restarted by calling <c>Run</c> again.
		/// </para>
		/// </summary>
		public void Quit ()
		{
			_Quit = true;
		}

		/// <summary>
		/// If this menu gets selected in its parent menu, run it.
		/// </summary>
		/// <param name="arg">
		/// If an additional nonempty argument is given, it will be executed in the local context once the menu is running.
		/// </param>
		public override void Execute (string arg)
		{
			if (!string.IsNullOrWhiteSpace (arg)) {
				IO.ImmediateInput (arg);
			}
			Run ();
		}
	}
}

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
		public event Action<CMenu> OnRun = delegate { };

		/// <summary>
		/// Is executed after this menu stopped processing
		/// </summary>
		public event Action<CMenu> OnQuit = delegate { };

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
		public CMenu (string selector = null, bool addDefaultItems = true)
			: base (selector)
		{
			if (addDefaultItems) {
				Add (new MI_Quit (this));
				Add (new MI_Help (this));
			}
		}

		/// <summary>
		/// The string which is displayed in front of every prompt (i.e. query for user input).
		///
		/// <para>
		/// Set to null if no prompt should be displayed.
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

			OnRun (this);

			while (!_Quit) {
				var input = CQ.QueryInput (PromptCharacter);
				ExecuteChild (input);
			}

			OnQuit (this);
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
		/// If this item gets selected in its parent menu, run it as a menu itself.
		/// </summary>
		/// <param name="arg">
		/// If an additional nonnull, nonwhitespace argument is given, it will be executed
		/// in the local menu context immediately.
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

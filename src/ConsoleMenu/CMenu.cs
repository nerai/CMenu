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
	/// Create a menu which can display the time:
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
		/// <item>help</item>
		/// <item>quit</item>
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
		/// Set to null to disable explicit prompting.
		/// </summary>
		public string PromptCharacter = "$";

		/// <summary>
		/// Start console promting and processing.
		///
		/// <para>
		/// Immediately before processing begins, the event OnRun is called.
		/// Immediately after processing has ended, the event OnQuit called.
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

		public void Quit ()
		{
			_Quit = true;
		}
	}
}




## Conditional commands

Commands which cannot currently be used, but should still be available in the menu tree at other times, can disable themselves. Disabled commands cannot be used and are not listed by `help`.

In this example, a global flag (`bool Enabled`) is used to determine if a command is enabled. It is initially cleared, the `enable` command sets it.

	m.Add ("enable", s => Enabled = true);

Create a new inline command, then set its enabledness function so it returns the above flag.

	var mi = m.Add ("inline", s => Console.WriteLine ("Disabled inline command was enabled!"));
	mi.SetEnablednessCondition (() => Enabled);

	$ inline
	Unknown command: inline
	$ enable
	$ inline
	Disabled inline command was enabled!

It is also possible to override the enabledness by subclassing.

	private class DisabledItem : CMenuItem
	{
		public DisabledItem ()
			: base ("subclassed")
		{
			HelpText = "This command, which is defined in its own class, is disabled by default.";
		}

		public override bool IsEnabled ()
		{
			return Enabled;
		}

		public override void Execute (string arg)
		{
			Console.WriteLine ("Disabled subclassed command was enabled!");
		}
	}

	$ subclassed
	Unknown command: subclassed
	$ enable
	$ subclassed
	Disabled subclassed command was enabled!

Disabled commands are not displayed by `help`:

	$ help
	Available commands:
	e   | enable
	h   | help
	q   | quit
	$ enable
	$ help
	Available commands:
	e   | enable
	h   | help
	i   | inline
	q   | quit
	s   | subclassed

Command abbreviations do not change when disabled items become enabled, i.e. it is made sure they are already long enough to be unique. This avoids confusion about abbreviations suddenly changing.

	m.Add ("incollision", s => Console.WriteLine ("The abbreviation of 'incollision' is longer to account for the hidden 'inline' command."));

	$ help
	Available commands:
	e   | enable
	h   | help
	inc | incollision
	q   | quit
	$ enable
	$ help
	Available commands:
	e   | enable
	h   | help
	inc | incollision
	inl | inline
	q   | quit
	s   | subclassed



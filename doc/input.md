# Input



## Immediate mode

By default, the user has to type in commands (and possibly their arguments). Sometimes though, only selection of options is required, similar to a classic 'menu'.

For this purpose, set the `ImmediateMode` flag in your menu. When run, it will display all available options by default (no need to type `help`). Each option will be preceded by a selection number, and simply entering that number will activate the corresponding menu item - no need to even press the Enter key.

	var m = new CMenu ();
	m.ImmediateMenuMode = true;
	m.Add ("foo", s => Console.WriteLine ("foo"));
	m.Add ("bar", s => Console.WriteLine ("bar"));
	m.Run ();

	 1 quit
	 2 help
	 3 foo
	 4 bar
	[presses 3 key]
	foo
	 1 quit
	 2 help
	 3 foo
	 4 bar



## Modifying the input queue

It is also possible to modify the input queue. The `IO` class provides flexible means to add input either directly or via an `IEnumerable<string>`. The latter allows you to stay in control over the input even after you added it, for instance by changing its content or canceling it.

Check out how the "repeat" command adds its argument to the input queue two times.

	// Add a command which repeats another command
	menu.Add ("repeat",
		s => {
			IO.ImmediateInput (s);
			IO.ImmediateInput (s);
		},
		"Repeats a command two times.");

	$ repeat hello
	Hello world!
	Hello world!
	$ r l 123
	String "123" has length 3
	String "123" has length 3



## Passive mode

By default, input is handled in prompting mode, i.e. CMenu will actively prompt the user for required input and read their console input.

This behavior may be undesirable if you want closer control, for instance:

* In a GUI environment, creating input in the GUI instead of the console as usual
* In a batch or shell environment, feeding stored input instead of prompting the user for it

To suppress active prompting, enable promptless mode by clearing the `PromptUserForInput` flag. The menu will then wait for programmatic input, e.g. via `CQ.AddInput`.


	IO is currently in active mode - you will be prompted for input.
	The 'promptless' command will turn on promptless mode, which disables interactive input.
	The 'active' command will turn active mode back on.
	Please enter 'promptless' (or 'p').

	$ p
	Promptless mode selected. Input will be ignored.
	A timer will be set which will input 'active' in 5 seconds.
	5...
	4...
	3...
	2...
	1...
	0...
	Sending input 'active' to the command queue.
	Prompting mode selected again.
	$ 

Side note: Switching from prompting to promptless mode during a prompting input query (i.e. while the user is being prompted for input) will cause the prompt to still wait for input after switching. This is due to limitations in the underlying system.

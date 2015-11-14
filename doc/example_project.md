# Example project

The source code contains an example project. It offers commands, which illustrate several (more or less advanced) use cases. It may be useful to reference them in your own projects.

## echo
Simply prints text to the console. This is probably most useful in batch processing.

### Help text
	echo [text]
	Prints the specified text to stdout.

### Example
	$ echo 123
	123

## if
Simple conditional execution. By default only supports the `not` operator and the constants `true` and `false`, but can be extended with arbitrary additional conditions.

Condition combination is not currently supported, though it can in part be emulated via chaining ("if <c1> if <c2> ...")
It is allowed to specify multiple concurrent `not`, each of which invert the condition again.

### Help text
	if [not] <condition> <command>
	Executes <command> if <condition> is met.
	If the modifier <not> is given, the condition result is reversed.
	
### Example
	$ if true echo 1
	1
	$ if not true echo 1
	$ if not false echo 1
	1

## pause

	pause
	Stops further operation until the enter key is pressed.

## record, replay
`record` and `replay` allow persisting several commands to disk for later reading them as input. This can be used for basic batch processing.

Replaying can be stopped via if `endreplay` is encountered as a direct command in the file. It does not work as an indirect statement (e.g. `if true endreplay`). For an example of how that functionality can be achieved, see `return`.

### record

	record name
	Records all subsequent commands to the specified file name.
	Recording can be stopped by the command endrecord
	Stored records can be played via the "replay" command.

### replay

	replay [name]
	Replays all commands stored in the specified file name, or
	Displays a list of all records.

	Replaying puts all stored commands in the same order on the stack as they were originally entered.
	Replaying stops when the line "endreplay" is encountered.

### Example

	$ record r1
	Recording started. Enter "endrecord" to finish.
	record> echo 1
	record> echo 2
	record> endrecord
	$ replay r1
	1
	2

## proc, call, return, goto
These implement a basic procedural calling system. Early exiting, jumping within the local procedure and reentrant calls are supported.

### Example

    $ proc p1
	Recording started. Enter "endproc" to finish.
	proc> echo In proc p1
	proc> return
	proc> echo This line will never be displayed.
	proc> endproc
	$ call p1
	In proc p1

	$ proc p2
	Recording started. Enter "endproc" to finish.
	proc> echo 1 - entered p2
	proc> goto g
	proc> echo 2 - this line will not be displayed.
	proc> :g
	proc> echo 3 - p2 completed
	proc> endproc
	$ call p2
	1 - entered p2
	3 - p2 completed


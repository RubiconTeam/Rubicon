extends GDNoteType

# This is a template for a custom note type in C#.
# Sorry if the amount of lines seem a little scary at first, there's a lot here I know!
# This can also act as a Node! So yes, you will have access to such things like _process(delta)

# This is what your note type will be named.
@export var type_name : StringName = &""

# This is basically your _ready() function.
# Do note that you can access the PlayField just by getting play_field!
func initialize() -> void:
	pass

# This is for modifying the initial note data before the notes spawn.
func initialize_note(_notes : Array[NoteData], _note_type : StringName):
	if type_name != _note_type:
		return
	
	# Add your code here!

# This is for customizing the note's graphic when a note spawns! (This note is a node.)
func spawn_note(_note : Note, _note_type : StringName) -> void:
	if type_name != _note_type:
		return
	
	# Add your code here!

# This is triggerred every time a note is hit/missed! Use this to modify "result".
func note_hit(_bar_line_name : StringName, _result : NoteResult) -> void:
	if type_name != _result.Note.Type:
		return
	
	# Add your code here!

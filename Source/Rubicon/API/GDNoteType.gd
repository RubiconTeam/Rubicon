class_name GDNoteType

extends Node

## A template for a note type in GDScript. Must be inherited.

var _initialized : bool = false

func _ready(): ## If it hasn't been initialized already, link itself to the play field
	if _initialized:
		return 
		
	var game : RubiconGame = RubiconEngine.GetGameInstance()
	var play_field : PlayField = game.PlayField
	var factory : NoteFactory = play_field.Factory
	
	factory.SpawnNote.connect(spawn_note)
	play_field.InitializeNote.connect(initialize_note) 
	play_field.NoteHit.connect(note_hit)
 
func initialize_note(_notes : Array[NoteData], _note_type : StringName) -> void: ## Used to set up note data initially for every note type.
	pass

func spawn_note(_note : Note, _note_type : StringName) -> void: ## Triggers when the factory spawns a note of this type. Use this to set up your note.
	pass

func note_hit(_bar_line_name : StringName, _result : NoteResult) -> void: ## Triggers every time a note of this type is hit.
	pass

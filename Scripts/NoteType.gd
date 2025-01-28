class_name NoteType

extends Node

func _ready():
	var game : RubiconGame = RubiconEngine.GetGameInstance()
	var play_field : PlayField = game.PlayField
	var factory : NoteFactory = play_field.Factory
	factory.SpawnNote.connect(spawn_note)
	
	var hit_event : RubiconEvent = play_field.NoteHitEvent
	hit_event.Add(note_hit)

func spawn_note(_note : Note, _note_type : StringName) -> void:
	pass

func note_hit(_bar_line : BarLine, _lane : int, _hit : int, _holding : bool) -> NoteResult:
	var result : NoteResult = NoteResult.new()
	result.Flags = NoteResultFlags.NONE
	return result
